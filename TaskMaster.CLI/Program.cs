using TaskMaster.CLI.Models;
using TaskMaster.CLI.Repositories;

// Initialize our database
var repository = new TaskRepository();

Console.WriteLine("Welcome to TaskMaster CLI");
Console.WriteLine("Commands: add | list | complete | exit");

while (true)
{
    Console.Write("\n> Enter command: ");
    string command = Console.ReadLine()?.ToLower().Trim() ?? "";

    if (command == "exit")
    {
        Console.WriteLine("Goodbye!");
        break;
    }

    switch (command)
    {
        case "add":
            Console.Write("Enter Title: ");
            string title = Console.ReadLine() ?? "";

            Console.Write("Enter Description: ");
            string desc = Console.ReadLine() ?? "";

            var newTask = new TaskItem(title, desc);
            repository.AddTask(newTask);

            Console.WriteLine("Task added successfully!");
            break;

        case "list":
            // 1. Obtenemos todas las tareas primero
            var allTasks = repository.GetAllTasks().ToList();

            if (!allTasks.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\nYour task list is empty! Enjoy your free time.");
                Console.ResetColor();
            }
            else
            {
                // 2. Preguntamos por el filtro justo antes de mostrar nada
                Console.Write("Filter by status? (all | pending | completed) [all]: ");
                string filter = Console.ReadLine()?.ToLower().Trim() ?? "all";

                // 3. Filtramos la lista original usando LINQ
                var tasksToShow = filter switch
                {
                    "pending" => allTasks.Where(t => t.Status == TaskMaster.CLI.Models.TaskStatus.Pending).ToList(),
                    "completed" => allTasks.Where(t => t.Status == TaskMaster.CLI.Models.TaskStatus.Completed).ToList(),
                    _ => allTasks
                };

                // 4. Verificamos si el filtro nos dejó con algo que mostrar
                if (!tasksToShow.Any())
                {
                    Console.WriteLine($"\nNo tasks found with status: {filter}");
                }
                else
                {
                    // 5. Dibujamos la tabla usando solo las tareas filtradas (tasksToShow)
                    Console.WriteLine("\n" + new string('=', 60));
                    Console.WriteLine($"{"ID",-10} | {"TITLE",-25} | {"STATUS",-12}");
                    Console.WriteLine(new string('-', 60));

                    foreach (var task in tasksToShow)
                    {
                        Console.ForegroundColor = task.Status switch
                        {
                            TaskMaster.CLI.Models.TaskStatus.Completed => ConsoleColor.Green,
                            TaskMaster.CLI.Models.TaskStatus.InProgress => ConsoleColor.Yellow,
                            _ => ConsoleColor.White
                        };

                        string shortId = task.Id.ToString().Substring(0, 8);
                        string displayTitle = task.Title.Length > 25
                            ? task.Title.Substring(0, 22) + "..."
                            : task.Title;

                        Console.WriteLine($"{shortId,-10} | {displayTitle,-25} | {task.Status,-12}");
                    }

                    Console.ResetColor();
                    Console.WriteLine(new string('=', 60));
                }
            }
            break;

        case "complete":
            Console.Write("Enter Task ID (or first few characters): ");
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
            Console.WriteLine("Unknown command. Try: add, list, complete, or exit.");
            break;

        case "delete":
        {
            Console.Write("Enter the ID of the task to delete: ");
            string idToDelete = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(idToDelete)) break;

            //Confirm result 
            if (repository.DeleteTask(idToDelete)) 
            {
                Console.ForegroundColor =ConsoleColor.Red;
                Console.WriteLine("Task deleted successfully! ");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"Error: Task with ID starting with '{idToDelete}' not found.");
            }    
            break;
        }    
    }
}