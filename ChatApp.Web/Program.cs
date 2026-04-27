using ChatApp.Application.Interfaces;
using ChatApp.Web.Handlers;
using ChatApp.Web.Services.Actions;
using ChatApp.Web.Services.Actions.Interfaces;
using ChatApp.Web.Services.Api;
using ChatApp.Web.Services.Api.Interfaces;
using ChatApp.Web.Services.Common;
using ChatApp.Web.Services.Common.Interfaces;
using ChatApp.Web.Services.State;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(Program).Assembly); });
builder.Services.AddScoped<AppStateService>();
builder.Services.AddScoped<ChatStateService>();
builder.Services.AddScoped<SidebarStateService>();
builder.Services.AddScoped<ISidebarActionService, SidebarActionService>();
builder.Services.AddScoped<IChatSettingsActionService, ChatSettingsActionService>();
builder.Services.AddScoped<IChatActionService, ChatActionService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<IImageApiClient, ImageApiClient>();
builder.Services.AddScoped<IAuthClient, AuthApiClient>();
builder.Services.AddScoped<IChatApiClient, ChatApiClient>();
builder.Services.AddScoped<IContactApiClient, ContactApiClient>();
builder.Services.AddScoped<IInviteApiClient, InviteApiClient>();
builder.Services.AddScoped<IGroupChatApiClient, GroupChatApiClient>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ChatHubService>();
builder.Services.AddTransient<AuthorizationHandler>();
builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    UseCookies = true
});
builder.Services.AddHttpClient("MessengerAPI", client =>
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthorizationHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MessengerAPI"));
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
