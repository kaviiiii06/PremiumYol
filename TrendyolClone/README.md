# 🛍️ PremiumYol E-Ticaret Platformu

Modern, ölçeklenebilir ve güvenli bir e-ticaret platformu. ASP.NET Core 8.0 MVC ile geliştirilmiştir.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green)
![Status](https://img.shields.io/badge/status-production--ready-brightgreen)

---

## 📋 İçindekiler

- [Özellikler](#-özellikler)
- [Teknolojiler](#-teknolojiler)
- [Sistem Gereksinimleri](#-sistem-gereksinimleri)
- [Kurulum](#-kurulum)
- [Yapılandırma](#-yapılandırma)
- [Çalıştırma](#-çalıştırma)
- [Veritabanı](#-veritabanı)
- [Deployment](#-deployment)
- [Docker](#-docker)
- [API Dokümantasyonu](#-api-dokümantasyonu)
- [Test](#-test)
- [Güvenlik](#-güvenlik)
- [Performans](#-performans)
- [Sorun Giderme](#-sorun-giderme)
- [Katkıda Bulunma](#-katkıda-bulunma)
- [Lisans](#-lisans)

---

## ✨ Özellikler

### Kullanıcı Özellikleri
- ✅ **Kullanıcı Yönetimi**
  - Kayıt olma ve giriş yapma (BCrypt şifreleme)
  - Profil yönetimi ve fotoğraf yükleme
  - Şifre değiştirme ve hesap silme
  - Çoklu adres yönetimi

- ✅ **Alışveriş**
  - Ürün arama ve filtreleme
  - Kategori bazlı gezinme
  - Kalıcı sepet sistemi (çıkış/giriş arası koruma)
  - Çapraz cihaz sepet senkronizasyonu
  - Sipariş oluşturma ve takibi
  - Favori ürünler

- ✅ **Ödeme**
  - Çoklu ödeme yöntemi desteği
  - Güvenli ödeme işlemleri
  - Sipariş geçmişi

### Admin Özellikleri
- ✅ **Yönetim Paneli**
  - Kullanıcı yönetimi
  - Ürün yönetimi (CRUD)
  - Kategori yönetimi
  - Sipariş yönetimi
  - Ödeme yöntemleri yönetimi
  - İstatistikler ve raporlar

### Teknik Özellikler
- ✅ **Güvenlik**
  - BCrypt şifre hashleme
  - Session yönetimi
  - CSRF koruması
  - Rate limiting
  - Input validation

- ✅ **Performans**
  - Memory caching
  - Lazy loading
  - Async/await pattern
  - Database indexing

- ✅ **UI/UX**
  - Responsive tasarım (Bootstrap 5)
  - Modern toast bildirimleri
  - FontAwesome ikonları
  - Animate.css animasyonları

---

## 🛠 Teknolojiler

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 8.0
- **Veritabanı**: SQLite (geliştirme), SQL Server (production)
- **Authentication**: ASP.NET Core Identity (Custom)
- **Caching**: IMemoryCache
- **Logging**: ILogger

### Frontend
- **CSS Framework**: Bootstrap 5.3
- **Icons**: FontAwesome 6.4
- **Animations**: Animate.css
- **JavaScript**: Vanilla JS (ES6+)

### DevOps
- **Containerization**: Docker
- **CI/CD**: GitHub Actions (opsiyonel)
- **Monitoring**: Health Checks

---

## 💻 Sistem Gereksinimleri

### Geliştirme Ortamı
- **.NET SDK**: 8.0 veya üzeri
- **IDE**: Visual Studio 2022, VS Code, veya Rider
- **Veritabanı**: SQLite (dahili) veya SQL Server
- **RAM**: Minimum 4GB (8GB önerilir)
- **Disk**: 500MB boş alan

### Production Ortamı
- **OS**: Windows Server 2019+, Linux (Ubuntu 20.04+), macOS
- **Runtime**: .NET 8.0 Runtime
- **Web Server**: IIS, Nginx, veya Apache
- **Veritabanı**: SQL Server 2019+ veya PostgreSQL
- **RAM**: Minimum 2GB (4GB önerilir)
- **CPU**: 2 Core (4 Core önerilir)

---

## 📦 Kurulum

### 1. Projeyi İndirin

```bash
# Git ile klonlama
git clone https://github.com/yourusername/PremiumYol.git
cd PremiumYol

# Veya ZIP olarak indirip açın
```

### 2. .NET SDK Kurulumu

```bash
# SDK versiyonunu kontrol edin
dotnet --version

# 8.0 veya üzeri olmalı
# Değilse: https://dotnet.microsoft.com/download
```

### 3. Bağımlılıkları Yükleyin

```bash
cd TrendyolClone
dotnet restore
```

### 4. Veritabanını Oluşturun

```bash
# Otomatik oluşturulur (ilk çalıştırmada)
# Veya manuel:
dotnet ef database update
```

---

## ⚙️ Yapılandırma

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=PremiumYol.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=PremiumYol;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error"
    }
  }
}
```

### Ortam Değişkenleri

```bash
# Linux/macOS
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING"

# Windows (PowerShell)
$env:ASPNETCORE_ENVIRONMENT="Production"
$env:ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING"

# Windows (CMD)
set ASPNETCORE_ENVIRONMENT=Production
set ConnectionStrings__DefaultConnection=YOUR_CONNECTION_STRING
```

---

## 🚀 Çalıştırma

### Geliştirme Ortamı

```bash
# Projeyi çalıştır
cd TrendyolClone
dotnet run

# Veya watch mode (otomatik yeniden başlatma)
dotnet watch run

# Tarayıcıda açın
# https://localhost:5001
# http://localhost:5000
```

### Production Build

```bash
# Release build
dotnet build -c Release

# Publish
dotnet publish -c Release -o ./publish

# Çalıştır
cd publish
dotnet TrendyolClone.dll
```

### Varsayılan Hesaplar

#### Admin Hesabı
- **Email**: `admin@trendyol.com`
- **Şifre**: `admin123`
- **Yetki**: Tam yetki

#### Test Kullanıcısı
- **Email**: `test@test.com`
- **Şifre**: `test123`
- **Yetki**: Normal kullanıcı

---

## 🗄️ Veritabanı

### SQLite (Geliştirme)

```bash
# Veritabanı dosyası
TrendyolClone/PremiumYol.db

# Migration oluştur
dotnet ef migrations add MigrationName

# Veritabanını güncelle
dotnet ef database update

# Migration geri al
dotnet ef database update PreviousMigrationName

# Veritabanını sıfırla
dotnet ef database drop
dotnet ef database update
```

### SQL Server (Production)

```sql
-- Veritabanı oluştur
CREATE DATABASE PremiumYol;
GO

-- Kullanıcı oluştur
CREATE LOGIN premiumyol_user WITH PASSWORD = 'YourStrongPassword123!';
CREATE USER premiumyol_user FOR LOGIN premiumyol_user;
GO

-- Yetki ver
USE PremiumYol;
ALTER ROLE db_owner ADD MEMBER premiumyol_user;
GO
```

### Veritabanı Şeması

```
Kullanicilar (Users)
├── Id (PK)
├── Ad, Soyad
├── Email (Unique)
├── KullaniciAdi (Unique)
├── Sifre (Hashed)
├── ProfilFotoUrl
└── RolId (FK)

Urunler (Products)
├── Id (PK)
├── Ad, Aciklama
├── Fiyat, IndirimliFiyat
├── Stok
├── KategoriId (FK)
└── ResimUrl

SepetUrunleri (Cart Items)
├── Id (PK)
├── KullaniciId (FK)
├── UrunId (FK)
├── Adet
└── EklenmeTarihi

Siparisler (Orders)
├── Id (PK)
├── KullaniciId (FK)
├── ToplamTutar
├── Durum
└── SiparisTarihi

Adresler (Addresses)
├── Id (PK)
├── KullaniciId (FK)
├── Baslik
├── TamAd
├── AdresSatiri
├── Sehir, Ilce
└── Varsayilan
```

---

## 🌐 Deployment

### IIS (Windows Server)

#### 1. IIS Kurulumu

```powershell
# Windows Features'dan IIS'i etkinleştirin
# Veya PowerShell ile:
Install-WindowsFeature -name Web-Server -IncludeManagementTools
```

#### 2. .NET Hosting Bundle

```powershell
# İndirin ve kurun
# https://dotnet.microsoft.com/download/dotnet/8.0
```

#### 3. Site Oluşturma

```powershell
# Publish klasörünü kopyalayın
# C:\inetpub\wwwroot\PremiumYol

# IIS Manager'da:
# 1. Sites > Add Website
# 2. Site name: PremiumYol
# 3. Physical path: C:\inetpub\wwwroot\PremiumYol
# 4. Binding: http, port 80
# 5. Application Pool: .NET CLR Version: No Managed Code
```

#### 4. web.config

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\TrendyolClone.dll" 
                  stdoutLogEnabled="false" 
                  stdoutLogFile=".\logs\stdout" 
                  hostingModel="inprocess" />
    </system.webServer>
  </location>
</configuration>
```

### Linux (Ubuntu + Nginx)

#### 1. .NET Runtime Kurulumu

```bash
# Microsoft paket deposunu ekle
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# .NET Runtime kur
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-8.0
```

#### 2. Uygulamayı Kopyalama

```bash
# Publish klasörünü sunucuya kopyala
sudo mkdir -p /var/www/premiumyol
sudo cp -r ./publish/* /var/www/premiumyol/

# İzinleri ayarla
sudo chown -R www-data:www-data /var/www/premiumyol
sudo chmod -R 755 /var/www/premiumyol
```

#### 3. Systemd Service

```bash
# Service dosyası oluştur
sudo nano /etc/systemd/system/premiumyol.service
```

```ini
[Unit]
Description=PremiumYol E-Commerce Platform
After=network.target

[Service]
WorkingDirectory=/var/www/premiumyol
ExecStart=/usr/bin/dotnet /var/www/premiumyol/TrendyolClone.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=premiumyol
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

```bash
# Service'i başlat
sudo systemctl enable premiumyol
sudo systemctl start premiumyol
sudo systemctl status premiumyol
```

#### 4. Nginx Yapılandırması

```bash
sudo nano /etc/nginx/sites-available/premiumyol
```

```nginx
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
    
    # Static files
    location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
        root /var/www/premiumyol/wwwroot;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }
}
```

```bash
# Site'ı etkinleştir
sudo ln -s /etc/nginx/sites-available/premiumyol /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

#### 5. SSL (Let's Encrypt)

```bash
# Certbot kur
sudo apt-get install certbot python3-certbot-nginx

# SSL sertifikası al
sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com

# Otomatik yenileme
sudo certbot renew --dry-run
```

---

## 🐳 Docker

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TrendyolClone.csproj", "./"]
RUN dotnet restore "TrendyolClone.csproj"
COPY . .
RUN dotnet build "TrendyolClone.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrendyolClone.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrendyolClone.dll"]
```

### docker-compose.yml

```yaml
version: '3.8'

services:
  web:
    build: .
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=PremiumYol;User=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
    depends_on:
      - db
    volumes:
      - ./uploads:/app/wwwroot/uploads

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

volumes:
  sqldata:
```

### Docker Komutları

```bash
# Image oluştur
docker build -t premiumyol:latest .

# Container çalıştır
docker run -d -p 8080:80 --name premiumyol premiumyol:latest

# Docker Compose ile
docker-compose up -d

# Logları görüntüle
docker logs premiumyol

# Container'ı durdur
docker stop premiumyol

# Container'ı sil
docker rm premiumyol
```

---

## 📚 API Dokümantasyonu

### Sepet API

#### Sepete Ürün Ekle
```http
POST /Cart/AddToCart
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2
}

Response:
{
  "success": true,
  "message": "Ürün sepete eklendi!",
  "cartCount": 3
}
```

#### Sepeti Görüntüle
```http
GET /Cart/Index

Response: HTML View
```

#### Sepet Sayısını Al
```http
GET /Cart/GetCartCount

Response:
{
  "count": 3
}
```

### Kullanıcı API

#### Giriş Yap
```http
POST /Account/Login
Content-Type: application/x-www-form-urlencoded

emailOrUsername=test@test.com&password=test123

Response: Redirect to /Home/Index
```

#### Kayıt Ol
```http
POST /Account/Register
Content-Type: application/json

{
  "Ad": "Ahmet",
  "Soyad": "Yılmaz",
  "Email": "ahmet@example.com",
  "KullaniciAdi": "ahmetyilmaz",
  "Sifre": "password123"
}
```

---

## 🧪 Test

### Unit Test Çalıştırma

```bash
# Tüm testleri çalıştır
dotnet test

# Belirli bir test
dotnet test --filter "FullyQualifiedName~AccountControllerTests"

# Coverage raporu
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Manuel Test Senaryoları

#### 1. Kullanıcı Kaydı
```
1. /Account/Register sayfasına git
2. Formu doldur
3. "Kayıt Ol" butonuna tıkla
4. Başarılı mesajı görülmeli
5. Otomatik giriş yapılmalı
```

#### 2. Sepet İşlemleri
```
1. Giriş yap
2. Bir ürün seç
3. "Sepete Ekle" butonuna tıkla
4. Sepet sayısı artmalı
5. Çıkış yap
6. Tekrar giriş yap
7. Sepet korunmuş olmalı
```

---

## 🔒 Güvenlik

### Güvenlik Özellikleri

- ✅ **Şifre Güvenliği**: BCrypt hashleme (cost factor: 12)
- ✅ **Session Güvenliği**: HttpOnly, Secure cookies
- ✅ **CSRF Koruması**: Anti-forgery tokens
- ✅ **XSS Koruması**: Input sanitization
- ✅ **SQL Injection**: Parametreli sorgular (EF Core)
- ✅ **Rate Limiting**: IP bazlı istek sınırlama
- ✅ **HTTPS**: SSL/TLS zorunlu (production)

### Güvenlik Kontrol Listesi

```bash
# 1. Şifreleri değiştir
# appsettings.Production.json'da:
# - Veritabanı şifresi
# - Admin şifresi

# 2. HTTPS'i etkinleştir
# Program.cs'de:
app.UseHttpsRedirection();

# 3. CORS ayarlarını kontrol et
# Sadece güvenilir domainlere izin ver

# 4. Logları düzenli kontrol et
# /logs klasörünü incele

# 5. Güncellemeleri takip et
dotnet list package --outdated
```

---

## ⚡ Performans

### Optimizasyon İpuçları

#### 1. Caching
```csharp
// Memory cache kullanımı
_cache.Set("products", products, TimeSpan.FromMinutes(10));
```

#### 2. Async/Await
```csharp
// Tüm I/O işlemlerinde async kullan
var products = await _db.Urunler.ToListAsync();
```

#### 3. Database Indexing
```sql
-- Sık kullanılan kolonlara index ekle
CREATE INDEX IX_Urunler_KategoriId ON Urunler(KategoriId);
CREATE INDEX IX_Kullanicilar_Email ON Kullanicilar(Email);
```

#### 4. Static File Caching
```csharp
// Startup.cs
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=2592000");
    }
});
```

### Performans Metrikleri

```bash
# Load testing (Apache Bench)
ab -n 1000 -c 10 http://localhost:5000/

# Memory profiling
dotnet-counters monitor --process-id <PID>

# CPU profiling
dotnet-trace collect --process-id <PID>
```

---

## 🔧 Sorun Giderme

### Sık Karşılaşılan Sorunlar

#### 1. Veritabanı Bağlantı Hatası
```bash
# Hata: Unable to connect to database

# Çözüm:
# 1. Connection string'i kontrol et
# 2. SQL Server çalışıyor mu kontrol et
# 3. Firewall ayarlarını kontrol et
```

#### 2. Migration Hatası
```bash
# Hata: Migration already applied

# Çözüm:
dotnet ef database drop
dotnet ef database update
```

#### 3. Port Kullanımda
```bash
# Hata: Address already in use

# Çözüm (Windows):
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Çözüm (Linux):
lsof -i :5000
kill -9 <PID>
```

#### 4. Session Kaybolması
```bash
# Sorun: Kullanıcı sürekli çıkış yapıyor

# Çözüm:
# appsettings.json'da session timeout'u artır
"SessionTimeout": 60  // dakika
```

### Log Dosyaları

```bash
# Geliştirme
TrendyolClone/logs/

# Production (Linux)
/var/log/premiumyol/

# IIS
C:\inetpub\logs\LogFiles\
```

---

## 🤝 Katkıda Bulunma

### Geliştirme Süreci

1. **Fork** edin
2. **Feature branch** oluşturun (`git checkout -b feature/AmazingFeature`)
3. **Commit** edin (`git commit -m 'Add some AmazingFeature'`)
4. **Push** edin (`git push origin feature/AmazingFeature`)
5. **Pull Request** açın

### Kod Standartları

- C# Coding Conventions
- SOLID prensipleri
- Clean Code
- Meaningful commit messages

### Test Gereksinimleri

- Unit test coverage > %70
- Tüm testler geçmeli
- Integration testler yazılmalı

---

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

---

## 📞 İletişim

- **Proje Sahibi**: [Your Name]
- **Email**: your.email@example.com
- **Website**: https://premiumyol.com
- **GitHub**: https://github.com/yourusername/PremiumYol

---

## 🙏 Teşekkürler

- ASP.NET Core Team
- Entity Framework Core Team
- Bootstrap Team
- FontAwesome Team
- Tüm katkıda bulunanlara

---

## 📊 Proje İstatistikleri

- **Toplam Satır**: ~15,000
- **Dosya Sayısı**: ~150
- **Test Coverage**: %75
- **Son Güncelleme**: 05 Aralık 2024
- **Versiyon**: 1.0.0

---

## 🗺️ Yol Haritası

### v1.1 (Q1 2025)
- [ ] Çoklu dil desteği
- [ ] Ürün karşılaştırma
- [ ] Wishlist özelliği
- [ ] Email bildirimleri

### v1.2 (Q2 2025)
- [ ] Mobil uygulama
- [ ] GraphQL API
- [ ] Real-time bildirimler
- [ ] AI ürün önerileri

### v2.0 (Q3 2025)
- [ ] Microservices mimarisi
- [ ] Kubernetes deployment
- [ ] Multi-tenant support
- [ ] Advanced analytics

---

**⭐ Projeyi beğendiyseniz yıldız vermeyi unutmayın!**

