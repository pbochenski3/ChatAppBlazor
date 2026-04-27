using ChatApp.Api;
using ChatApp.Api.Components;
using ChatApp.Api.Services;
using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Interfaces.Repository;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Providers;
using ChatApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
        policy.WithOrigins("https://localhost:7255")
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
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,
        typeof(ChatDeletedNotification).Assembly
    );
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});
//Messagesne()
builder.Services.AddInfrastructure(builder.Configuration);
//Users
builder.Services.AddScoped<IUserRepository, UserRepository>();
//Contacts
builder.Services.AddScoped<IContactRepository, ContactRepository>();
//Invites
builder.Services.AddScoped<IInviteRepository, InviteRepository>();
//Chat
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
//SignalR - UserIdProvider
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
// Connection tracker for debugging SignalR connections
builder.Services.AddSingleton<ConnectionTracker>();
//JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
//UserChat
builder.Services.AddScoped<IUserChatRepository, UserChatRepository>();
//Sidebar
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
app.UseHttpsRedirection();
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
