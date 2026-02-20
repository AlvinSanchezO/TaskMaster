using TaskMaster.CLI.Data;
using TaskMaster.CLI.Interfaces;
using TaskMaster.CLI.Repositories;
using Microsoft.EntityFrameworkCore;
using TaskMaster.CLI.Models;

var builder = WebApplication.CreateBuilder(args);

// Registro de Servicios e Infraestructura
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de la Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Inyección del Repositorio
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

var app = builder.Build();

// Configuración del Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- ENDPOINTS ---

// GET: Obtener todas las tareas
app.MapGet("/api/tasks", (ITaskRepository repo) =>
{
    return Results.Ok(repo.GetAllTasks());
})
.WithName("GetTasks");

// POST: Create a new task
app.MapPost("/api/tasks", (CreateTaskRequest input, ITaskRepository repo) =>
{
    var newTask = new TaskItem(input.Title, input.Description ?? "");
    repo.AddTask(newTask);
    return Results.Created($"/api/tasks/{newTask.Id}", newTask);
})
.WithName("CreateTask");

// PUT: Complete a task
app.MapPut("/api/tasks/{id}/complete", (Guid id, ITaskRepository repo) =>
{
    var success = repo.UpdateTaskStatus(id.ToString(), TaskMaster.CLI.Models.TaskStatus.Completed);

    return success ? Results.NoContent() : Results.NotFound();
})
.WithName("CompleteTask");

// DELETE: Quit a task
app.MapDelete("/api/tasks/{id}", (Guid id, ITaskRepository repo) =>
{
    var success = repo.DeleteTask(id.ToString());
    return success ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteTask");
app.Run();

// DTO for creating a new task
record CreateTaskRequest(string Title, string? Description);