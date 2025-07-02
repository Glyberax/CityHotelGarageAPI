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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
   
    });


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

// AutoMapper Configuration - Optimized
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

// Service Pattern - Dependency Injection
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IGarageService, GarageService>();
builder.Services.AddScoped<ICarService, CarService>();

// Entity Framework DbContext - Enhanced Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=CityHotelGarageDB;Username=postgres;Password=4512");
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
    
    // Performance optimizations
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    options.EnableServiceProviderCaching();
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

// CORS Configuration - Frontend bağlantısı için
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    // Production için daha güvenli CORS policy
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// JSON Serializer ayarları - Enhanced
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null; // PascalCase korur
    options.SerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "Production");
app.UseAuthorization();

// Health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

// Database initialization ve seeding - Enhanced Error Handling
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        Console.WriteLine("Veritabanı bağlantısı kontrol ediliyor...");
        
        // Database oluştur ve migrate et
        await context.Database.EnsureCreatedAsync();
        
        Console.WriteLine("Veritabanı bağlantısı başarılı!");
        
        // Demo verileri ekle
        await SeedData(context);
        
        Console.WriteLine("Veritabanı hazır!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Veritabanı hatası: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        
        // Development'ta hatayı göster, production'da logla
        if (app.Environment.IsDevelopment())
        {
            throw; // Development'ta crash yap
        }
        else
        {
            // Production'da loglama servisi kullanılabilir
            Console.WriteLine("Production modunda çalışmaya devam ediliyor...");
        }
    }
}

// Application startup messages
Console.WriteLine("City Hotel Garage API başlatılıyor...");
Console.WriteLine("   - GET  /api/Cities");
Console.WriteLine("   - GET  /api/Hotels");
Console.WriteLine("   - GET  /api/Garages");
Console.WriteLine("   - GET  /api/Cars");
Console.WriteLine("   - POST /api/Cars (Create new car)");
Console.WriteLine("   - PUT  /api/Cars/{id} (Update car)");
Console.WriteLine("   - DELETE /api/Cars/{id} (Remove car)");

app.Run();

// Demo veri ekleme metodu
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

        
        context.Garages.AddRange(garage1, garage2, garage3);
        await context.SaveChangesAsync();
        Console.WriteLine("   ✅ Garajlar eklendi");

        // Arabalar
        var cars = new[]
        {
            new Car { Brand = "BMW", LicensePlate = "34ABC123", OwnerName = "Ahmet Yılmaz", GarageId = garage1.Id },
            new Car { Brand = "Mercedes", LicensePlate = "34DEF456", OwnerName = "Ayşe Kaya", GarageId = garage1.Id },
            new Car { Brand = "Audi", LicensePlate = "06GHI789", OwnerName = "Mehmet Demir", GarageId = garage3.Id }
        };
        
        context.Cars.AddRange(cars);
        await context.SaveChangesAsync();
        Console.WriteLine("   ✅ Arabalar eklendi");

        Console.WriteLine("🎉 Demo verileri başarıyla eklendi!");
        Console.WriteLine($"   📊 {context.Cities.Count()} şehir");
        Console.WriteLine($"   🏨 {context.Hotels.Count()} otel");
        Console.WriteLine($"   🅿️  {context.Garages.Count()} garaj");
        Console.WriteLine($"   🚗 {context.Cars.Count()} araba");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Demo veri ekleme hatası: {ex.Message}");
        throw;
    }
}