namespace TaskMaster.CLI.Models;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    // Vincula la tarea con el ID del usuario que la creó
    public Guid UserId { get; set; }

    public TaskItem() { }

    // Actualizamos el constructor para que pida el UserId obligatoriamente
    public TaskItem(string title, string description, Guid userId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Status = TaskStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UserId = userId; // Asignamos el dueño
    }

    public override string ToString()
    {
        return $"[{Id.ToString()[..8]}] {Title} - Status: {Status} (Owner: {UserId.ToString()[..4]})";
    }
}