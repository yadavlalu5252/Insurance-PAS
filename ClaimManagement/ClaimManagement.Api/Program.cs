using Asp.Versioning;
using ClaimManagement.Api.Middleware;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Mapper;
using ClaimManagement.Application.Wrapper;
using ClaimManagement.Infrastructure.Config;
using ClaimManagement.Infrastructure.Data;
using ClaimManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register controllers (bad request bodies also return the wrapper format)
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new BadRequestObjectResult(ApiResponse<object>.Fail("Invalid request", errors));
        };
    });

// API versioning: /api/v1/...
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Database, services, mapper
builder.Services.AddDbContext<ClaimDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));

var filePath = builder.Configuration["FileStorage:RootPath"]
    ?? Path.Combine(builder.Environment.ContentRootPath, "ClaimFiles");

builder.Services.AddScoped<IFileStorageService>(sp => new LocalFileStorageService(filePath));
builder.Services.AddScoped<IClaimDocumentService, ClaimDocumentService>();
builder.Services.AddScoped<IClaimWorkflowService, ClaimWorkflowService>();
builder.Services.AddScoped<IInvestigationService, InvestigationService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingData>());

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<PolicyClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["policyAddress"] ?? "http://localhost:5002");
});

builder.Services.AddHttpClient<PremiumClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["premiumAddress"] ?? "http://localhost:5003");
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();