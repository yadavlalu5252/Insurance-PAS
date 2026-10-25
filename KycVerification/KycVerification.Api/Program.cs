using KycVerification.Application.Interface;
using KycVerification.Application.Mapper;
using KycVerification.Infrastructure.Data;
using KycVerification.Infrastructure.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

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
            options.TokenValidationParameters = new TokenValidationParameters
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

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer",
                document)] = []
        });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")));

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(KycMappingProfile));

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();