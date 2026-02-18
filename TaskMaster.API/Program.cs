using TaskMaster.CLI.Data;
using TaskMaster.CLI.Interfaces;
using TaskMaster.CLI.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro de Servicios e Infraestructura
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Configuración de la Base de Datos con la Connection String del appsettings.json
// Esto asegura que la API use el puerto 5433 definido para Docker
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Inyección del Repositorio
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

var app = builder.Build();

// 4. Configuración del Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 5. Endpoint GET real que consume la base de datos
app.MapGet("/api/tasks", (ITaskRepository repo) =>
{
    var tasks = repo.GetAllTasks();
    return Results.Ok(tasks);
})
.WithName("GetTasks");

app.Run();