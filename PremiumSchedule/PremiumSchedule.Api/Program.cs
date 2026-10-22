using Hangfire;
using Microsoft.EntityFrameworkCore;
using PremiumSchedule.Api.Middleware;
using PremiumSchedule.Application.Interface;
using PremiumSchedule.Application.Interfaces;
using PremiumSchedule.Application.Mapper;
using PremiumSchedule.Infrastructure.Data;
using PremiumSchedule.Infrastructure.Repositories;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Evaluation;

// Add services to the container.

builder.Services.AddControllers();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")
    ));

builder.Services.AddScoped<IPremiumSchedule, PremiumScheduleService>();


builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(PremiumScheduleMapper).Assembly);
});

// Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("dbconn")));

builder.Services.AddHangfireServer();


builder.Services.AddHttpClient<IPolicyIssuanceClient, PolicyIssuanceClient>(
    client =>
    {
        client.BaseAddress = new Uri(
            "https://localhost:7179/");
    });


builder.Services.AddScoped<IPremiumPaymentService, PremiumPaymentService>();


builder.Services.AddScoped<IRefundService, RefundService>();


var app = builder.Build();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<IPremiumSchedule>(
    "update-overdue-premium-schedules",
    service => service.UpdateOverdueSchedules(
        10,
        500),
    Cron.Daily);

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
