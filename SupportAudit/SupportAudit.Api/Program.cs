using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SupportAudit.Api.Infrastructure;
using SupportAudit.Application.Interfaces;
using SupportAudit.Infrastructure.Data;
using SupportAudit.Infrastructure.Repository;
using System.Text;

static void ConfigureJwtAuthentication(
    IServiceCollection services,
    IConfiguration configuration)
{
    var jwtKey = configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("JWT key is missing.");

    var issuer = configuration["Jwt:Issuer"]
        ?? throw new InvalidOperationException("JWT issuer is missing.");

    var audience = configuration["Jwt:Audience"]
        ?? throw new InvalidOperationException("JWT audience is missing.");

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                    ClockSkew = TimeSpan.Zero
                };
        });

    services.AddAuthorization();
}


var builder = WebApplication.CreateBuilder(args);

ConfigureJwtAuthentication(
    builder.Services,
    builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<RequestUserContext>();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));

// Dependency Injection
builder.Services.AddScoped<ISupportService, SupportService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<RequestUserContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
