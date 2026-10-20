using InsuranceProductManagement.Api.Middleware;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Application.Mapper;
using InsuranceProductManagement.Infrastructure.Data;
using InsuranceProductManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")));
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<ICoverageMasterService, CoverageMasterService>();

builder.Services.AddScoped<IRiderMasterService, RiderMasterService>();

builder.Services.AddScoped<IBenefitMasterService, BenefitMasterService>();

builder.Services.AddScoped<IPremiumFrequencyService, PremiumFrequencyService>();

builder.Services.AddScoped<IProductCommissionService, ProductCommissionService>();
builder.Services.AddScoped<IInsuranceProductService, InsuranceProductService>();
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(ProductCategoryMappingProfile).Assembly);

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