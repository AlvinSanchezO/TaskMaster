using TaskMaster.CLI.Data;
using TaskMaster.CLI.Interfaces;
using TaskMaster.CLI.Models;
using TaskMaster.CLI.Repositories;
using Microsoft.Extensions.DependencyInjection;

// --- CAMBIO AQUÍ: Envolver en una clase ---
namespace TaskMaster.CLI;

public class Program
{
    public static void Main(string[] args)
    {
        // Pega TODO el código que ya tenías aquí adentro:
        var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>()
            .AddScoped<ITaskRepository, TaskRepository>()
            .BuildServiceProvider();

        using var score = serviceProvider.CreateScope();
        var repository = score.ServiceProvider.GetRequiredService<ITaskRepository>();

        Console.WriteLine("Welcome to TaskMaster CLI");
        // ... (el resto de tu lógica del switch y los comandos)
    }
}