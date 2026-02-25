# 🛍️ PremiumYol E-Ticaret Platformu

Modern, ölçeklenebilir ve güvenli bir e-ticaret platformu. ASP.NET Core 8.0 MVC ile geliştirilmiştir.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green)
![Status](https://img.shields.io/badge/status-production--ready-brightgreen)
![Build](https://img.shields.io/badge/build-passing-success)
![Coverage](https://img.shields.io/badge/completion-100%25-success)

---

## 🎉 Proje Durumu

**Versiyon**: 2.0.0  
**Durum**: Production Ready ✅  
**Son Güncelleme**: 09 Aralık 2024  
**Toplam Modül**: 10/10 Tamamlandı

---

## 📋 Hızlı Başlangıç

```bash
# Projeyi klonlayın
git clone https://github.com/baranakbulut/PremiumYol.git
cd PremiumYol/TrendyolClone

# Bağımlılıkları yükleyin
dotnet restore

# Projeyi çalıştırın
dotnet run
```

Tarayıcınızda https://localhost:5001 adresini açın.

**Varsayılan Admin**: admin@trendyol.com / admin123

---

## 📋 İçindekiler

- [Özellikler](#-özellikler)
- [Teknolojiler](#-teknolojiler)
- [Dokümantasyon](#-dokümantasyon)
- [Modül Durumu](#-modül-durumu)
- [Kurulum](#-kurulum)
- [Test](#-test)
- [Docker](#-docker)
- [İletişim](#-i̇letişim)
- [Lisans](#-lisans)

## ✨ Özellikler

### 🛒 E-Ticaret Özellikleri
- Gelişmiş ürün varyasyon sistemi (renk, beden, SKU)
- Akıllı sepet yönetimi ve kupon sistemi
- Marka ve kampanya yönetimi
- Gelişmiş arama motoru (otomatik tamamlama)
- Sipariş takibi ve kargo entegrasyonu
- Bildirim sistemi (Email, SMS)
- Yorum ve değerlendirme sistemi
- SEO optimizasyonu

### 👨‍💼 Yönetim Panelleri
- Admin dashboard (istatistikler, raporlar)
- Satıcı paneli (ürün, sipariş, finans)
- Kullanıcı profil yönetimi

### 🔒 Güvenlik
- BCrypt şifre hashleme
- Session yönetimi
- CSRF koruması
- Rate limiting
- Input validation

## 🛠 Teknolojiler

- **Backend**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 8.0
- **Veritabanı**: SQLite / SQL Server
- **Frontend**: Bootstrap 5, FontAwesome, Animate.css
- **Caching**: IMemoryCache
- **Containerization**: Docker

---

## 📚 Dokümantasyon

- [Kurulum Rehberi](docs/KURULUM.md) - Detaylı kurulum adımları
- [Modül Dokümantasyonu](docs/MODUL_DOKUMANTASYONU.md) - Tüm modüllerin detayları
- [Teknik Dokümantasyon](TrendyolClone/README.md) - API, deployment, güvenlik

---

## 📊 Modül Durumu

| Modül | Özellik | Durum |
|-------|---------|-------|
| 1 | Ürün Varyasyon Sistemi | ✅ %100 |
| 2 | Sepet & Kupon Sistemi | ✅ %100 |
| 3 | Marka & Kampanya | ✅ %100 |
| 4 | Arama Motoru | ✅ %100 |
| 5 | Sipariş Sistemi | ✅ %100 |
| 6 | Bildirim Sistemi | ✅ %100 |
| 7 | Admin Geliştirmeleri | ✅ %100 |
| 8 | Satıcı Paneli | ✅ %100 |
| 9 | Yorum & Değerlendirme | ✅ %100 |
| 10 | SEO & Marketing | ✅ %100 |

---

## 💻 Kurulum

Detaylı kurulum adımları için [Kurulum Rehberi](docs/KURULUM.md) sayfasını ziyaret edin.

### Hızlı Kurulum

```bash
# Projeyi klonlayın
git clone https://github.com/baranakbulut/PremiumYol.git
cd PremiumYol/TrendyolClone

# Bağımlılıkları yükleyin
dotnet restore

# Projeyi çalıştırın
dotnet run
```

Tarayıcınızda https://localhost:5001 adresini açın.

**Varsayılan Hesaplar**:
- Admin: admin@trendyol.com / admin123
- Test: test@test.com / test123

---

## 🧪 Test

```powershell
# Endpoint testleri
cd docs
.\test-endpoints.ps1

# Unit testler
cd TrendyolClone
dotnet test
```

---

## � Dockesr

```bash
# Docker ile çalıştırma
docker-compose up -d

# Tarayıcıda açın
http://localhost:8080
```

Detaylı Docker yapılandırması için [Teknik Dokümantasyon](TrendyolClone/README.md) sayfasını ziyaret edin.

---

## 📞 İletişim

- **Geliştirici**: Baran Akbulut
- **Telefon**: 0538 969 36 06
- **Instagram**: [@one.barann](https://instagram.com/one.barann)
- **GitHub**: https://github.com/baranakbulut/PremiumYol

---

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

---

## 🙏 Teşekkürler

ASP.NET Core, Entity Framework Core, Bootstrap ve tüm açık kaynak topluluğuna teşekkürler.

---

**⭐ Projeyi beğendiyseniz yıldız vermeyi unutmayın!**
