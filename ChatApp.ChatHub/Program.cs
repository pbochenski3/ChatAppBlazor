using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Services;
using ChatApp.ChatHub;
using ChatApp.Infrastructure.Extensions;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("ChatDatabase"));
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
//Messages
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
//Users
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
//Contacts
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
//Invites
builder.Services.AddScoped<IInviteService, InviteService>();
builder.Services.AddScoped<IInviteRepository, InviteRepository>();
var app = builder.Build();
app.UseCors();
app.UseRouting();
app.MapHub<ChatHub>("/chathub");

app.Run();
