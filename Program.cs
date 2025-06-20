using CityHotelGarageAPI.Repository.Data;
using CityHotelGarageAPI.Repository.Models;
using CityHotelGarageAPI.Repository.Interfaces;
using CityHotelGarageAPI.Repository.Repositories;
using CityHotelGarageAPI.Operations.Interfaces;
using CityHotelGarageAPI.Operations.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repository Pattern - Dependency Injection
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IGarageRepository, GarageRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();

// Service Pattern - Dependency Injection
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICarService, CarService>();

// Entity Framework DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=CityHotelGarageDB;Username=postgres;Password=4512"));

// CORS ekle (frontend bağlantısı geliştirilmesi için)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();
    await SeedData(context);
}

app.Run();

// Demo veri ekleme metodu
static async Task SeedData(AppDbContext context)
{
    if (context.Cities.Any()) return;

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