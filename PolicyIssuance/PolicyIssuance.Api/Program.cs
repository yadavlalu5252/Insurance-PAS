
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PolicyIssuance.Api.Middleware;
using PolicyIssuance.Application.Config;
using PolicyIssuance.Application.Interface;
using PolicyIssuance.Infrastructure.Clients;
using PolicyIssuance.Infrastructure.Data;
using PolicyIssuance.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "PolicyIssuance.Api",
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
            Description = "Enter your JWT access token."
        });

    options.AddSecurityRequirement(document =>
        new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [
                new Microsoft.OpenApi.OpenApiSecuritySchemeReference(
                    "Bearer", document)
            ] = new List<string>()
        });
});

// Database
var connectionString =
    builder.Configuration.GetConnectionString("dbconn");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string 'dbconn' is missing.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// JWT settings must match Authentication API
var jwtKey = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT Key is missing. Configure Jwt:Key in PolicyIssuance.Api.");
}

if (string.IsNullOrWhiteSpace(issuer))
{
    throw new InvalidOperationException(
        "JWT Issuer is missing. Configure Jwt:Issuer.");
}

if (string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException(
        "JWT Audience is missing. Configure Jwt:Audience.");
}

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
                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };
    });

builder.Services.AddAuthorization();

// AutoMapper
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(MapperConfig).Assembly);

// Policy services
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IPolicyDocumentService, PolicyDocumentService>();
builder.Services.AddScoped<IPolicyEndorsementService, PolicyEndorsementService>();
builder.Services.AddScoped<IPolicyClosureService, PolicyClosureService>();

// Interservice clients
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
