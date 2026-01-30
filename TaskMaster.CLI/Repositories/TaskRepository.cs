using TaskMaster.CLI.Models;

namespace TaskMaster.CLI.Repositories;

public class TaskRepository
{
    //private list: only this class can see it
    private readonly List<TaskItem> _tasks = new List<TaskItem>();

    //Add a task to the collection
    public void AddTask(TaskItem task)
    {
        _tasks.Add(task);
    }

    //Returns the list  as an IEnumerable to prevent external modification
    public IEnumerable<TaskItem> GetAllTasks()
    {
        return _tasks;
    }
}

