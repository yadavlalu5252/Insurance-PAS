using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PolicyServicing.Application.Interface;
using PolicyServicing.Infrastructure.Config;
using PolicyServicing.Infrastructure.Data;
using PolicyServicing.Infrastructure.Repository;
using PolicyServicing.Infrastructure.Repository.PolicyServicing.Infrastructure.Repository;
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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

        // 🚨 ADD THIS LINE:
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));

builder.Services.AddHttpContextAccessor();

var policyUrl = builder.Configuration["ServiceUrls:PolicyService"];

builder.Services.AddHttpClient<PolicyClient>(client =>
{
    client.BaseAddress = new Uri(policyUrl);
});

var customerUrl = builder.Configuration["ServiceUrls:CustomerService"];

builder.Services.AddHttpClient<CustomerClient>(client =>
{
    client.BaseAddress = new Uri(customerUrl);
});

var premiumUrl = builder.Configuration["ServiceUrls:PremiumService"];
var claimsUrl = builder.Configuration["ServiceUrls:ClaimsService"];
var productUrl = builder.Configuration["ServiceUrls:ProductService"];

builder.Services.AddHttpClient<PremiumClient>(client =>
{
    client.BaseAddress = new Uri(premiumUrl);
});

builder.Services.AddHttpClient<ClaimsClient>(client =>
{
    client.BaseAddress = new Uri(claimsUrl);
});

builder.Services.AddHttpClient<ProductClient>(client =>
{
    client.BaseAddress = new Uri(productUrl);
});


builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);


builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IProcessCancellation, ProcessCancellationService>();
builder.Services.AddScoped<IEndorsementService, EndorsementService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IProcessRiderModify, ProcessRiderModify>();
builder.Services.AddScoped<IPolicyRenew, PolicyRenewalService>();
builder.Services.AddScoped<IProcessCoverage, PolicyCoverageService>();
builder.Services.AddScoped<IProcessNominee,ProcessNomineeService>();
builder.Services.AddScoped<IUpdateContact, UpdateContactService>();


builder.Services.AddMemoryCache();
builder.Services.AddScoped<OtpService>();




// Swagger configuration
builder.Services.AddEndpointsApiExplorer();

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


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); 
    });
});


var app = builder.Build();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
