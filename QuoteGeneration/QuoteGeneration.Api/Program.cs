using QuoteGeneration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QuoteGeneration.Application.QuoteGeneration.Mapper;
using QuoteGeneration.Application.Interfaces;
using QuoteGeneration.Infrastructure.Services;
using QuoteGeneration.Infrastructure.Config;
using QuoteGeneration.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")));

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(QuoteMappingProfile).Assembly
);

builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IPremiumCalculationService, PremiumCalculationService>();
builder.Services.AddScoped<IRiskAssessmentService, RiskAssessmentService>();
builder.Services.AddScoped<IQuoteRiderService, QuoteRiderService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IQuoteHistoryService, QuoteHistoryService>();

builder.Services.AddHttpClient<InsuranceProductClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ServiceUrls:ProductManagement"]!);
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();