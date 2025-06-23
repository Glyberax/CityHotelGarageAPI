using CityHotelGarageAPI.Repository.Data;
using CityHotelGarageAPI.Repository.Models;
using CityHotelGarageAPI.Repository.Interfaces;
using CityHotelGarageAPI.Repository.Repositories;
using CityHotelGarageAPI.Operations.Interfaces;
using CityHotelGarageAPI.Operations.Services;
using CityHotelGarageAPI.Operations.Mappings;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
});

// AutoMapper Configuration - Namespace conflict çözümü ile
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
    
    // Development ortamında SQL loglarını göster
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
        c.RoutePrefix = string.Empty; // Swagger'ı root'ta çalıştır
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
        
        Console.WriteLine("🗄️ Veritabanı başarıyla hazırlandı!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Veritabanı hatası: {ex.Message}");
    }
}

// Uygulama başlatma mesajı
Console.WriteLine("🚀 City Hotel Garage API başlatılıyor...");
Console.WriteLine($"🌍 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("📚 Swagger UI: http://localhost:5010");
Console.WriteLine("🔗 API Base: http://localhost:5010/api");

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
    var cities = new[]
    {
        new City { Name = "İstanbul", Population = 15500000 },
        new City { Name = "Ankara", Population = 5500000 },
        new City { Name = "İzmir", Population = 4400000 },
        new City { Name = "Bursa", Population = 3100000 },
        new City { Name = "Antalya", Population = 2600000 }
    };
    context.Cities.AddRange(cities);
    await context.SaveChangesAsync();
    Console.WriteLine($"✅ {cities.Length} şehir eklendi");

    // Oteller
    var hotels = new[]
    {
        new Hotel { Name = "Grand Hotel İstanbul", Yildiz = 5, CityId = cities[0].Id },
        new Hotel { Name = "City Hotel İstanbul", Yildiz = 4, CityId = cities[0].Id },
        new Hotel { Name = "Bosphorus Palace", Yildiz = 5, CityId = cities[0].Id },
        new Hotel { Name = "Ankara Palace", Yildiz = 4, CityId = cities[1].Id },
        new Hotel { Name = "Capital Hotel", Yildiz = 3, CityId = cities[1].Id },
        new Hotel { Name = "İzmir Resort", Yildiz = 5, CityId = cities[2].Id },
        new Hotel { Name = "Aegean Hotel", Yildiz = 4, CityId = cities[2].Id },
        new Hotel { Name = "Bursa Thermal", Yildiz = 4, CityId = cities[3].Id },
        new Hotel { Name = "Antalya Beach Resort", Yildiz = 5, CityId = cities[4].Id }
    };
    context.Hotels.AddRange(hotels);
    await context.SaveChangesAsync();
    Console.WriteLine($"✅ {hotels.Length} otel eklendi");

    // Garajlar
    var garages = new[]
    {
        new Garage { Name = "Ana Garaj", Capacity = 50, HotelId = hotels[0].Id },
        new Garage { Name = "Yan Garaj", Capacity = 30, HotelId = hotels[0].Id },
        new Garage { Name = "VIP Garaj", Capacity = 20, HotelId = hotels[1].Id },
        new Garage { Name = "Kapalı Garaj", Capacity = 25, HotelId = hotels[2].Id },
        new Garage { Name = "Merkez Garaj", Capacity = 40, HotelId = hotels[3].Id },
        new Garage { Name = "Güvenli Garaj", Capacity = 35, HotelId = hotels[4].Id },
        new Garage { Name = "Deniz Manzaralı Garaj", Capacity = 45, HotelId = hotels[5].Id },
        new Garage { Name = "Liman Garajı", Capacity = 30, HotelId = hotels[6].Id },
        new Garage { Name = "Termal Garaj", Capacity = 20, HotelId = hotels[7].Id },
        new Garage { Name = "Plaj Garajı", Capacity = 60, HotelId = hotels[8].Id }
    };
    context.Garages.AddRange(garages);
    await context.SaveChangesAsync();
    Console.WriteLine($"✅ {garages.Length} garaj eklendi");

    // Arabalar
    var cars = new[]
    {
        new Car { Brand = "BMW", LicensePlate = "34ABC123", OwnerName = "Ahmet Yılmaz", GarageId = garages[0].Id },
        new Car { Brand = "Mercedes", LicensePlate = "34DEF456", OwnerName = "Ayşe Kaya", GarageId = garages[0].Id },
        new Car { Brand = "Audi", LicensePlate = "06GHI789", OwnerName = "Mehmet Demir", GarageId = garages[2].Id },
        new Car { Brand = "Toyota", LicensePlate = "34JKL012", OwnerName = "Fatma Özkan", GarageId = garages[1].Id },
        new Car { Brand = "Volkswagen", LicensePlate = "06MNO345", OwnerName = "Ali Çelik", GarageId = garages[4].Id },
        new Car { Brand = "Ford", LicensePlate = "35PQR678", OwnerName = "Zeynep Aktaş", GarageId = garages[6].Id },
        new Car { Brand = "Renault", LicensePlate = "16STU901", OwnerName = "Hasan Kara", GarageId = garages[7].Id },
        new Car { Brand = "Peugeot", LicensePlate = "07VWX234", OwnerName = "Elif Yıldız", GarageId = garages[8].Id },
        new Car { Brand = "Honda", LicensePlate = "34YZA567", OwnerName = "Murat Şen", GarageId = garages[3].Id },
        new Car { Brand = "Hyundai", LicensePlate = "35BCD890", OwnerName = "Selin Özkan", GarageId = garages[9].Id }
    };
    context.Cars.AddRange(cars);
    await context.SaveChangesAsync();
    Console.WriteLine($"✅ {cars.Length} araba eklendi");

    Console.WriteLine("🎉 Demo verileri başarıyla eklendi!");
    Console.WriteLine($"📊 Toplam: {context.Cities.Count()} şehir, {context.Hotels.Count()} otel, {context.Garages.Count()} garaj, {context.Cars.Count()} araba");
}