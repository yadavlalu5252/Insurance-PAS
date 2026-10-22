using Microsoft.EntityFrameworkCore;
using Premium.Application.Mapper;
using PremiumPayment.Application.Interface;
using PremiumPayment.Infrastructure.Data;
using PremiumPayment.Infrastructure.Repositories;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")));



builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddScoped<IPremiumPaymentService, PremiumPaymentService>();
builder.Services.AddHttpClient<IPremiumScheduleClient, PremiumScheduleClient>(
    client =>
    {
        client.BaseAddress = new Uri("https://localhost:7179/");
    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();