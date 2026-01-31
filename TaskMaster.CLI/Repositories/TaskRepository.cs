using System.Text.Json;
using TaskMaster.CLI.Models;

namespace TaskMaster.CLI.Repositories;

public class TaskRepository
{
    private const string FilePath = "tasks.json";
    private List<TaskItem> _tasks = new();

    public TaskRepository()
    {
        // Al iniciar el repositorio, intentamos cargar lo que haya en el disco
        LoadFromFile();
    }

    public void AddTask(TaskItem task)
    {
        _tasks.Add(task);
        SaveToFile(); // Guardamos automáticamente al añadir
    }

    public IEnumerable<TaskItem> GetAllTasks() => _tasks;

    private void SaveToFile()
    {
        // JsonSerializerOptions hace que el JSON se vea ordenado (con sangría)
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(_tasks, options);
        File.WriteAllText(FilePath, jsonString);
    }

    private void LoadFromFile()
    {
        if (!File.Exists(FilePath)) return;

        try
        {
            string jsonString = File.ReadAllText(FilePath);
            _tasks = JsonSerializer.Deserialize<List<TaskItem>>(jsonString) ?? new List<TaskItem>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tasks: {ex.Message}");
            _tasks = new List<TaskItem>();
        }
    }
}