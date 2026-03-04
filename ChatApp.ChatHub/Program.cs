using ChatApp.ChatHub;
using ChatApp.Infrastructure.Extensions;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddDatabase(builder.Configuration.GetConnectionString("ChatDatabase"));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7181")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
var app = builder.Build();
app.UseCors();
app.UseRouting();
app.MapHub<ChatHub>("/chathub");

app.Run();
