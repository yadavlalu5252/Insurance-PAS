using Microsoft.EntityFrameworkCore;
using PolicyServicing.Application.Interface;
using PolicyServicing.Infrastructure.Config;
using PolicyServicing.Infrastructure.Data;
using PolicyServicing.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));


var policyUrl = builder.Configuration["ServiceUrls:PolicyService"];

builder.Services.AddHttpClient<PolicyClient>(client =>
{
    client.BaseAddress = new Uri(policyUrl);
});

var customerUrl = builder.Configuration["ServiceUrls:CustomerService"];

builder.Services.AddHttpClient<CustomerClient>(client =>
{
    client.BaseAddress = new Uri(customerUrl);
});

var premiumUrl = builder.Configuration["ServiceUrls:PremiumService"];
var claimsUrl = builder.Configuration["ServiceUrls:ClaimsService"];
var productUrl = builder.Configuration["ServiceUrls:ProductService"];

builder.Services.AddHttpClient<PremiumClient>(client =>
{
    client.BaseAddress = new Uri(premiumUrl);
});

builder.Services.AddHttpClient<ClaimsClient>(client =>
{
    client.BaseAddress = new Uri(claimsUrl);
});

builder.Services.AddHttpClient<ProductClient>(client =>
{
    client.BaseAddress = new Uri(productUrl);
});



builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IProcessCancellation, ProcessCancellationService>();
builder.Services.AddScoped<IEndorsementService, EndorsementService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IProcessRiderModify, ProcessRiderModify>();
builder.Services.AddScoped<IPolicyRenew, PolicyRenewalService>();
builder.Services.AddScoped<IProcessCoverage, PolicyCoverageService>();
builder.Services.AddScoped<IProcessNominee,ProcessNomineeService>();
builder.Services.AddScoped<IUpdateContact, UpdateContactService>();


builder.Services.AddMemoryCache();
builder.Services.AddScoped<OtpService>();




// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

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
