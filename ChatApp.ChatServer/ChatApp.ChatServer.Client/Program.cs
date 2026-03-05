using ChatApp.ChatServer.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<ChatHubService>();
builder.Services.AddScoped<AppStateService>();

await builder.Build().RunAsync();
