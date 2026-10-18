# Personel Yönetimi

Personel ve departman kayıtlarının yönetildiği web uygulaması.

| Proje | Teknolojiler | Doküman |
|---|---|---|
| [backend](backend/) | .NET 10, ASP.NET Core Web API, EF Core (Code First), PostgreSQL, FluentValidation | [backend/README.md](backend/README.md) |
| [frontend](frontend/) | React, JavaScript, Vite, Ant Design, Axios, Context API | [frontend/README.md](frontend/README.md) |

## Hızlı Başlangıç

Gereksinimler: .NET 10 SDK, `dotnet-ef` aracı, PostgreSQL, Node.js 20.19+

1. `backend/Presentation/Api/appsettings.json` içindeki PostgreSQL bağlantı bilgisini düzenleyin.
2. Veritabanını oluşturup API'yi başlatın (`backend` klasöründe):

   ```bash
   dotnet ef database update --project Infrastructure/Persistance --startup-project Presentation/Api
   dotnet run --project Presentation/Api --launch-profile http
   ```

3. Frontend'i başlatın (`frontend` klasöründe):

   ```bash
   npm install
   npm run dev
   ```

4. Tarayıcıda `http://localhost:5173` adresini açın.

API adresi `frontend/.env` içindeki `VITE_API_URL` ile ayarlanır (varsayılan `http://localhost:5207/api`). Endpoint listesi ve API arayüzü (Scalar) için backend README'sine bakın.
