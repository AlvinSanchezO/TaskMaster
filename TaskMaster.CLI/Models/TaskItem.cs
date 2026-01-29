namespace TaskMaster.CLI.Models;
public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } =  string.Empty;
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }


//Constructor
public TaskItem(string title, string description)
    {
        Id = Guid.NewGuid(); //Generates a new ID automatically
        Title = title;
        Description = description;
        Status = TaskStatus.Pending; //Default Status
        CreatedAt = DateTime.Now; //Sets the creation time
    }

    //Override ToString for better display
    public override string ToString()
    {
        return $"[{Id.ToString()[..8]}] {Title} - Status: {Status} (Created: {CreatedAt:yyyy-MM-dd})";
    }
}