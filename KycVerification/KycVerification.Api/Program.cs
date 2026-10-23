using KycVerification.Application.Interface;
using KycVerification.Application.Mapper;
using KycVerification.Infrastructure.Data;
using KycVerification.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Framework services
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")));

// AutoMapper
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(KycMappingProfile));

// Services
builder.Services.AddScoped<
    IDocumentTypeMasterServices,
    DocumentTypeMasterServices>();

builder.Services.AddScoped<
    ICustomerKycServices,
    CustomerKycServices>();

builder.Services.AddScoped<
    IKycService,
    KycService>();

builder.Services.AddScoped<
    ICkycDetailsServices,
    CkycDetailsServices>();

builder.Services.AddScoped<
    IKycDocumentServices,
    KycDocumentServices>();

builder.Services.AddScoped<
    IKycVerificationLogServices,
    KycVerificationLogServices>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AngularClient");

app.UseAuthorization();

app.MapControllers();

app.Run();