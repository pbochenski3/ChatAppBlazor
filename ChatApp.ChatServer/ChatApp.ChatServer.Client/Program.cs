using ChatApp.ChatServer.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<UserDateService>();

await builder.Build().RunAsync();
