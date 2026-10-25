using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using QuoteGeneration.Api.Middleware;
using QuoteGeneration.Api.Middleware;
using QuoteGeneration.Application.Interfaces;
using QuoteGeneration.Application.QuoteGeneration.Mapper;
using QuoteGeneration.Application.QuoteGeneration.Mapper;
using QuoteGeneration.Infrastructure.Config;
using QuoteGeneration.Infrastructure.Config;
using QuoteGeneration.Infrastructure.Data;
using QuoteGeneration.Infrastructure.Services;
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("myapp", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// Register controllers
builder.Services.AddControllers();
// Authorization policies
builder.Services.AddAuthorization();

// Swagger with JWT Bearer support
builder.Services.AddSwaggerGen(options =>
{
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


// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")));

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(QuoteMappingProfile).Assembly
);

builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IPremiumCalculationService, PremiumCalculationService>();
builder.Services.AddScoped<IRiskAssessmentService, RiskAssessmentService>();
builder.Services.AddScoped<IQuoteRiderService, QuoteRiderService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IQuoteHistoryService, QuoteHistoryService>();

builder.Services.AddHttpClient<InsuranceProductClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ServiceUrls:ProductManagement"]!);
});


builder.Services.AddHttpClient<UnderwritingClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ServiceUrls:underwritingAddress"]
        ?? throw new InvalidOperationException("Underwriting URL is missing."));
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("myapp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();