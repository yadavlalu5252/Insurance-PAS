using Microsoft.EntityFrameworkCore;
using SupportAudit.Api.Infrastructure;
using SupportAudit.Application.Interfaces;
using SupportAudit.Infrastructure.Data;
using SupportAudit.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<RequestUserContext>();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));

// Dependency Injection
builder.Services.AddScoped<ISupportService, SupportService>();
builder.Services.AddScoped<IAuditService, AuditService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
