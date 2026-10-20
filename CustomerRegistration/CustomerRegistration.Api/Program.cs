
using CustomerRegistration.Api.Middleware;
using CustomerRegistration.Application.Interface;
using CustomerRegistration.Application.Interfaces;
using CustomerRegistration.Application.Mapper;
using CustomerRegistration.Infrastructure.Data;
using CustomerRegistration.Infrastructure.Repository;
using CustomerRegistration.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")
    ));

builder.Services.AddScoped<IDummyService, DummyService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerAddressService, CustomerAddressService>();
builder.Services.AddScoped<INomineeService, NomineeService>();
builder.Services.AddScoped<ICustomerDocumentService, CustomerDocumentService>();

builder.Services.AddAutoMapper(cfg => { },typeof(CustomerMappingProfile).Assembly);
var app = builder.Build();

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
