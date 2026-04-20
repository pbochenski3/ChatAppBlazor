using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Services;
using ChatApp.Application.Services.Chats;
using ChatApp.Domain.Repository;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Providers;
using ChatApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using ChatApp.Web.Components;
using System.Text;
using ChatApp.Api.Components;
using ChatApp.Api.Services;
using ChatApp.Api;
using ChatApp.Api.Controllers;
using ChatApp.Application.Notifications;

var builder = WebApplication.CreateBuilder(args);

var JwtSetting = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(JwtSetting["Key"]!);
var issuer = JwtSetting["Issuer"];
var audience = JwtSetting["Audience"];
builder.Services.AddAntiforgery();
builder.Services.AddSignalR();
builder.Services.AddDbContextFactory<ChatDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatDatabase")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorAppPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7181") 
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,                            
        typeof(ChatDeletedNotification).Assembly 
    );
});
//Messagesne()
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddInfrastructure(builder.Configuration);
//Users
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
//Contacts
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
//Invites
builder.Services.AddScoped<IInviteService, InviteService>();
builder.Services.AddScoped<IInviteRepository, InviteRepository>();
//Chat
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IPrivateChatService, PrivateChatService>();
builder.Services.AddScoped<IGroupChatService, GroupChatService>();
builder.Services.AddScoped<IUserChatService, UserChatService>();
builder.Services.AddScoped<IChatReadStatusService, ChatReadStatusService>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
//SignalR - UserIdProvider
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
//Transaction provider for managing transactions across multiple repositories
builder.Services.AddScoped<ITransactionProvider, TransactionProvider>();
// Connection tracker for debugging SignalR connections
builder.Services.AddSingleton<ConnectionTracker>();
//JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
//UserChat
builder.Services.AddScoped<IUserChatRepository, UserChatRepository>();
//Sidebar
builder.Services.AddScoped<ISidebarService, SidebarService>();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
//FileService
var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "LocalS3");

string[] subFolders = { "Avatars", "GroupAvatars", "ChatImages" };
foreach (var folder in subFolders)
{
    var path = Path.Combine(storagePath, folder);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
}
builder.Services.AddScoped<IFileService>(sp => new LocalFileService(storagePath));

builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
var app = builder.Build();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storagePath),
    RequestPath = "/cdn"
});
app.UseStaticFiles();
app.MapStaticAssets();

app.UseCors("BlazorAppPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ChatApp.Web._Imports).Assembly);

app.Run();
