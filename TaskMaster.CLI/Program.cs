using TaskMaster.CLI.Data;
using TaskMaster.CLI.Interfaces;
using TaskMaster.CLI.Models;
using TaskMaster.CLI.Repositories;

//Initialize our context
using var context = new AppDbContext();

// Initialize our database
ITaskRepository repository = new TaskRepository(context);

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
            Console.Write("Enter Task ID (full or short): ");
            string idInput = Console.ReadLine() ?? "";

            // Ya no parseamos a Guid aquí, el repositorio se encarga
            if (repository.UpdateTaskStatus(idInput, TaskMaster.CLI.Models.TaskStatus.Completed))
            {
                Console.WriteLine("Task marked as Completed!");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
            break;

        case "delete":
            Console.Write("Enter the ID of the task to delete (full or short): ");
            string idToDelete = Console.ReadLine() ?? "";

            if (repository.DeleteTask(idToDelete))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Task deleted successfully!");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
            break;
    }
}
