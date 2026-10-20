using Microsoft.EntityFrameworkCore;
using PolicyIssuance.Api.Middleware;
using PolicyIssuance.Application.Config;
using PolicyIssuance.Application.Interface;
using PolicyIssuance.Infrastructure.Data;
using PolicyIssuance.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")
    ));

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(MapperConfig).Assembly
);

builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IPolicyDocumentService, PolicyDocumentService>();
builder.Services.AddScoped<IPolicyEndorsementService, PolicyEndorsementService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();