using ChatApp.Application.Interfaces;
using ChatApp.Application.Handlers;
using ChatApp.Web.Services.Actions;
using ChatApp.Web.Services.Api;
using ChatApp.Web.Services.Api.Interfaces;
using ChatApp.Web.Services.State;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ChatApp.Web.Services.Actions.Interfaces;
using ChatApp.Web.Services.Common;
using ChatApp.Web.Services.Common.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(Program).Assembly); });
builder.Services.AddScoped<AppStateService>();
builder.Services.AddScoped<ChatStateService>();
builder.Services.AddScoped<SidebarStateService>();
builder.Services.AddScoped<ISidebarActionService,SidebarActionService>();
builder.Services.AddScoped<IChatSettingsActionService,ChatSettingsActionService>();
builder.Services.AddScoped<IChatActionService,ChatActionService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<IImageApiClient, ImageApiClient>();
builder.Services.AddScoped<IAuthClient, AuthClient>();
builder.Services.AddScoped<IChatApiClient, ChatApiClient>();
builder.Services.AddScoped<IContactApiClient, ContactApiClient>();
builder.Services.AddScoped<IInviteApiClient, InviteApiClient>();
builder.Services.AddScoped<IGroupChatApiClient, GroupChatApiClient>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ChatHubService>();
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
