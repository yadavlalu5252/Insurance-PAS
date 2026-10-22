using Microsoft.EntityFrameworkCore;
using Underwriting.Application.Interfaces;
using Underwriting.Infrastructure.Config;
using Underwriting.Infrastructure.Data;
using Underwriting.Infrastructure.Mapper;
using Underwriting.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));
builder.Services.AddAutoMapper(cfg => { }, typeof(UnderwritingMappingProfile));
builder.Services.AddHttpClient<CustomerRegistrationClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["customerAddress"]
        ?? throw new InvalidOperationException(
            "Customer Registration URL is missing."));
});
builder.Services.AddScoped<Iunderwriting, underwritingService>();
builder.Services.AddScoped<Isavemedical, savemedicalServices>();
builder.Services.AddScoped<Isavefinancial, savefinancialservice>();
builder.Services.AddScoped< IRiskScoringService,RiskScoringService>();

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