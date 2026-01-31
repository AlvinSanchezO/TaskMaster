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

        default:
            Console.WriteLine("Unknow command. Try: add, list, or exit.");
            break;
    }
}