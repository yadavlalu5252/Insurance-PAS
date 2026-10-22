
using DashboardNotification.Api.Middleware;
using DashboardNotification.Application.Interface;
using DashboardNotification.Infrastructure.Data;
using DashboardNotification.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database connection
builder.Services.AddDbContext<DashboardNotificationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")));

// Repository registration
builder.Services.AddScoped<INotificationTemplate, NotificationTemplateService>();

builder.Services.AddScoped<IDashboardMetrics, DashboardMetricsRepository>();
var app = builder.Build();

// Global exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
