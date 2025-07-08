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
using FluentValidation.AspNetCore;
// ✅ JWT Bearer Token için eklenen using'ler
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "City Hotel Garage API",
        Version = "v1",
        Description = "Şehir, Otel, Garaj ve Araba yönetim sistemi - JWT Bearer Token ile güvenlik",
    });

    // ✅ Swagger'da JWT Bearer Token desteği
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
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

// ✅ JWT Configuration - Docker Environment Variables desteği ile
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT__SecretKey") 
                   ?? "CityHotelGarageSecretKey2024!@#VerySecureKey123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT__Issuer") ?? "CityHotelGarageAPI";
var jwtAudience = Environment.GetEnvironmentVariable("JWT__Audience") ?? "CityHotelGarageUsers";
var jwtExpiryMinutes = Environment.GetEnvironmentVariable("JWT__ExpiryMinutes") ?? "10080"; // 7 gün

Console.WriteLine($"🔐 JWT Configuration:");
Console.WriteLine($"   Issuer: {jwtIssuer}");
Console.WriteLine($"   Audience: {jwtAudience}");
Console.WriteLine($"   Expiry: {jwtExpiryMinutes} minutes");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Token süresinde tolerans yok
        };

        // Bearer token events
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ Bearer Token Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"✅ Bearer Token validated for user: {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            }
        };
    });

// ✅ Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Manager", "Admin"));
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

// ✅ JWT Configuration - Docker environment variables ile
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
{
    ["JWT:SecretKey"] = jwtSecretKey,
    ["JWT:Issuer"] = jwtIssuer,
    ["JWT:Audience"] = jwtAudience,
    ["JWT:AccessTokenExpirationMinutes"] = jwtExpiryMinutes,
    ["JWT:RefreshTokenExpirationDays"] = "30"
});

// FluentValidation Configuration - ASYNC Validators
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

// ✅ AUTH Validators - YENİ EKLENENLER
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordDto>, ChangePasswordDtoValidator>();

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
// ✅ User Repository - YENİ EKLENEN
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Service Pattern - Dependency Injection
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IGarageService, GarageService>();
builder.Services.AddScoped<ICarService, CarService>();
// ✅ Auth Service ve JWT Service - YENİ EKLENENLER
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// ✅ Password Hasher for User Authentication
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Entity Framework DbContext - Docker Configuration Enhanced
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // 🐳 Docker ortamında connection string'i environment variable'dan al
    var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                           ?? builder.Configuration.GetConnectionString("DefaultConnection") 
                           ?? "Host=postgres;Port=5432;Database=CityHotelGarageDB;Username=postgres;Password=4512";
    
    Console.WriteLine($"🐘 Database Connection: {connectionString.Replace("Password=4512", "Password=***")}");
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // 🐳 Docker için retry policy
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

// CORS Configuration - Environment variables desteği ile
var allowedOrigins = Environment.GetEnvironmentVariable("CORS__AllowedOrigins")?.Split(',')
                    ?? new[] { "http://localhost:3000", "http://localhost:3001", "https://yourdomain.com" };

Console.WriteLine($"🌐 CORS Allowed Origins: {string.Join(", ", allowedOrigins)}");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    options.AddPolicy("DockerCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// JSON Serializer ayarları
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// Health Checks - Enhanced for Docker
builder.Services.AddHealthChecks()
    .AddCheck("api", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is running"));

var app = builder.Build();

// Configure the HTTP request pipeline - Docker optimized
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "City Hotel Garage API v1");
        c.RoutePrefix = string.Empty;
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.DefaultModelsExpandDepth(-1);
        c.DisplayRequestDuration();
    });
    
    app.UseDeveloperExceptionPage();
    app.UseCors("AllowAll");
}
else
{
    // 🐳 Production'da da Swagger'ı aç (Docker için)
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "City Hotel Garage API v1");
        c.RoutePrefix = "swagger";
    });
    
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseCors("DockerCorsPolicy");
}

app.UseHttpsRedirection();

// ✅ Authentication & Authorization Middleware (SIRASI ÖNEMLİ!)
app.UseAuthentication(); // Bearer token kontrolü
app.UseAuthorization();  // Rol/Policy kontrolü

// 🐳 Enhanced Health check endpoints for Docker
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description,
                duration = x.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.MapControllers();

// 🐳 Database initialization ve seeding - Docker için güçlendirilmiş
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        Console.WriteLine("🐳 Docker ortamında veritabanı bağlantısı kontrol ediliyor...");
        
        // PostgreSQL'in hazır olmasını bekle - Enhanced retry logic
        var maxRetries = 30; // 5 dakika bekle
        var retryCount = 0;
        
        while (retryCount < maxRetries)
        {
            try
            {
                await context.Database.CanConnectAsync();
                Console.WriteLine("✅ PostgreSQL bağlantısı başarılı!");
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"🔄 PostgreSQL bekleniyor... Deneme {retryCount}/{maxRetries} - {ex.Message}");
                
                if (retryCount >= maxRetries)
                {
                    Console.WriteLine("❌ PostgreSQL bağlantısı kurulamadı! Container'lar kontrol edin.");
                    throw;
                }
                
                await Task.Delay(10000); // 10 saniye bekle
            }
        }
        
        // Database oluştur ve migrate et
        await context.Database.EnsureCreatedAsync();
        
        // Demo verileri ekle
        await SeedData(context);
        Console.WriteLine("🎯 Docker ortamında veritabanı hazır!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Docker veritabanı hatası: {ex.Message}");
        
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
        
        // Production'da crash yap
        if (!app.Environment.IsDevelopment())
        {
            throw;
        }
    }
}

// 🐳 Docker startup messages - Enhanced
Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine("🐳 City Hotel Garage API Docker'da başlatılıyor...");
Console.WriteLine($"🔧 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"🌐 API Base: http://localhost:5010/api");
Console.WriteLine($"📊 Swagger UI: http://localhost:5010/swagger");
Console.WriteLine($"❤️  Health Check: http://localhost:5010/health");
Console.WriteLine($"🐘 PostgreSQL: postgres:5432 (External: localhost:5433)");
Console.WriteLine($"🔐 JWT Bearer Token: Enabled ({jwtExpiryMinutes} minutes expiry)");
Console.WriteLine("👤 Auth Endpoints:");
Console.WriteLine("   - POST /api/Auth/register");
Console.WriteLine("   - POST /api/Auth/login");
Console.WriteLine("   - POST /api/Auth/refresh-token");
Console.WriteLine("   - POST /api/Auth/logout [Auth Required]");
Console.WriteLine("   - GET  /api/Auth/profile [Auth Required]");
Console.WriteLine("📋 Demo Accounts:");
Console.WriteLine("   👤 admin / Admin123! (Admin)");
Console.WriteLine("   👤 manager / Manager123! (Manager)");
Console.WriteLine("   👤 testuser / User123! (User)");
Console.WriteLine("=".PadRight(60, '='));

app.Run();

// Demo veri ekleme metodu - aynı kalacak
static async Task SeedData(AppDbContext context)
{
    if (context.Cities.Any()) 
    {
        Console.WriteLine("📊 Demo veriler zaten mevcut, seeding atlanıyor.");
        return;
    }

    Console.WriteLine("🌱 Demo verileri ekleniyor...");

    try
    {
        // Şehirler
        var istanbul = new City { Name = "İstanbul", Population = 15500000 };
        var ankara = new City { Name = "Ankara", Population = 5500000 };
        var izmir = new City { Name = "İzmir", Population = 4500000 };
        
        context.Cities.AddRange(istanbul, ankara, izmir);
        await context.SaveChangesAsync();
        Console.WriteLine("   ✅ Şehirler eklendi");

        // Oteller
        var hotel1 = new Hotel { Name = "Grand Hotel", Yildiz = 5, CityId = istanbul.Id };
        var hotel2 = new Hotel { Name = "City Hotel", Yildiz = 4, CityId = istanbul.Id };
        var hotel3 = new Hotel { Name = "Ankara Palace", Yildiz = 4, CityId = ankara.Id };
        var hotel4 = new Hotel { Name = "İzmir Resort", Yildiz = 3, CityId = izmir.Id };
        
        context.Hotels.AddRange(hotel1, hotel2, hotel3, hotel4);
        await context.SaveChangesAsync();
        Console.WriteLine("   ✅ Oteller eklendi");

        // Garajlar
        var garage1 = new Garage { Name = "Ana Garaj", Capacity = 50, HotelId = hotel1.Id };
        var garage2 = new Garage { Name = "Yan Garaj", Capacity = 30, HotelId = hotel1.Id };
        var garage3 = new Garage { Name = "VIP Garaj", Capacity = 20, HotelId = hotel2.Id };
        var garage4 = new Garage { Name = "Otopark A", Capacity = 40, HotelId = hotel3.Id };
        var garage5 = new Garage { Name = "Açık Alan", Capacity = 60, HotelId = hotel4.Id };
        
        context.Garages.AddRange(garage1, garage2, garage3, garage4, garage5);
        await context.SaveChangesAsync();
        Console.WriteLine("   ✅ Garajlar eklendi");

        // Arabalar
        var cars = new[]
        {
            new Car { Brand = "BMW", LicensePlate = "34ABC123", OwnerName = "Ahmet Yılmaz", GarageId = garage1.Id },
            new Car { Brand = "Mercedes", LicensePlate = "34DEF456", OwnerName = "Ayşe Kaya", GarageId = garage1.Id },
            new Car { Brand = "Audi", LicensePlate = "06GHI789", OwnerName = "Mehmet Demir", GarageId = garage3.Id },
            new Car { Brand = "Volkswagen", LicensePlate = "35JKL012", OwnerName = "Fatma Özkan", GarageId = garage4.Id },
            new Car { Brand = "Toyota", LicensePlate = "35MNO345", OwnerName = "Ali Şahin", GarageId = garage5.Id },
            new Car { Brand = "Honda", LicensePlate = "06PQR678", OwnerName = "Zeynep Akın", GarageId = garage2.Id }
        };
        
        context.Cars.AddRange(cars);
        await context.SaveChangesAsync();
        Console.WriteLine("   ✅ Arabalar eklendi");

        // ✅ Demo kullanıcılar ekleme - YENİ EKLENEN
        if (!context.Users.Any())
        {
            Console.WriteLine("👤 Demo kullanıcılar ekleniyor...");
            
            var passwordHasher = new PasswordHasher<User>();
            
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@cityhotelgarage.com",
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");
            
            var normalUser = new User
            {
                Username = "testuser",
                Email = "user@cityhotelgarage.com",
                FirstName = "Test",
                LastName = "User",
                Role = "User",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            normalUser.PasswordHash = passwordHasher.HashPassword(normalUser, "User123!");

            var managerUser = new User
            {
                Username = "manager",
                Email = "manager@cityhotelgarage.com",
                FirstName = "Manager",
                LastName = "User", 
                Role = "Manager",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            managerUser.PasswordHash = passwordHasher.HashPassword(managerUser, "Manager123!");
            
            context.Users.AddRange(adminUser, normalUser, managerUser);
            await context.SaveChangesAsync();
            Console.WriteLine("   ✅ Demo kullanıcılar eklendi");
        }

        Console.WriteLine("🎉 Demo verileri başarıyla eklendi!");
        Console.WriteLine($"   📊 {context.Cities.Count()} şehir");
        Console.WriteLine($"   🏨 {context.Hotels.Count()} otel");
        Console.WriteLine($"   🅿️  {context.Garages.Count()} garaj");
        Console.WriteLine($"   🚗 {context.Cars.Count()} araba");
        Console.WriteLine($"   👥 {context.Users.Count()} kullanıcı");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Demo veri ekleme hatası: {ex.Message}");
        throw;
    }
}