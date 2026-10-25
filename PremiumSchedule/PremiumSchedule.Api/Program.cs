using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PremiumSchedule.Api.Middleware;
using PremiumSchedule.Application.Interface;
using PremiumSchedule.Application.Interfaces;
using PremiumSchedule.Application.Mapper;
using PremiumSchedule.Infrastructure.Data;
using PremiumSchedule.Infrastructure.Repositories;
using QuestPDF.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Evaluation;

// Add services to the container.
builder.Services.AddControllers();

// Swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new Microsoft.OpenApi.OpenApiInfo
        {
            Title = "Premium Schedule API",
            Version = "v1"
        });

    options.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.ParameterLocation.Header,
            Description =
                "Enter your JWT access token."
        });

    options.AddSecurityRequirement(document =>
        new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [
                new Microsoft.OpenApi.OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = new List<string>()
        });
});

// OpenAPI
builder.Services.AddOpenApi();


// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")
    ));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Premium Schedule
builder.Services.AddScoped<IPremiumSchedule, PremiumScheduleService>();


// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(PremiumScheduleMapper).Assembly);
});


// Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("dbconn")));

builder.Services.AddHangfireServer();


// Policy Issuance Interservice Communication
builder.Services.AddHttpClient<IPolicyIssuanceClient, PolicyIssuanceClient>(
    client =>
    {
        client.BaseAddress = new Uri(
            "https://localhost:7129/");
    });

builder.Services.AddHttpClient<IQuoteClient, QuoteClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7129/api/Quote/");
});

//builder.Services.AddHttpClient<
//    IPremiumFrequencyClient,
//    PremiumFrequencyClient
//>(client =>
//{
//    client.BaseAddress = new Uri(
//        "https://localhost:YYYY/api/PremiumFrequency/"
//    );
//});


// Premium Payment
builder.Services.AddScoped<IPremiumPaymentService, PremiumPaymentService>();


// Refund
builder.Services.AddScoped<IRefundService, RefundService>();


// ============================================================
// JWT AUTHENTICATION
// ============================================================

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is missing.");

var issuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is missing.");

var audience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience is missing.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT Key must be at least 32 bytes.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // Validate Issuer
                ValidateIssuer = true,
                ValidIssuer = issuer,

                // Validate Audience
                ValidateAudience = true,
                ValidAudience = audience,

                // Validate JWT Signature
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                // Validate Token Expiration
                ValidateLifetime = true,

                // Allow 30 seconds clock difference
                ClockSkew = TimeSpan.FromSeconds(30)
            };
    });


// Authorization
builder.Services.AddAuthorization();


var app = builder.Build();


// ============================================================
// HANGFIRE
// ============================================================

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// Daily overdue schedule update
RecurringJob.AddOrUpdate<IPremiumSchedule>(
    "update-overdue-premium-schedules",
    service => service.UpdateOverdueSchedules(
        10,
        500),
    Cron.Daily);


// Global Exception Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();


// ============================================================
// HTTP REQUEST PIPELINE
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseCors("AngularPolicy");

app.UseHttpsRedirection();


// ============================================================
// AUTHENTICATION & AUTHORIZATION
// ============================================================

// Authenticate JWT
app.UseAuthentication();

// Check [Authorize] / [Authorize(Roles = "...")]
app.UseAuthorization();


// Controllers
app.MapControllers();

app.Run();