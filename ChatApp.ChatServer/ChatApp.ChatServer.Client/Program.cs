using ChatApp.ChatServer.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<UserDateService>();
builder.Services.AddScoped<ChatHubService>();

await builder.Build().RunAsync();
