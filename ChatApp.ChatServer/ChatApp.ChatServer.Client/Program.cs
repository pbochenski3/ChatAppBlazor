using ChatApp.ChatServer.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<ChatHubService>();
builder.Services.AddScoped<AppStateService>();
builder.Services.AddScoped<ChatSettingsService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7255")

});

await builder.Build().RunAsync();
