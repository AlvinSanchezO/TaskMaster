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

//Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

//Inyección de Dependencias
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>(); // Registro de UserService del CLI

//Configuración de Autenticación JWT
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

app.UseHttpsRedirection();
app.UseAuthentication(); 

//ENDPOINTS DE AUTENTICACIÓN 

// POST: Registrar un nuevo usuario
app.MapPost("/api/auth/register", (UserAuthRequest req, UserService userService) =>
{
    var user = userService.Register(req.Username, req.Password);
    return Results.Ok(new { message = "Usuario registrado", userId = user.Id });
})
.WithName("Register");

//POST: Login y generación de JWT
app.MapPost("/api/auth/login", (UserAuthRequest req, UserService userService, IConfiguration config) =>
{
    var user = userService.Login(req.Username, req.Password);
    if (user == null) return Results.Unauthorized();

    //Estructura del Token 
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

//EndPoints de Tareas (protegidos por JWT)

app.MapGet("/api/tasks", (ITaskRepository repo) => Results.Ok(repo.GetAllTasks())).WithName("GetTasks");

app.MapPost("/api/tasks", (CreateTaskRequest input, ITaskRepository repo) =>
{
    var newTask = new TaskItem(input.Title, input.Description ?? "");
    repo.AddTask(newTask);
    return Results.Created($"/api/tasks/{newTask.Id}", newTask);
}).WithName("CreateTask");

app.MapPut("/api/tasks/{id}/complete", (Guid id, ITaskRepository repo) =>
{
    var success = repo.UpdateTaskStatus(id.ToString(), TaskMaster.CLI.Models.TaskStatus.Completed);
    return success ? Results.NoContent() : Results.NotFound();
}).WithName("CompleteTask");

app.MapDelete("/api/tasks/{id}", (Guid id, ITaskRepository repo) =>
{
    var success = repo.DeleteTask(id.ToString());
    return success ? Results.NoContent() : Results.NotFound();
}).WithName("DeleteTask");

app.Run();

// DTOs
record CreateTaskRequest(string Title, string? Description);
record UserAuthRequest(string Username, string Password); // DTO para Auth