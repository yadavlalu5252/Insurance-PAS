using DashboardNotification.Application.Helpers;
using DashboardNotification.Application.Services;
using DashboardNotification.Infrastructure.Data;
using DashboardNotification.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var options = new DbContextOptionsBuilder<DashboardNotificationDbContext>()
    .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=InsuranceDb;Integrated Security=True;Encrypt=True")
    .Options;

using var db = new DashboardNotificationDbContext(options);

var repository = new NotificationTemplateService(db);

var service = new NotificationMessageService(repository);

var values = new Dictionary<string, string>
{
    { "CustomerName", "Abhijit" },
    { "PolicyNumber", "POL1001" }
};

string? message = service.PrepareMessage(2, values);

Console.WriteLine(message ?? "Template not found or body is empty.");
