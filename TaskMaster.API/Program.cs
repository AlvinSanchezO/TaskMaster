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

//REGISTRO DE SERVICIOS 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CONFIGURACIÓN DE CORS 
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

//PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//ACTIVAR CORS 
app.UseCors("AllowBlazor");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//ENDPOINTS DE AUTENTICACIÓN

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

//ENDPOINTS DE TAREAS (PROTEGIDOS POR JWT) 

app.MapGet("/api/tasks", (ITaskRepository repo, ClaimsPrincipal user) =>
{
    var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdStr == null) return Results.Unauthorized();

    var currentUserId = Guid.Parse(userIdStr);
    var myTasks = repo.GetAllTasks().Where(t => t.UserId == currentUserId);
    return Results.Ok(myTasks);
})
.WithName("GetTasks")
.RequireAuthorization();

app.MapPost("/api/tasks", (CreateTaskRequest input, ITaskRepository repo, ClaimsPrincipal user) =>
{
    var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdStr == null) return Results.Unauthorized();

    var currentUserId = Guid.Parse(userIdStr);
    var newTask = new TaskItem(input.Title, input.Description ?? "", currentUserId);

    repo.AddTask(newTask);
    return Results.Created($"/api/tasks/{newTask.Id}", newTask);
})
.WithName("CreateTask")
.RequireAuthorization();

app.MapPut("/api/tasks/{id}/complete", (Guid id, ITaskRepository repo, ClaimsPrincipal user) =>
{
    var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdStr == null) return Results.Unauthorized();
    var currentUserId = Guid.Parse(userIdStr);

    var task = repo.GetAllTasks().FirstOrDefault(t => t.Id == id);
    if (task == null) return Results.NotFound();
    if (task.UserId != currentUserId) return Results.Forbid();

    var success = repo.UpdateTaskStatus(id.ToString(), TaskMaster.CLI.Models.TaskStatus.Completed);
    return success ? Results.NoContent() : Results.NotFound();
})
.WithName("CompleteTask")
.RequireAuthorization();

app.MapDelete("/api/tasks/{id}", (Guid id, ITaskRepository repo, ClaimsPrincipal user) =>
{
    var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdStr == null) return Results.Unauthorized();
    var currentUserId = Guid.Parse(userIdStr);

    var task = repo.GetAllTasks().FirstOrDefault(t => t.Id == id);
    if (task == null) return Results.NotFound();
    if (task.UserId != currentUserId) return Results.Forbid();

    var success = repo.DeleteTask(id.ToString());
    return success ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteTask")
.RequireAuthorization();

app.Run();

record CreateTaskRequest(string Title, string? Description);
record UserAuthRequest(string Username, string Password);