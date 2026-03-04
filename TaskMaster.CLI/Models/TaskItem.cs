namespace TaskMaster.CLI.Models;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }

    // Propiedades de categoría
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public TaskItem() { }

    public TaskItem(string title, string description, Guid userId)
    {
        Title = title;
        Description = description;
        UserId = userId;
    }
}