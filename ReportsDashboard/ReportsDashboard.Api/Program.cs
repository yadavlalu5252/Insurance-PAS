
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ReportsDashboard.Api.Services;
using ReportsDashboard.Application.Interfaces;
using ReportsDashboard.Infrastructure.Config;
using ReportsDashboard.Infrastructure.Data;
using ReportsDashboard.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Report services
builder.Services.AddScoped<ICustomerReport, customerreportservice>();
builder.Services.AddScoped<IPolicyReport, policyreportservice>();
builder.Services.AddScoped<Ipremiumreport, premiumreportservice>();
builder.Services.AddScoped<Iclaimsreport, claimsreportservice>();
builder.Services.AddScoped<Iunderwritingreport, underwritingreportservice>();
builder.Services.AddScoped<Iagentreport, agentreportservice>();
builder.Services.AddScoped<Ifinancialreport, financialreportservice>();
builder.Services.AddScoped<Ischeduledreport, scheduledreportservice>();

// HTTP clients
builder.Services.AddHttpClient<CustomerClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["customerAddress"] ?? "http://localhost:5129");
});

builder.Services.AddHttpClient<PolicyClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["policyAddress"] ?? "http://localhost:5092");
});

builder.Services.AddHttpClient<PremiumClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["premiumAddress"] ?? "http://localhost:5093");
});

builder.Services.AddHttpClient<ClaimsClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["claimsAddress"] ?? "http://localhost:5094");
});

builder.Services.AddHttpClient<UnderwritingClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["underwritingAddress"] ?? "http://localhost:5095");
});

builder.Services.AddHttpClient<AgentClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["agentAddress"] ?? "http://localhost:5096");
});

// JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT signing key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT audience is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.Name
        };
    });

// Admin-only authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("Admin")
        .Build();
});

// Swagger with Bearer token authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ReportsDashboard API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT access token."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

var app = builder.Build();

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
