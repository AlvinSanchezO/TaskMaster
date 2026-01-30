using TaskMaster.CLI.Models;
using TaskMaster.CLI.Repositories;

// 1. Initialize the repository
var repository = new TaskRepository();

// 2. Create and add some tasks
repository.AddTask(new TaskItem("Setup Project", "Complete Ticket #001"));
repository.AddTask(new TaskItem("Create Models", "Complete Ticket #002"));
repository.AddTask(new TaskItem("Build Repository", "Working on Ticket #003"));

// 3. Retrieve and list all tasks
Console.WriteLine("--- Current Tasks in Repository ---");
var allTasks = repository.GetAllTasks();

foreach (var task in allTasks)
{
    Console.WriteLine(task);
}