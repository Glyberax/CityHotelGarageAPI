# 🏨 City Hotel Garage API

.NET 8 Web API projesi - Şehir, Otel, Garaj ve Araba yönetim sistemi. Clean Architecture ve modern .NET ile geliştirilmiş profesyonel bir API.

## 📋 İçindekiler

- [Özellikler](#-özellikler)
- [Teknolojiler](#-teknolojiler)
- [Kurulum](#-kurulum)
- [Business Layer](#-business-layer)
- [API Endpoints](#-api-endpoints)
- [Mimari](#-mimari)
- [Konfigürasyon](#-konfigürasyon)
- [Kullanım](#-kullanım)

## 🚀 Özellikler

- **Clean Architecture** - Repository ve Service Pattern implementasyonu
- **FluentValidation** - Kapsamlı veri doğrulama
- **AutoMapper** - DTO mapping işlemleri
- **Entity Framework Core** - PostgreSQL ile ORM
- **RESTful API** - Standardize edilmiş endpoint'ler
- **Swagger UI** - Otomatik API dokumentasyonu
- **CRUD Operasyonları** - Tüm varlıklar için tam CRUD desteği
- **Error Handling** - Merkezi hata yönetimi
- **Validation** - Client ve server-side doğrulama

## 🛠️ Teknolojiler

| Kategori | Teknoloji |
|----------|-----------|
| **Framework** | .NET 8 Web API |
| **Veritabanı** | PostgreSQL |
| **ORM** | Entity Framework Core 9.0 |
| **Validation** | FluentValidation |
| **Mapping** | AutoMapper |
| **Documentation** | Swagger/OpenAPI |
| **Architecture** | Clean Architecture, Repository Pattern |

## 📦 Kurulum

### Gereksinimler
- .NET 8 SDK
- PostgreSQL 12+
- Visual Studio 2022 / VS Code / Rider

### 1. Repository'yi Klonlayın
```bash
git clone https://github.com/Glyberax/CityHotelGarageAPI.git
cd CityHotelGarageAPI
```

### 2. Business Layer'ı Ekleyin
Bu proje, business logic için ayrı bir Class Library kullanır:

```bash
# Git submodule olarak business layer'ı ekleyin
git submodule add https://github.com/Glyberax/CityHotelGarage.Business.git CityHotelGarage.Business
git submodule init
git submodule update
```

**Alternatif olarak:** Business repository'sini ayrı klonlayabilirsiniz:
```bash
git clone https://github.com/Glyberax/CityHotelGarage.Business.git
```

### 3. NuGet Paketlerini Yükleyin
```bash
dotnet restore
```

### 4. Veritabanı Bağlantısını Yapılandırın
`Program.cs` dosyasında PostgreSQL connection string'ini güncelleyin:
```csharp
options.UseNpgsql("Host=localhost;Port=5432;Database=CityHotelGarageDB;Username=postgres;Password=YOUR_PASSWORD");
```

### 5. Uygulamayı Çalıştırın
```bash
dotnet run
```

API şu adreste çalışacaktır: `http://localhost:5010`
Swagger UI: `http://localhost:5010`

## 🏗️ Business Layer

Bu proje, business logic'i ayrı bir Class Library'de tutar:

**Repository:** [CityHotelGarage.Business](https://github.com/Glyberax/CityHotelGarage.Business)

### Business Layer İçeriği:
- **Repository Pattern** - Veri erişim katmanı
- **Service Layer** - İş mantığı katmanı
- **DTOs** - Veri transfer objeleri
- **Validators** - FluentValidation kuralları
- **Mappings** - AutoMapper profilleri

### Kurulum Seçenekleri:

#### Option 1: Git Submodule (Önerilen)
```bash
git submodule add https://github.com/Glyberax/CityHotelGarage.Business.git CityHotelGarage.Business
```

#### Option 2: Manuel Klonlama
```bash
git clone https://github.com/Glyberax/CityHotelGarage.Business.git
# Solution'a project reference ekleyin
```

## 📋 API Endpoints

### Cities (Şehirler)
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| `GET` | `/api/Cities` | Tüm şehirleri listele |
| `GET` | `/api/Cities/{id}` | Belirli bir şehri getir |
| `POST` | `/api/Cities` | Yeni şehir ekle |
| `PUT` | `/api/Cities/{id}` | Şehir güncelle |
| `DELETE` | `/api/Cities/{id}` | Şehir sil |

### Hotels (Oteller)
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| `GET` | `/api/Hotels` | Tüm otelleri listele |
| `GET` | `/api/Hotels/{id}` | Belirli bir oteli getir |
| `POST` | `/api/Hotels` | Yeni otel ekle |
| `PUT` | `/api/Hotels/{id}` | Otel güncelle |
| `DELETE` | `/api/Hotels/{id}` | Otel sil |

### Garages (Garajlar)
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| `GET` | `/api/Garages` | Tüm garajları listele |
| `GET` | `/api/Garages/{id}` | Belirli bir garajı getir |
| `POST` | `/api/Garages` | Yeni garaj ekle |
| `PUT` | `/api/Garages/{id}` | Garaj güncelle |
| `DELETE` | `/api/Garages/{id}` | Garaj sil |

### Cars (Arabalar)
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| `GET` | `/api/Cars` | Tüm arabaları listele |
| `GET` | `/api/Cars/{id}` | Belirli bir arabayı getir |
| `GET` | `/api/Cars/ByLicensePlate/{plate}` | Plakaya göre araç getir |
| `POST` | `/api/Cars` | Yeni araç park et |
| `PUT` | `/api/Cars/{id}` | Araç bilgilerini güncelle |
| `DELETE` | `/api/Cars/{id}` | Aracı park yerinden çıkar |

## 🏛️ Mimari

### Proje Yapısı
```
CityHotelGarageAPI/
├── Controllers/           # API Controller'ları
├── CityHotelGarage.Business/  # Business Logic (Submodule)
│   ├── Operations/
│   │   ├── DTOs/         # Data Transfer Objects
│   │   ├── Services/     # Business Services
│   │   ├── Validators/   # FluentValidation Rules
│   │   ├── Interfaces/   # Service Interfaces
│   │   └── Mappings/     # AutoMapper Profiles
│   └── Repository/
│       ├── Models/       # Entity Models
│       ├── Data/         # DbContext
│       ├── Repositories/ # Repository Implementation
│       └── Interfaces/   # Repository Interfaces
├── Program.cs            # Startup Configuration
└── appsettings.json     # Configuration
```

### Katmanlar
1. **API Layer** - Controller'lar ve HTTP handling
2. **Service Layer** - Business logic ve validation
3. **Repository Layer** - Veri erişimi
4. **Data Layer** - Entity Framework ve database

## ⚙️ Konfigürasyon

### FluentValidation
Tüm DTO'lar için validation kuralları:
- Turkish license plate format validation
- String length validation
- Required field validation
- Range validation for numeric fields

### AutoMapper
Entity ↔ DTO mapping configuration

### Entity Framework
- PostgreSQL provider
- Code-First approach
- Migration support
- Seed data for demo

## 🎯 Kullanım

### Demo Veriler
Uygulama ilk çalıştırıldığında otomatik demo veriler eklenir:
- 2 Şehir (İstanbul, Ankara)
- 3 Otel
- 3 Garaj
- 3 Araç

### Swagger UI
API'yi test etmek için: `http://localhost:5010`

### Örnek Requests

#### Yeni Araç Park Etme
```
POST /api/Cars
{
  "brand": "BMW",
  "licensePlate": "34ABC123",
  "ownerName": "Ahmet Yılmaz",
  "garageId": 1
}
```

#### Şehir Ekleme
```
POST /api/Cities
{
  "name": "İzmir",
  "population": 4500000
}
```

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'i push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

## 📝 License

Bu proje MIT lisansı altında lisanslanmıştır.

## 📞 İletişim

**Geliştirici:** Arda Çalışkan  
**GitHub:** [@Glyberax](https://github.com/Glyberax)

---

⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!