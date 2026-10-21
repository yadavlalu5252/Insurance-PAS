using Microsoft.EntityFrameworkCore;
using PremiumSchedule.Infrastructure.Data;
using PremiumSchedule.Api.Middleware;
using PremiumSchedule.Application.Interfaces;
using PremiumSchedule.Infrastructure.Repositories;
using PremiumSchedule.Application.Mapper;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

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
