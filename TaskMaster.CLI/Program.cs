using TaskMaster.CLI.Models;
using TaskMaster.CLI.Repositories;

//Initialize our database
var repository = new TaskRepository();

Console.WriteLine("Welcome to TaskMaster CLI");
Console.WriteLine("Commands: add | list | exit");

while(true)
{
    Console.Write("\n> Enter command: ");
    string command = Console.ReadLine()?.ToLower().Trim() ?? "";

    if (command == "exit")
    {
        Console.WriteLine("GoodBye!");
        break;
    }

    switch (command)
    {
        case "add":
            Console.Write("Enter Tittle: ");
            string title = Console.ReadLine() ?? "";

            Console.Write("Enter Description: ");
            string desc = Console.ReadLine() ?? "";

            var newTask = new TaskItem(title, desc);
            repository.AddTask(newTask);

            Console.WriteLine("Task added sucessfully!");
            break;

        case "list":
            var tasks = repository.GetAllTasks();
            Console.WriteLine("\n--- YOUR TASKS ---");
            foreach (var task in tasks)
            {
                Console.WriteLine(task);
            }
            break;

        case "complete":
            Console.Write("Enter Task  ID (or first few character): ");
            string idInput = Console.ReadLine() ?? "";

            if (repository.UpdateTaskStatus(idInput, TaskMaster.CLI.Models.TaskStatus.Completed))
            {
                Console.WriteLine("Task marked as Completed!");
            }
            else
            {
                Console.WriteLine("Task not found with that ID.");
            }
            break;

        default:
            Console.WriteLine("Unknow command. Try: add, list, or exit.");
            break;

    }
}