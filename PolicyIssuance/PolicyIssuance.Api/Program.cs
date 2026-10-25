
using Microsoft.EntityFrameworkCore;
using PolicyIssuance.Api.Middleware;
using PolicyIssuance.Application.Config;
using PolicyIssuance.Application.Interface;
using PolicyIssuance.Infrastructure.Clients;
using PolicyIssuance.Infrastructure.Data;
using PolicyIssuance.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")
    ));

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(MapperConfig).Assembly
);

builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IPolicyDocumentService, PolicyDocumentService>();
builder.Services.AddScoped<IPolicyEndorsementService, PolicyEndorsementService>();
builder.Services.AddScoped<IPolicyClosureService, PolicyClosureService>();

builder.Services.AddHttpClient<CustomerClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ServiceUrls:CustomerRegistration"]!);
});

builder.Services.AddHttpClient<KycClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ServiceUrls:KycVerification"]!);
});

builder.Services.AddHttpClient<QuoteClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ServiceUrls:QuoteGeneration"]!);
});

builder.Services.AddHttpClient<UnderwritingClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ServiceUrls:Underwriting"]!);
});

builder.Services.AddHttpClient<ProductClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ServiceUrls:ProductManagement"]!);
});

builder.Services.AddHttpClient<PremiumPaymentClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ServiceUrls:PremiumPayment"]!);
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS before authorization
app.UseCors("AngularPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
