# 🚀 PremiumYol - Kurulum Rehberi

Bu dokümanda projenin nasıl kurulacağı ve çalıştırılacağı adım adım anlatılmaktadır.

---

## 📋 Gereksinimler

- .NET SDK 8.0 veya üzeri
- Visual Studio 2022 / VS Code / Rider
- SQLite (dahili) veya SQL Server
- Git

---

## 🔧 Kurulum Adımları

### 1. Projeyi İndirin

```bash
git clone https://github.com/baranakbulut/PremiumYol.git
cd PremiumYol
```

### 2. Bağımlılıkları Yükleyin

```bash
cd TrendyolClone
dotnet restore
```

### 3. Veritabanını Oluşturun

```bash
# Otomatik oluşturulur (ilk çalıştırmada)
# Veya manuel:
dotnet ef database update
```

### 4. Projeyi Çalıştırın

```bash
dotnet run
```

Tarayıcınızda şu adresleri açın:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

---

## 👤 Varsayılan Hesaplar

### Admin Hesabı
- Email: `admin@trendyol.com`
- Şifre: `admin123`

### Test Kullanıcısı
- Email: `test@test.com`
- Şifre: `test123`

---

## 🧪 Test

Endpoint testlerini çalıştırmak için:

```powershell
cd docs
.\test-endpoints.ps1
```

---

## 🐛 Sorun Giderme

### Port Kullanımda Hatası

```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### Veritabanı Hatası

```bash
dotnet ef database drop
dotnet ef database update
```

---

## 📚 Daha Fazla Bilgi

- [README.md](../README.md) - Genel proje bilgileri
- [MODUL_DOKUMANTASYONU.md](MODUL_DOKUMANTASYONU.md) - Modül detayları
- [TrendyolClone/README.md](../TrendyolClone/README.md) - Teknik dokümantasyon
