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

// FluentValidation Configuration
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Validator'ları kaydetme
builder.Services.AddScoped<IValidator<Car>, CarValidator>();
builder.Services.AddScoped<IValidator<CarCreateDto>, CarCreateDtoValidator>();
builder.Services.AddScoped<IValidator<City>, CityValidator>();
builder.Services.AddScoped<IValidator<CityCreateDto>, CityCreateDtoValidator>();
builder.Services.AddScoped<IValidator<Hotel>, HotelValidator>();
builder.Services.AddScoped<IValidator<HotelCreateDto>, HotelCreateDtoValidator>();
builder.Services.AddScoped<IValidator<Garage>, GarageValidator>();
builder.Services.AddScoped<IValidator<GarageCreateDto>, GarageCreateDtoValidator>();

// AutoMapper Configuration
builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile<AutoMapperProfile>();
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

// Entity Framework DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=CityHotelGarageDB;Username=postgres;Password=4512");
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.LogTo(Console.WriteLine);
    }
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
});

// JSON Serializer ayarları
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null; // PascalCase korur
});

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
    });
    
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Database initialization ve seeding
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Database oluştur
        await context.Database.EnsureCreatedAsync();
        
        // Demo verileri ekle
        await SeedData(context);
        
        Console.WriteLine("Veritabanı başarıyla hazırlandı!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Veritabanı hatası: {ex.Message}");
    }
}

Console.WriteLine("City Hotel Garage API başlatılıyor...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("Swagger UI: http://localhost:5010");
Console.WriteLine("API Base: http://localhost:5010/api");

app.Run();

// Demo veri ekleme metodu
static async Task SeedData(AppDbContext context)
{
    if (context.Cities.Any()) 
    {
        Console.WriteLine("📊 Demo veriler zaten mevcut.");
        return;
    }

    Console.WriteLine("🌱 Demo verileri ekleniyor...");

    // Şehirler
    var istanbul = new City { Name = "İstanbul", Population = 15500000 };
    var ankara = new City { Name = "Ankara", Population = 5500000 };
    context.Cities.AddRange(istanbul, ankara);
    await context.SaveChangesAsync();

    // Oteller
    var hotel1 = new Hotel { Name = "Grand Hotel", Yildiz = 5, CityId = istanbul.Id };
    var hotel2 = new Hotel { Name = "City Hotel", Yildiz = 4, CityId = istanbul.Id };
    var hotel3 = new Hotel { Name = "Ankara Palace", Yildiz = 4, CityId = ankara.Id };
    context.Hotels.AddRange(hotel1, hotel2, hotel3);
    await context.SaveChangesAsync();

    // Garajlar
    var garage1 = new Garage { Name = "Ana Garaj", Capacity = 50, HotelId = hotel1.Id };
    var garage2 = new Garage { Name = "Yan Garaj", Capacity = 30, HotelId = hotel1.Id };
    var garage3 = new Garage { Name = "VIP Garaj", Capacity = 20, HotelId = hotel2.Id };
    context.Garages.AddRange(garage1, garage2, garage3);
    await context.SaveChangesAsync();

    // Arabalar
    var car1 = new Car { Brand = "BMW", LicensePlate = "34ABC123", OwnerName = "Ahmet Yılmaz", GarageId = garage1.Id };
    var car2 = new Car { Brand = "Mercedes", LicensePlate = "34DEF456", OwnerName = "Ayşe Kaya", GarageId = garage1.Id };
    var car3 = new Car { Brand = "Audi", LicensePlate = "06GHI789", OwnerName = "Mehmet Demir", GarageId = garage3.Id };
    context.Cars.AddRange(car1, car2, car3);
    await context.SaveChangesAsync();

    Console.WriteLine("✅ Demo verileri eklendi!");
}