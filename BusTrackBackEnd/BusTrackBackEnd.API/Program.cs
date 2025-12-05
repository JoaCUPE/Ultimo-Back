using BusTrackBackEnd.API.Shared.Infrastructure.Persistence.EFC;
using BusTrackBackEnd.API.Shared.Domain.Repositories;
using BusTrackBackEnd.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using BusTrackBackEnd.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using BusTrackBackEnd.API.IAM.Infrastructure.Tokens.JWT.Configuration;
// --- IAM USINGS ---
using BusTrackBackEnd.API.IAM.Infrastructure.Hashing.BCrypt.Services;
using BusTrackBackEnd.API.IAM.Application.Internal.OutboundServices;
using BusTrackBackEnd.API.IAM.Domain.Repositories;
using BusTrackBackEnd.API.IAM.Infrastructure.Persistence.EFC.Repositories;
using BusTrackBackEnd.API.IAM.Domain.Services;
using BusTrackBackEnd.API.IAM.Application.Internal.CommandServices;
using BusTrackBackEnd.API.IAM.Application.Internal.QueryServices;
using BusTrackBackEnd.API.IAM.Infrastructure.Tokens.JWT.Services;
using BusTrackBackEnd.API.IAM.Domain.Model.Aggregates; // Para IUserRepository si está ahí

// --- ROUTES USINGS ---
using BusTrackBackEnd.API.Routes.Domain.Repositories;
using BusTrackBackEnd.API.Routes.Infrastructure.Persistence.EFC.Repositories;
using BusTrackBackEnd.API.Routes.Domain.Services;
using BusTrackBackEnd.API.Routes.Application.Internal.CommandServices;
using BusTrackBackEnd.API.Routes.Application.Internal.QueryServices;

// --- COMPANIES USINGS ---
using BusTrackBackEnd.API.Companies.Domain.Model.Aggregates; // Para ICompanyRepository
using BusTrackBackEnd.API.Companies.Infrastructure.Persistence.EFC.Repositories;
using BusTrackBackEnd.API.Companies.Domain.Services;
using BusTrackBackEnd.API.Companies.Application.CommandServices;
using BusTrackBackEnd.API.Companies.Application.QueryServices;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using IUserRepository = BusTrackBackEnd.API.IAM.Domain.Repositories.IUserRepository; // Para Swagger

var builder = WebApplication.CreateBuilder(args);

// ====================================================================
// 1. CONFIGURACIÓN DE CONTROLADORES Y RUTAS
// ====================================================================
builder.Services.AddControllers(options => 
{
    options.Conventions.Add(new KebabCaseRouteNamingConvention());
});

// ====================================================================
// 2. CONFIGURACIÓN DE BASE DE DATOS (MYSQL RAILWAY)
// ====================================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString != null)
    {
        // Conexión a MySQL (Railway)
        options.UseMySQL(connectionString)
            .LogTo(s => Console.WriteLine(s), LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }
});/*
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // "BusTrackTestDB" es el nombre de tu base de datos volátil
    options.UseInMemoryDatabase("BusTrackTestDB");
});*/
// ====================================================================
// 3. OPENAPI / SWAGGER
// ====================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BusTrack API",
        Version = "v1",
        Description = "API para gestión de transporte urbano - BusTrack"
    });
});

// ====================================================================
// 4. INYECCIÓN DE DEPENDENCIAS (SERVICIOS Y REPOSITORIOS)
// ====================================================================

// --- Shared Bounded Context ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --- IAM Bounded Context (Usuarios & Auth) ---
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();

// --- Routes Bounded Context (Rutas) ---
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<IRouteCommandService, RouteCommandService>();
builder.Services.AddScoped<IRouteQueryService, RouteQueryService>();

// --- Companies Bounded Context (Empresas) ---
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyCommandService, CompanyCommandService>();
builder.Services.AddScoped<ICompanyQueryService, CompanyQueryService>();


var app = builder.Build();

// ====================================================================
// 5. PIPELINE DE LA APLICACIÓN
// ====================================================================

// Configurar Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();