using AppChat.Tests;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

string connectionString = "Server=localhost\\SQLEXPRESS;Database=ChatAppDatabase;Trusted_Connection=True;TrustServerCertificate=True;";

var optionsBuilder = new DbContextOptionsBuilder<ChatDbContext>();
optionsBuilder.UseSqlServer(connectionString);

using (var context = new ChatDbContext(optionsBuilder.Options))
{
    var helper = new DbHelpers(context);
    Console.WriteLine("Na pewno chcesz wyczyscic baze danych?");
    Console.WriteLine("y/n");
    var answer = Console.ReadLine();
    if(answer.Equals("y"))
    {

    Console.WriteLine("Ostrzeżenie: Rozpoczynam pełny reset bazy danych...");

    try
    {
        await helper.ResetAllData();
        Console.WriteLine("Sukces! Baza została wyczyszczona.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Błąd podczas resetu: {ex.Message}");
    }
    }
}