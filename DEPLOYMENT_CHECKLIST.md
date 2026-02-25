# ✅ Production Deployment Checklist

Bu checklist'i Vercel'e deploy etmeden önce kontrol edin.

---

## 🔍 Kod Kontrolü

- [x] Test verileri temizlendi
- [x] Test kullanıcıları kaldırıldı
- [x] Gereksiz log dosyaları silindi
- [x] Gereksiz dokümantasyon dosyaları temizlendi
- [x] .gitignore güncel
- [x] Veritabanı dosyası (.db) silindi

---

## 🔒 Güvenlik

- [ ] Admin şifresi güçlü ve güvenli
- [ ] API anahtarları environment variables'da
- [ ] HTTPS zorunlu (appsettings.Production.json)
- [ ] CORS ayarları yapılandırıldı
- [ ] Rate limiting aktif
- [ ] Hassas bilgiler koddan kaldırıldı

---

## ⚙️ Yapılandırma

- [x] appsettings.Production.json oluşturuldu
- [x] vercel.json yapılandırıldı
- [x] .vercelignore oluşturuldu
- [ ] Environment variables hazır
- [ ] Veritabanı connection string hazır

---

## 📦 Build

- [ ] Lokal build testi yapıldı
  ```bash
  cd TrendyolClone
  dotnet build -c Release
  ```
- [ ] Publish testi yapıldı
  ```bash
  dotnet publish -c Release -o ./publish
  ```
- [ ] Hata yok

---

## 🗄️ Veritabanı

- [ ] Production veritabanı seçildi (PostgreSQL/Supabase)
- [ ] Connection string alındı
- [ ] Migration'lar hazır
- [ ] Seed data (roller, kategoriler) hazır

---

## 🚀 Vercel

- [ ] Vercel hesabı oluşturuldu
- [ ] GitHub repository bağlandı
- [ ] Build ayarları yapılandırıldı
- [ ] Environment variables eklendi
- [ ] Custom domain (opsiyonel) ayarlandı

---

## ✅ Deployment Sonrası

- [ ] Site açılıyor
- [ ] Admin paneline giriş yapılabiliyor
- [ ] Kategoriler yüklendi
- [ ] Roller oluşturuldu
- [ ] Kayıt olma çalışıyor
- [ ] Giriş yapma çalışıyor
- [ ] Ürün ekleme çalışıyor

---

## 📝 Notlar

### Admin Bilgileri
- Kullanıcı: `baranAdmin2025`
- Şifre: `Baran@2025!Secure`
- Email: `admin@premiumyol.com`

### İletişim
- Email: baran@onewearr.shop
- Telefon: 0538 969 36 06

---

## 🔄 Deployment Komutu

```bash
# 1. Son değişiklikleri commit edin
git add .
git commit -m "Production ready for Vercel deployment"

# 2. GitHub'a push edin
git push origin main

# 3. Vercel otomatik deploy edecek
# Dashboard'dan takip edin: https://vercel.com/dashboard
```

---

**Hazırlayan**: Baran Akbulut  
**Tarih**: 09 Aralık 2024  
**Durum**: ✅ Production Ready
