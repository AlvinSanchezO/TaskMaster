using TaskMaster.CLI.Data;
using TaskMaster.CLI.Interfaces;
using TaskMaster.CLI.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro de Servicios (Estándar .NET 9)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Requiere el paquete Swashbuckle.AspNetCore

// 2. Tu Infraestructura Reutilizada
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

var app = builder.Build();

// 3. Configuración del Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Interfaz visual en /swagger/index.html
}

app.UseHttpsRedirection();

// 4. Endpoints (Eliminamos .WithOpenApi() para evitar el aviso de 'obsoleto')
app.MapGet("/api/tasks", (ITaskRepository repo) =>
{
    var tasks = repo.GetAllTasks();
    return Results.Ok(tasks);
})
.WithName("GetTasks"); // .WithOpenApi() ya no es necesario aquí en .NET 9

app.Run();