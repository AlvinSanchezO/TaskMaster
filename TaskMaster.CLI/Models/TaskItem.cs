namespace TaskMaster.CLI.Models;

public class TaskItem
{
    // 1. Propiedades
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    // 2. Constructor VACÍO (Obligatorio para Entity Framework)
    // EF usa este para "crear" el objeto antes de llenarlo con datos de la DB
    public TaskItem() { }

    // 3. Tu Constructor con parámetros (Para tu uso manual)
    public TaskItem(string title, string description)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Status = TaskStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    // 4. Métodos
    public override string ToString()
    {
        return $"[{Id.ToString()[..8]}] {Title} - Status: {Status} (Created: {CreatedAt:yyyy-MM-dd})";
    }
}