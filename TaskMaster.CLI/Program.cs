// 1. Esta línea es VITAL para conectar las carpetas
using TaskMaster.CLI.Models;

Console.WriteLine("=== Testing TaskMaster Model ===");

// 2. Ahora sí puedes instanciar la clase
var myTask = new TaskItem("Finish Ticket 002", "Models and Enums implemented");

// 3. Gracias a tu Override, esto imprimirá los datos limpios
Console.WriteLine(myTask);