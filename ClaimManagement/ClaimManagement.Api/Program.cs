using Asp.Versioning;
using ClaimManagement.Api.Middleware;
using ClaimManagement.Api.Security;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Mapper;
using ClaimManagement.Application.Wrapper;
using ClaimManagement.Infrastructure.Config;
using ClaimManagement.Infrastructure.Data;
using ClaimManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers (bad request bodies also return the wrapper format)
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

// Database (connection string comes from user-secrets)
builder.Services.AddDbContext<ClaimDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn"),
        sql => sql.EnableRetryOnFailure(3)));   // Azure SQL drops connections now and then

// File storage
var filePath = builder.Configuration["FileStorage:RootPath"]
    ?? Path.Combine(builder.Environment.ContentRootPath, "ClaimFiles");
builder.Services.AddScoped<IFileStorageService>(sp => new LocalFileStorageService(filePath));

// Other teams' services (Policy and Premium), called over HTTP
// If your earlier file already had these two blocks with other key names, keep THOSE names and values.
builder.Services.AddHttpClient<PolicyClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["policyAddress"] ?? "http://localhost:5002");
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHttpClient<PremiumClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["premiumAddress"] ?? "http://localhost:5003");
    client.Timeout = TimeSpan.FromSeconds(5);
});

// Services
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IClaimDocumentService, ClaimDocumentService>();
builder.Services.AddScoped<IClaimWorkflowService, ClaimWorkflowService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddScoped<IInvestigationService, InvestigationService>();
builder.Services.AddScoped<ISettlementService, SettlementService>();

// Encryption of the account number and the payment reference
builder.Services.AddDataProtection();
builder.Services.AddScoped<ISecretProtector, DataProtectionSecretProtector>();

// Mapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingData>());

// Swagger, with the Authorize button for the token
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste only the token. Swagger adds the word Bearer."
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

// Authentication: this API only checks tokens, the Authentication service creates them
var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing. Set it with dotnet user-secrets.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true
        };

        // answers in the same wrapper format as the rest of the API
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Login is required. Send a valid token."));
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("You do not have permission for this action."));
            }
        };
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("angular", policy => policy
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Must be FIRST so it can catch errors from everything below
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("angular"); 

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();