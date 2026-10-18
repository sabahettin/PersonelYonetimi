# Personel Yönetimi — Frontend

Personel ve departman kayıtlarını listeleme, ekleme, düzenleme ve silme işlemlerinin yapıldığı React arayüzü. Veriler [backend](../backend/README.md) REST API'si üzerinden yönetilir.

**Teknolojiler:** React 19, JavaScript, Vite, Ant Design 6, Axios

## Gereksinimler

- [Node.js](https://nodejs.org/) 20.19+ veya 22.12+ (Vite 8 gereksinimi)
- Çalışır durumda backend API (kurulum için [backend/README.md](../backend/README.md))

## Kurulum ve Çalıştırma

### 1. Backend'i başlatın

`backend` klasöründe veritabanını oluşturup API'yi çalıştırın (ayrıntılar backend README'sinde):

```bash
dotnet ef database update --project Infrastructure/Persistance --startup-project Presentation/Api
dotnet run --project Presentation/Api --launch-profile http
```

### 2. API adresini ayarlayın

API adresi `frontend/.env` dosyasındaki `VITE_API_URL` değişkeninden okunur. Dosya yoksa varsayılan olarak `http://localhost:5207/api` kullanılır.

```env
VITE_API_URL=http://localhost:5207/api
```

| Backend profili | `VITE_API_URL` |
|---|---|
| `http` | `http://localhost:5207/api` |
| `https` | `https://localhost:7223/api` |

> `https` profilinde backend HTTP isteklerini HTTPS'e yönlendirir. Tarayıcı, yönlendirilen POST/PUT/DELETE isteklerinde CORS hatası verebilir. Bu yüzden adres, çalışan profille aynı olmalıdır. `.env` değiştirildiğinde geliştirme sunucusunu yeniden başlatın.

### 3. Bağımlılıkları kurup uygulamayı başlatın

`frontend` klasöründe:

```bash
npm install
npm run dev
```

Uygulama `http://localhost:5173` adresinde açılır.

### Diğer komutlar

| Komut | Açıklama |
|---|---|
| `npm run build` | Üretim derlemesini `dist/` klasörüne alır |
| `npm run preview` | Üretim derlemesini yerelde sunar |
| `npm run lint` | Kodu Oxlint ile denetler |

## Özellikler

**Personeller**
- Ad, soyad, e-posta ve departman bilgileriyle listeleme
- Ekleme ve düzenleme formu (modal). Departman seçenekleri backend'den gelir.
- Düzenlemede kayıt `GET /api/employees/{id}` ile backend'den alınarak forma doldurulur
- Silme öncesinde onay

**Departmanlar**
- Listeleme, ekleme, düzenleme ve silme
- Personeli olan bir departman silinmek istenirse backend'in döndüğü açıklama gösterilir (409)

**Ortak davranışlar**
- Her satırda aksiyon butonu ve Düzenle / Sil seçenekli Dropdown menü
- Form validasyonları: zorunlu alanlar, e-posta formatı, uzunluk sınırları (backend kurallarıyla aynı)
- Veri yüklenirken tablo ve formlarda yükleniyor göstergesi
- Liste boşken bilgilendirme metni
- İşlem sonrası başarı veya hata bildirimi. Backend'in validasyon ve hata mesajları kullanıcıya gösterilir. Backend'e ulaşılamazsa ayrı bir uyarı verilir.

## Proje Yapısı

```
src/
├── api/
│   ├── api.js                 # Axios örneği (baseURL = VITE_API_URL)
│   ├── employeeService.js     # Personel endpoint çağrıları (getAll, getById, add, update, remove)
│   └── departmentService.js   # Departman endpoint çağrıları
├── context/
│   ├── EmployeeContext.jsx    # Personel verisi ve işlemleri (useEmployee)
│   └── DepartmentContext.jsx  # Departman verisi ve işlemleri (useDepartment)
├── components/
│   └── RowActions.jsx         # Tablo satırı için Düzenle / Sil Dropdown menüsü
├── pages/
│   ├── EmployeesPage.jsx      # Personel listesi
│   ├── EmployeesModal.jsx     # Personel ekleme / düzenleme formu
│   ├── DepartmentsPage.jsx    # Departman listesi
│   └── DepartmentsModal.jsx   # Departman ekleme / düzenleme formu
├── layout/sidebar/
│   └── sidebar.jsx            # Sol menü ve sayfa geçişi
├── utils/
│   └── getErrorMessage.js     # Backend ProblemDetails hatalarını kullanıcı mesajına çevirir
├── App.jsx                    # Ant Design App ve context provider'ları
└── main.jsx
```

## Mimari

Veri akışı tek yönlüdür: **Sayfa / Modal → Context → Servis → API**

- **Servis katmanı (`api/`):** Tüm HTTP istekleri burada yapılır. Component'ler Axios'u doğrudan kullanmaz.
- **Context API (`context/`):** Liste verisi, yükleniyor durumu ve ekleme / güncelleme / silme fonksiyonları merkezi olarak tutulur. Her değişiklikten sonra liste backend'den yeniden çekilir. Böylece ekran her zaman backend'deki güncel veriyi gösterir; örneğin departman adı backend tarafında birleştirilir.
- **Component'ler (`pages/`, `components/`):** Veriyi context hook'ları (`useEmployee`, `useDepartment`) üzerinden okur. Kullanıcıya bildirim göstermek ve formu yönetmek bu katmanın işidir.
- **Bildirimler:** Ant Design `App` bileşeni ile `App.useApp()` üzerinden gösterilir. Bu yüzden `App.jsx` uygulamayı `<AntApp>` ile sarar.

## Kullanılan API Endpoint'leri

| İşlem | İstek |
|---|---|
| Personelleri listele | `GET /api/employees` |
| Personel detayı (düzenleme formu) | `GET /api/employees/{id}` |
| Personel ekle | `POST /api/employees` |
| Personel güncelle | `PUT /api/employees/{id}` |
| Personel sil | `DELETE /api/employees/{id}` |
| Departmanları listele | `GET /api/departments` |
| Departman ekle | `POST /api/departments` |
| Departman güncelle | `PUT /api/departments/{id}` |
| Departman sil | `DELETE /api/departments/{id}` |

İstek ve yanıt modelleri, validasyon kuralları ve hata formatı için [backend/README.md](../backend/README.md) dosyasına bakın.
