using AgentManagement.Api.Middleware;
using AgentManagement.Application.Interface;
using AgentManagement.Infrastructure.Config;
using AgentManagement.Infrastructure.Data;
using AgentManagement.Infrastructure.Service;
using AgentManagement.Infrastructure.Service.AgentManagement.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container
builder.Services.AddControllers();

builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IAgentCustomerService, AgentCustomerService>();
builder.Services.AddScoped<IAgentPolicyService, AgentPolicyService>();
builder.Services.AddScoped<ICommissionRuleService, CommissionRuleService>();
builder.Services.AddScoped<IAgentCommissionService, AgentCommissionService>();

builder.Services.AddScoped<IAgentCommissionPaymentService, AgentCommissionPaymentService>();



builder.Services.AddAutoMapper(cfg => { }, typeof(MapperConfig).Assembly);

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")
    ));

var app = builder.Build();

app.UseCors("AllowAngular");

app.UseMiddleware<GlobalExceptionMiddleware>();


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();