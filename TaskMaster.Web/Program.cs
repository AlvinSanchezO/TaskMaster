using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskMaster.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuración de conexión a la API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5144/")
});

await builder.Build().RunAsync();