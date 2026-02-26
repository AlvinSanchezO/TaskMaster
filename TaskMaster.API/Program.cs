using TaskMaster.CLI.Data;
using TaskMaster.CLI.Interfaces;
using TaskMaster.CLI.Repositories;
using TaskMaster.CLI.Models;
using TaskMaster.CLI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- REGISTRO DE SERVICIOS ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CONFIGURACIÓN DE CORS (Puerto de Blazor: 5049)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
        policy.WithOrigins("http://localhost:5049")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Inyección de Dependencias
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();

// Configuración de Autenticación JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// --- PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazor");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// --- ENDPOINTS DE AUTENTICACIÓN [#TM-025] ---

app.MapPost("/api/auth/register", (UserAuthRequest req, UserService userService) =>
{
    var user = userService.Register(req.Username, req.Password);
    return Results.Ok(new { message = "Usuario registrado", userId = user.Id });
})
.WithName("Register");

app.MapPost("/api/auth/login", (UserAuthRequest req, UserService userService, IConfiguration config) =>
{
    var user = userService.Login(req.Username, req.Password);
    if (user == null) return Results.Unauthorized();

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: config["Jwt:Issuer"],
        audience: config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(3),
        signingCredentials: creds
    );

    return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
})
.WithName("Login");

// --- ENDPOINTS DE CATEGORÍAS [#TM-027] ---

app.MapGet("/api/categories", async (AppDbContext db) =>
    Results.Ok(await db.Categories.ToListAsync()))
    .WithName("GetCategories")
    .RequireAuthorization();

app.MapPost("/api/categories", async (Category category, AppDbContext db) => {
    db.Categories.Add(category);
    await db.SaveChangesAsync();
    return Results.Created($"/api/categories/{category.Id}", category);
})
.RequireAuthorization();

// --- ENDPOINTS DE TAREAS (CON CATEGORY) [#TM-027] ---

app.MapGet("/api/tasks", async (AppDbContext db, ClaimsPrincipal user) =>
{
    var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdStr == null) return Results.Unauthorized();

    var currentUserId = Guid.Parse(userIdStr);

    var myTasks = await db.Tasks
        .Include(t => t.Category) // Carga relacional para ver colores
        .Where(t => t.UserId == currentUserId)
        .ToListAsync();

    return Results.Ok(myTasks);
})
.WithName("GetTasks")
.RequireAuthorization();

app.MapPost("/api/tasks", async (CreateTaskRequest input, AppDbContext db, ClaimsPrincipal user) =>
{
    var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdStr == null) return Results.Unauthorized();

    var currentUserId = Guid.Parse(userIdStr);

    var newTask = new TaskItem(input.Title, input.Description ?? "", currentUserId)
    {
        CategoryId = input.CategoryId
    };

    db.Tasks.Add(newTask);
    await db.SaveChangesAsync();

    return Results.Created($"/api/tasks/{newTask.Id}", newTask);
})
.WithName("CreateTask")
.RequireAuthorization();

app.MapPut("/api/tasks/{id}/complete", async (Guid id, AppDbContext db) => {
    var task = await db.Tasks.FindAsync(id);
    if (task == null) return Results.NotFound();
    task.Status = TaskMaster.CLI.Models.TaskStatus.Completed;
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.RequireAuthorization();

app.MapDelete("/api/tasks/{id}", async (Guid id, AppDbContext db) => {
    var task = await db.Tasks.FindAsync(id);
    if (task == null) return Results.NotFound();
    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.RequireAuthorization();

// --- SEEDER DE CATEGORÍAS [#TM-027] ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Categories.Any())
    {
        db.Categories.AddRange(
            new Category { Name = "Trabajo", ColorHex = "#0d6efd" },
            new Category { Name = "Personal", ColorHex = "#198754" },
            new Category { Name = "Urgente", ColorHex = "#dc3545" },
            new Category { Name = "Estudios", ColorHex = "#6f42c1" }
        );
        db.SaveChanges();
    }
}

app.Run();

// --- DTOs ---
record CreateTaskRequest(string Title, string? Description, Guid? CategoryId);
record UserAuthRequest(string Username, string Password);