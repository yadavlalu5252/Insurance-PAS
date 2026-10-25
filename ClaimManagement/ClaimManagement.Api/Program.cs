using Asp.Versioning;
using ClaimManagement.Api.Middleware;
using ClaimManagement.Api.Security;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Mapper;
using ClaimManagement.Application.Wrapper;
using ClaimManagement.Infrastructure.Config;
using ClaimManagement.Infrastructure.Data;
using ClaimManagement.Infrastructure.Repositories;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
builder.Services.AddDataProtection();
builder.Services.AddScoped<ISecretProtector, DataProtectionSecretProtector>();
builder.Services.AddScoped<ISettlementService, SettlementService>();
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


var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing. Set it with dotnet user-secrets.");

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidIssuer = jwt["Issuer"],
//            ValidateAudience = true,
//            ValidAudience = jwt["Audience"],
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
//            ValidateLifetime = true
//        };

//        // answers in the same wrapper format as the rest of the API
//        options.Events = new JwtBearerEvents
//        {
//            OnChallenge = async context =>
//            {
//                context.HandleResponse();
//                context.Response.StatusCode = 401;
//                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Login is required. Send a valid token."));
//            },
//            OnForbidden = async context =>
//            {
//                context.Response.StatusCode = 403;
//                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("You do not have permission for this action."));
//            }
//        };
//    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();


var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();