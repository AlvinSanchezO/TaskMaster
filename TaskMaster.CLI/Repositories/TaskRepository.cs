using TaskMaster.CLI.Models;
using TaskMaster.CLI.Data;
using System.Collections.Generic;
using System.Linq;

namespace TaskMaster.CLI.Repositories;

public class TaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddTask(TaskItem task)
    {
        _context.Tasks.Add(task);
        _context.SaveChanges(); // INSERT ef core 
    }

    public List<TaskItem> GetAllTasks()
    {
        return _context.Tasks.OrderBy(t => t.CreatedAt).ToList();
    }

    // Método auxiliar para buscar por ID completo o parcial
    private TaskItem? GetTaskByIdOrPartial(string idInput)
    {
        // Si es un Guid completo, usamos Find (es más rápido)
        if (Guid.TryParse(idInput, out Guid fullGuid))
        {
            return _context.Tasks.Find(fullGuid);
        }

        // Si no es un Guid completo, buscamos tareas cuyo ID empiece con ese texto
        return _context.Tasks
            .AsEnumerable() // Traemos a memoria para comparar strings del Guid
            .FirstOrDefault(t => t.Id.ToString().StartsWith(idInput, StringComparison.OrdinalIgnoreCase));
    }

    public bool UpdateTaskStatus(string idInput, Models.TaskStatus newStatus)
    {
        var task = GetTaskByIdOrPartial(idInput);
        if (task == null) return false;

        task.Status = newStatus;
        _context.SaveChanges();
        return true;
    }

    public bool DeleteTask(string idInput)
    {
        var task = GetTaskByIdOrPartial(idInput);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        _context.SaveChanges();
        return true;
    }
}



