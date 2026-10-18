# Personel Yönetimi — Backend

Personel ve departman kayıtlarını yöneten REST API.

**Teknolojiler:** .NET 10, ASP.NET Core Web API, Entity Framework Core 10 (Code First), PostgreSQL (Npgsql), FluentValidation, AutoMapper, Scalar (OpenAPI arayüzü)

## Proje Yapısı

```
backend/
├── Core/
│   ├── Domain/          # Entity'ler (Employee, Department, BaseEntity) ve enum'lar
│   └── Application/     # DTO'lar, FluentValidation validator'ları, AutoMapper profili,
│                        # repository arayüzleri, uygulamaya özel exception'lar
├── Infrastructure/
│   └── Persistance/     # DbContext, entity konfigürasyonları, migration'lar,
│                        # repository implementasyonları
└── Presentation/
    └── Api/             # Controller'lar, exception handler'lar, Program.cs
```

Bağımlılık yönü: `Api → Persistance → Application → Domain`

## Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 14+ (yerelde `localhost:5432` üzerinde çalışır durumda)
- EF Core CLI aracı (migration'ları komut satırından uygulamak için):

```bash
dotnet tool install --global dotnet-ef
```

## Kurulum ve Çalıştırma

### 1. Bağlantı bilgisini ayarlayın

`Presentation/Api/appsettings.json` içinde şifre yerine `<şifreniz>` yazar. Şifreyi repoya göndermemek için `Presentation/Api/appsettings.Development.json` dosyasına kendi PostgreSQL bilgilerinizle ekleyin. Bu dosya `.gitignore` ile repo dışında tutulur ve `appsettings.json`'daki değeri ezer:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=EmployeeDb;Username=postgres;Password=<şifreniz>"
  }
}
```

API (`Development` ortamı) ve `dotnet ef` komutları bu dosyayı otomatik okur.

AutoMapper lisans anahtarınız varsa aynı dosyaya `"AutoMapper": { "LicenseKey": "<anahtar>" }` olarak ekleyin.

### 2. Veritabanını oluşturun

Veritabanı Code First yaklaşımıyla migration'lardan oluşturulur. `EmployeeDb` veritabanının önceden var olması gerekmez.

`backend` klasöründe:

```bash
dotnet ef database update --project Infrastructure/Persistance --startup-project Presentation/Api
```

Visual Studio kullanıyorsanız: Startup project olarak `Api`'yi seçin, Package Manager Console'da Default project olarak `Persistance`'ı seçin ve `Update-Database` çalıştırın.

### 3. API'yi başlatın

```bash
dotnet run --project Presentation/Api --launch-profile http
```

| | Adres |
|---|---|
| API | `http://localhost:5207` |
| Scalar arayüzü | `http://localhost:5207/scalar/v1` |
| OpenAPI dokümanı | `http://localhost:5207/openapi/v1.json` |

Frontend varsayılan olarak bu adresi kullanır (`frontend/.env` → `VITE_API_URL=http://localhost:5207/api`).

> **HTTPS ile çalıştırmak isterseniz:** `--launch-profile https` ile başlatın. API `https://localhost:7223` adresinde de yayın yapar ve HTTP isteklerini HTTPS'e yönlendirir. Bu durumda frontend'deki `VITE_API_URL` değerini `https://localhost:7223/api` yapın. Aksi halde tarayıcı, yönlendirilen POST/PUT/DELETE isteklerinde CORS hatası verebilir.

## Endpoint'ler

Tüm endpoint'ler `/api` ön ekiyle başlar. Kimlik doğrulama gerekmez.

### Personeller — `/api/employees`

| Metot | Yol | Açıklama | Başarılı | Olası hatalar |
|---|---|---|---|---|
| GET | `/api/employees` | Tüm personelleri departman adıyla listeler | 200 | |
| GET | `/api/employees/{id}` | Tek bir personelin detayı | 200 | 404 |
| POST | `/api/employees` | Yeni personel ekler | 201 + `Location` başlığı | 400 |
| PUT | `/api/employees/{id}` | Personeli günceller | 204 | 400, 404 |
| DELETE | `/api/employees/{id}` | Personeli siler (soft delete) | 204 | 404 |

**Ekleme isteği** (`POST /api/employees`):

```json
{
  "name": "Ayşe",
  "surname": "Yılmaz",
  "email": "ayse.yilmaz@example.com",
  "departmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Güncelleme isteği** (`PUT /api/employees/{id}`): Gövde ekleme isteğiyle aynıdır, ek olarak `id` alanı içerir. Bu alan URL'deki id ile aynı olmalıdır.

**Yanıt modeli:**

```json
{
  "id": "8b0c7a1e-2f4d-4c1a-9a51-0e6f5d1c2b3a",
  "name": "Ayşe",
  "surname": "Yılmaz",
  "email": "ayse.yilmaz@example.com",
  "departmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "departmentName": "Yazılım"
}
```

**Validasyon kuralları:**

| Alan | Kural |
|---|---|
| `name` | Zorunlu, en fazla 30 karakter |
| `surname` | Zorunlu, en fazla 30 karakter |
| `email` | Zorunlu, geçerli e-posta formatı, en fazla 75 karakter, başka bir personelde kayıtlı olmamalı |
| `departmentId` | Zorunlu |

### Departmanlar — `/api/departments`

Frontend'deki departman seçim listesi bu endpoint'ten beslenir.

| Metot | Yol | Açıklama | Başarılı | Olası hatalar |
|---|---|---|---|---|
| GET | `/api/departments` | Tüm departmanları listeler | 200 | |
| GET | `/api/departments/{id}` | Tek bir departmanın detayı | 200 | 404 |
| POST | `/api/departments` | Yeni departman ekler | 201 | 400 |
| PUT | `/api/departments/{id}` | Departmanı günceller | 204 | 400, 404 |
| DELETE | `/api/departments/{id}` | Departmanı siler (soft delete) | 204 | 404, 409 |

Departman adı zorunludur, 2-100 karakter arasında olmalıdır ve benzersizdir. Aktif personeli olan bir departman silinemez. Bu durumda `409 Conflict` döner.

## Hata Yanıtları

Hatalar [RFC 9457 ProblemDetails](https://www.rfc-editor.org/rfc/rfc9457) formatında döner.

**Validasyon hatası (400):** Hangi alanın neden geçersiz olduğu `errors` içinde listelenir.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": ["A valid email address is required."]
  }
}
```

**Kayıt bulunamadı (404), silme kısıtı (409):** `title` ve `detail` alanlarında açıklama döner.

## Tasarım Notları

- **Soft delete:** Silinen kayıtlar veritabanından kaldırılmaz. `IsDeleted` alanı işaretlenir ve global query filter sayesinde sorgularda görünmez.
- **Audit alanları:** `CreatedDate`, `ModifiedDate`, `DeletedDate` ve ilgili kullanıcı alanları `ApplicationDbContext.SaveChangesAsync` içinde otomatik doldurulur.
- **Katmanlama:** Controller'lar veritabanına doğrudan erişmez. Tüm veri erişimi repository'ler üzerinden yapılır. Dışarıya entity'ler değil DTO'lar açılır.
