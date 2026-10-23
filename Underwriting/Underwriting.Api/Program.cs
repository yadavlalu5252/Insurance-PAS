using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Underwriting.Application.Interfaces;
using Underwriting.Infrastructure.Config;
using Underwriting.Infrastructure.Data;
using Underwriting.Infrastructure.Mapper;
using Underwriting.Infrastructure.Repositories;
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));
builder.Services.AddAutoMapper(cfg => { }, typeof(UnderwritingMappingProfile));
builder.Services.AddHttpClient<CustomerRegistrationClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["customerAddress"]
        ?? throw new InvalidOperationException(
            "Customer Registration URL is missing."));
});
builder.Services.AddScoped<Iunderwriting, underwritingService>();
builder.Services.AddScoped<Isavemedical, savemedicalServices>();
builder.Services.AddScoped<Isavefinancial, savefinancialservice>();
builder.Services.AddScoped< IRiskScoringService,RiskScoringService>();

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