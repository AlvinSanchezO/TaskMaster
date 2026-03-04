namespace TaskMaster.CLI.Models;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#000000";

    // Relación inversa
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}