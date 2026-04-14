using ChatApp.Application.Interfaces;
using ChatApp.ChatServer.Client.Services.Api;
using ChatApp.ChatServer.Client.Services.Api.Interfaces;
using ChatApp.ChatServer.Client.Services.State;
using ChatApp.Infrastructure.Handlers;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<ChatHubService>();
builder.Services.AddScoped<AppStateService>();
builder.Services.AddScoped<ChatSettingsService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<IAuthClient, AuthClient>();
builder.Services.AddScoped<IChatApiClient, ChatApiClient>();
builder.Services.AddScoped<IContactApiClient, ContactApiClient>();
builder.Services.AddScoped<IInviteApiClient, InviteApiClient>();
builder.Services.AddScoped<ITokenProvider>(sp => sp.GetRequiredService<AppStateService>());
builder.Services.AddTransient<AuthorizationHandler>();
builder.Services.AddHttpClient("ChatAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7255");
})
.AddHttpMessageHandler<AuthorizationHandler>();
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ChatAPI");
});

await builder.Build().RunAsync();
