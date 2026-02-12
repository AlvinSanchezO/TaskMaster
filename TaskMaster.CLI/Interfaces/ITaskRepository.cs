using TaskMaster.CLI.Models;

namespace TaskMaster.CLI.Interfaces
{
    public interface ITaskRepository
    {
        List<TaskItem> GetAllTasks();
        void AddTask(TaskItem task);
        bool UpdateTaskStatus(string idInput, TaskMaster.CLI.Models.TaskStatus newStatus);
        bool DeleteTask(string idInput);
    }
}
