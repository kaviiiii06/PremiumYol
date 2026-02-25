# 🚀 Vercel Deployment Rehberi

Bu dokümanda PremiumYol projesinin Vercel'e nasıl deploy edileceği anlatılmaktadır.

---

## 📋 Ön Hazırlık

### 1. Vercel Hesabı Oluşturun
- [Vercel](https://vercel.com) sitesine gidin
- GitHub hesabınızla giriş yapın

### 2. Projeyi GitHub'a Yükleyin
```bash
git add .
git commit -m "Production ready - Vercel deployment"
git push origin main
```

---

## 🔧 Vercel Yapılandırması

### 1. Yeni Proje Oluşturun
1. Vercel dashboard'a gidin
2. "New Project" butonuna tıklayın
3. GitHub repository'nizi seçin
4. "Import" butonuna tıklayın

### 2. Build Ayarları
- **Framework Preset**: Other
- **Build Command**: `cd TrendyolClone && dotnet publish -c Release -o bin/Release/net8.0/publish`
- **Output Directory**: `TrendyolClone/bin/Release/net8.0/publish`
- **Install Command**: Boş bırakın

### 3. Environment Variables
Aşağıdaki environment variable'ları ekleyin:

```
ASPNETCORE_ENVIRONMENT=Production
```

---

## 🗄️ Veritabanı Yapılandırması

### SQLite (Geliştirme/Test)
- Vercel'de SQLite dosya sistemi sınırlamaları nedeniyle önerilmez
- Geliştirme amaçlı kullanılabilir

### PostgreSQL (Önerilen)
1. [Vercel Postgres](https://vercel.com/docs/storage/vercel-postgres) oluşturun
2. Connection string'i environment variable olarak ekleyin:
```
ConnectionStrings__DefaultConnection=YOUR_POSTGRES_CONNECTION_STRING
```

### Supabase (Alternatif)
1. [Supabase](https://supabase.com) hesabı oluşturun
2. Yeni proje oluşturun
3. Connection string'i alın
4. Vercel'de environment variable olarak ekleyin

---

## 🚀 Deployment

### İlk Deployment
1. Vercel dashboard'da "Deploy" butonuna tıklayın
2. Build işleminin tamamlanmasını bekleyin
3. Deployment URL'ini alın

### Otomatik Deployment
- Her `git push` işleminde otomatik deploy edilir
- Production branch: `main`
- Preview deployments: Diğer branch'ler

---

## ✅ Deployment Sonrası Kontroller

### 1. Admin Hesabı
- URL: `https://your-app.vercel.app/Admin`
- Kullanıcı: `baranAdmin2025`
- Şifre: `Baran@2025!Secure`

### 2. Test Endpoint'leri
```bash
# Ana sayfa
curl https://your-app.vercel.app/

# Health check
curl https://your-app.vercel.app/health

# API test
curl https://your-app.vercel.app/api/products
```

### 3. Veritabanı Kontrolü
- Admin panelden kategorilerin yüklendiğini kontrol edin
- Rollerin oluşturulduğunu kontrol edin

---

## 🔒 Güvenlik

### 1. Environment Variables
Hassas bilgileri asla kod içinde tutmayın:
- API anahtarları
- Veritabanı şifreleri
- Secret key'ler

### 2. HTTPS
- Vercel otomatik olarak HTTPS sağlar
- Custom domain için SSL sertifikası otomatik oluşturulur

### 3. Rate Limiting
- Production'da rate limiting aktif
- `appsettings.Production.json` içinde yapılandırılmış

---

## 🐛 Sorun Giderme

### Build Hatası
```bash
# Lokal olarak test edin
cd TrendyolClone
dotnet publish -c Release

# Hata varsa düzeltin ve tekrar push edin
```

### Runtime Hatası
1. Vercel dashboard'da "Logs" sekmesine gidin
2. Hata mesajlarını inceleyin
3. Environment variables'ı kontrol edin

### Veritabanı Bağlantı Hatası
1. Connection string'i kontrol edin
2. Veritabanı servisinin aktif olduğundan emin olun
3. Firewall kurallarını kontrol edin

---

## 📊 Monitoring

### Vercel Analytics
- Otomatik olarak aktif
- Dashboard'dan erişilebilir

### Custom Monitoring
- Application Insights (opsiyonel)
- Sentry (opsiyonel)
- LogRocket (opsiyonel)

---

## 🔄 Güncelleme

### Kod Güncellemesi
```bash
git add .
git commit -m "Update: feature description"
git push origin main
```

### Veritabanı Migration
```bash
# Lokal olarak migration oluşturun
dotnet ef migrations add MigrationName

# Push edin
git push origin main

# Vercel'de otomatik çalışacak
```

---

## 💰 Maliyet

### Vercel Free Tier
- 100 GB bandwidth
- Serverless function execution
- Otomatik HTTPS
- Preview deployments

### Vercel Pro ($20/ay)
- Daha fazla bandwidth
- Daha fazla build time
- Team collaboration
- Advanced analytics

---

## 📞 Destek

Sorun yaşarsanız:
- [Vercel Documentation](https://vercel.com/docs)
- [Vercel Community](https://github.com/vercel/vercel/discussions)
- Proje sahibi: baran@onewearr.shop

---

**Son Güncelleme**: 09 Aralık 2024
