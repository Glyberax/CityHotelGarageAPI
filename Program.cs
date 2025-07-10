using CityHotelGarage.Business.Repository.Data;
using CityHotelGarage.Business.Repository.Models;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Repositories;
using CityHotelGarage.Business.Operations.Interfaces;
using CityHotelGarage.Business.Operations.Services;
using CityHotelGarage.Business.Operations.Mappings;
using CityHotelGarage.Business.Operations.Validators;
using CityHotelGarage.Business.Operations.DTOs;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers ve API Explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration with JWT Support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "City Hotel Garage API",
        Version = "v1",
        Description = "Şehir, Otel, Garaj ve Araba yönetim sistemi - JWT Bearer Token ile güvenlik",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "City Hotel Garage Team",
            Email = "info@cityhotelgarage.com"
        }
    });

    // JWT Bearer Token desteği
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// JWT Configuration - Environment Variables ile
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT__SecretKey") 
                   ?? "MyVerySecureSecretKey2024ForCityHotelGarageAPI!";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT__Issuer") ?? "CityHotelGarageAPI";
var jwtAudience = Environment.GetEnvironmentVariable("JWT__Audience") ?? "CityHotelGarageUsers";
var jwtExpiryMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT__ExpiryMinutes") ?? "60");

// JWT Debug Info
Console.WriteLine($"🔐 JWT Configuration:");
Console.WriteLine($"   Issuer: {jwtIssuer}");
Console.WriteLine($"   Audience: {jwtAudience}");
Console.WriteLine($"   Expiry: {jwtExpiryMinutes} minutes");
Console.WriteLine($"   SecretKey Length: {jwtSecretKey.Length} characters");

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ClockSkew = TimeSpan.Zero
    };

    // Event handlers for debugging
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"🚫 JWT Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("✅ JWT Token validated successfully");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"🔍 JWT Challenge: {context.Error} - {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Manager", "Admin"));
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

// JWT Configuration for Dependency Injection
builder.Services.Configure<JwtConfig>(options =>
{
    options.SecretKey = jwtSecretKey;
    options.Issuer = jwtIssuer;
    options.Audience = jwtAudience;
    options.AccessTokenExpirationMinutes = jwtExpiryMinutes;
    options.RefreshTokenExpirationDays = 30;
});

// AutoMapper Configuration
builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile<AutoMapperProfile>();
    cfg.AllowNullCollections = true;
    cfg.AllowNullDestinationValues = true;
}).CreateMapper());

// Repository Pattern - Dependency Injection
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IGarageRepository, GarageRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Service Pattern - Dependency Injection
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IGarageService, GarageService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Password Hasher for User Authentication
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Entity Framework DbContext - Docker Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
{
   // var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings_Postgresql_DefaultConnection");
   var connectionString = builder.Configuration.GetValue<string>("Postgresql:DefaultConnection");
                          // ?? "Host=postgres;Port=5432;Database=CityHotelGarageDB;Username=postgres;Password=4512";
    
    Console.WriteLine($"🐘 Database Connection: {connectionString.Replace("Password=4512", "Password=***")}");
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(60);
    });
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
    
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    options.EnableServiceProviderCaching();
});

// CORS Configuration
var allowedOrigins = Environment.GetEnvironmentVariable("CORS__AllowedOrigins")?.Split(',')
                    ?? new[] { "http://localhost:3000", "http://localhost:5173", "http://localhost:8080" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// FluentValidation Configuration - Manuel validation için sadece DI
// CREATE Validators
builder.Services.AddScoped<IValidator<CarCreateDto>, CarCreateDtoValidator>();
builder.Services.AddScoped<IValidator<CityCreateDto>, CityCreateDtoValidator>();
builder.Services.AddScoped<IValidator<HotelCreateDto>, HotelCreateDtoValidator>();
builder.Services.AddScoped<IValidator<GarageCreateDto>, GarageCreateDtoValidator>();

// UPDATE Validators
builder.Services.AddScoped<IValidator<CarUpdateDto>, CarUpdateDtoValidator>();
builder.Services.AddScoped<IValidator<CityUpdateDto>, CityUpdateDtoValidator>();
builder.Services.AddScoped<IValidator<HotelUpdateDto>, HotelUpdateDtoValidator>();
builder.Services.AddScoped<IValidator<GarageUpdateDto>, GarageUpdateDtoValidator>();

// AUTH Validators
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordDto>, ChangePasswordDtoValidator>();

// Health Checks
builder.Services.AddHealthChecks();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "City Hotel Garage API v1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });
}

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});

// CORS
app.UseCors("AllowSpecificOrigins");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Health Check endpoint - Database bağlantısı ile birlikte
app.MapGet("/health", async (AppDbContext context) =>
{
    try
    {
        // Database bağlantısını test et
        await context.Database.CanConnectAsync();
        return Results.Ok(new { 
            status = "Healthy", 
            timestamp = DateTime.UtcNow,
            database = "Connected",
            environment = app.Environment.EnvironmentName,
            version = "1.0.0"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: $"Database connection failed: {ex.Message}",
            title : "Health Check Failed",
            statusCode : 503
        );
    }
}).WithTags("Health");

// Controllers
app.MapControllers();

// Database Migration and Seeding - Enhanced with Demo Data
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        Console.WriteLine("🔄 Checking database connection...");
        await context.Database.EnsureCreatedAsync();
        
        Console.WriteLine("🚀 Database is ready!");
        
        // Demo verileri ekle
        await SeedData(context);
        
        Console.WriteLine("🎯 Database ready with demo data!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database setup error: {ex.Message}");
        throw;
    }
}

Console.WriteLine($"Application starting...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"Listening on: {Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://localhost:5010"}");
Console.WriteLine($"Swagger UI: http://localhost:5010/swagger");
Console.WriteLine($"Health Check: http://localhost:5010/health");

app.Run();

// Demo veri ekleme metodu - Enhanced
static async Task SeedData(AppDbContext context)
{
    if (context.Cities.Any()) 
    {
        Console.WriteLine("Demo veriler zaten mevcut, seeding atlanıyor.");
        return;
    }

    Console.WriteLine("🌱 Demo verileri ekleniyor...");

    try
    {
        // Şehirler
        var istanbul = new City { Name = "İstanbul", Population = 15500000, CreatedDate = DateTime.UtcNow };
        var ankara = new City { Name = "Ankara", Population = 5500000, CreatedDate = DateTime.UtcNow };
        var izmir = new City { Name = "İzmir", Population = 4500000, CreatedDate = DateTime.UtcNow };
        
        context.Cities.AddRange(istanbul, ankara, izmir);
        await context.SaveChangesAsync();
        Console.WriteLine("   ✅ Şehirler eklendi");

        // Oteller
        var hotel1 = new Hotel { Name = "Grand Hotel", Yildiz = 5, CityId = istanbul.Id, CreatedDate = DateTime.UtcNow };
        var hotel2 = new Hotel { Name = "City Hotel", Yildiz = 4, CityId = istanbul.Id, CreatedDate = DateTime.UtcNow };
        var hotel3 = new Hotel { Name = "Ankara Palace", Yildiz = 4, CityId = ankara.Id, CreatedDate = DateTime.UtcNow };
        var hotel4 = new Hotel { Name = "İzmir Resort", Yildiz = 3, CityId = izmir.Id, CreatedDate = DateTime.UtcNow };
        
        context.Hotels.AddRange(hotel1, hotel2, hotel3, hotel4);
        await context.SaveChangesAsync();
        Console.WriteLine("   ✅ Oteller eklendi");

        // Garajlar
        var garage1 = new Garage { Name = "Ana Garaj", Capacity = 50, HotelId = hotel1.Id, CreatedDate = DateTime.UtcNow };
        var garage2 = new Garage { Name = "Yan Garaj", Capacity = 30, HotelId = hotel1.Id, CreatedDate = DateTime.UtcNow };
        var garage3 = new Garage { Name = "VIP Garaj", Capacity = 20, HotelId = hotel2.Id, CreatedDate = DateTime.UtcNow };
        var garage4 = new Garage { Name = "Otopark A", Capacity = 40, HotelId = hotel3.Id, CreatedDate = DateTime.UtcNow };
        var garage5 = new Garage { Name = "Açık Alan", Capacity = 60, HotelId = hotel4.Id, CreatedDate = DateTime.UtcNow };
        
        context.Garages.AddRange(garage1, garage2, garage3, garage4, garage5);
        await context.SaveChangesAsync();
        Console.WriteLine("   ✅ Garajlar eklendi");

        // Arabalar
        var cars = new[]
        {
            new Car { Brand = "BMW", LicensePlate = "34ABC123", OwnerName = "Ahmet Yılmaz", GarageId = garage1.Id, EntryTime = DateTime.UtcNow },
            new Car { Brand = "Mercedes", LicensePlate = "34DEF456", OwnerName = "Ayşe Kaya", GarageId = garage1.Id, EntryTime = DateTime.UtcNow },
            new Car { Brand = "Audi", LicensePlate = "06GHI789", OwnerName = "Mehmet Demir", GarageId = garage3.Id, EntryTime = DateTime.UtcNow },
            new Car { Brand = "Volkswagen", LicensePlate = "35JKL012", OwnerName = "Fatma Özkan", GarageId = garage4.Id, EntryTime = DateTime.UtcNow },
            new Car { Brand = "Toyota", LicensePlate = "35MNO345", OwnerName = "Ali Şahin", GarageId = garage5.Id, EntryTime = DateTime.UtcNow },
            new Car { Brand = "Honda", LicensePlate = "06PQR678", OwnerName = "Zeynep Akın", GarageId = garage2.Id, EntryTime = DateTime.UtcNow }
        };
        
        context.Cars.AddRange(cars);
        await context.SaveChangesAsync();
        Console.WriteLine("Arabalar eklendi");

        Console.WriteLine("Demo verileri başarıyla eklendi!");
        Console.WriteLine($"{context.Cities.Count()} şehir");
        Console.WriteLine($"{context.Hotels.Count()} otel");
        Console.WriteLine($"{context.Garages.Count()} garaj");
        Console.WriteLine($"{context.Cars.Count()} araba");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Demo veri ekleme hatası: {ex.Message}");
        throw;
    }
}

// JwtConfig class for dependency injection
public class JwtConfig
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 30;
}