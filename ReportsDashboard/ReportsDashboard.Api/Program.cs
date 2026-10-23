
using Microsoft.EntityFrameworkCore;
using ReportsDashboard.Api.Services;
using ReportsDashboard.Application.Interfaces;
using ReportsDashboard.Infrastructure.Config;
using ReportsDashboard.Infrastructure.Data;
using ReportsDashboard.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICustomerReport, customerreportservice>();
builder.Services.AddScoped<IPolicyReport, policyreportservice>();
builder.Services.AddScoped<Ipremiumreport, premiumreportservice>();
builder.Services.AddScoped<Iclaimsreport, claimsreportservice>();
builder.Services.AddScoped<Iunderwritingreport, underwritingreportservice>();
builder.Services.AddScoped<Iagentreport, agentreportservice>();
builder.Services.AddScoped<Ifinancialreport, financialreportservice>();

builder.Services.AddScoped<Ischeduledreport, scheduledreportservice>();

builder.Services.AddHttpClient<CustomerClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["customerAddress"] ?? "http://localhost:5129");
});

builder.Services.AddHttpClient<PolicyClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["policyAddress"] ?? "http://localhost:5092");
});

builder.Services.AddHttpClient<PremiumClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["premiumAddress"] ?? "http://localhost:5093");
});

builder.Services.AddHttpClient<ClaimsClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["claimsAddress"] ?? "http://localhost:5094");
});

builder.Services.AddHttpClient<UnderwritingClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["underwritingAddress"] ?? "http://localhost:5095");
});

builder.Services.AddHttpClient<AgentClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["agentAddress"] ?? "http://localhost:5096");
});

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
