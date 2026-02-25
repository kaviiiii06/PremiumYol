# 🌊 Render Deployment Rehberi

Render ile projenizi ücretsiz olarak canlıya alın!

---

## ✅ Adım 1: Render Hesabı Oluşturun

1. https://render.com adresine gidin
2. Sağ üstteki "Get Started" butonuna tıklayın
3. "Sign up with GitHub" seçin
4. GitHub hesabınızla giriş yapın ve yetkilendirin

---

## 🚀 Adım 2: Yeni Web Service Oluşturun

1. Render dashboard'da "New +" butonuna tıklayın
2. "Web Service" seçin
3. GitHub repository'nizi bağlayın:
   - "Connect account" tıklayın (ilk kez ise)
   - `kaviiiii06/PremiumYol` repository'sini seçin
   - "Connect" butonuna tıklayın

---

## ⚙️ Adım 3: Service Ayarları

Aşağıdaki ayarları yapın:

### Temel Ayarlar
- **Name**: `premiumyol` (veya istediğiniz isim)
- **Region**: `Frankfurt (EU Central)` (Türkiye'ye en yakın)
- **Branch**: `main`
- **Root Directory**: Boş bırakın

### Build Ayarları
- **Runtime**: `Docker`
- **Dockerfile Path**: `TrendyolClone/Dockerfile`

### Plan
- **Instance Type**: `Free` seçin
  - 512 MB RAM
  - 0.1 CPU
  - Tamamen ücretsiz!

---

## 🔧 Adım 4: Environment Variables (Opsiyonel)

"Advanced" butonuna tıklayın ve environment variables ekleyin:

```
ASPNETCORE_ENVIRONMENT=Production
```

---

## 🎯 Adım 5: Deploy!

1. "Create Web Service" butonuna tıklayın
2. Render otomatik olarak:
   - Dockerfile'ı algılayacak
   - Docker image oluşturacak
   - Container'ı başlatacak
3. İlk deployment 5-10 dakika sürebilir

---

## 📊 Deployment Takibi

Deploy sırasında:
- **Logs** sekmesinden build loglarını görebilirsiniz
- **Events** sekmesinden deployment durumunu takip edebilirsiniz

### Beklenen Log Çıktısı
```
==> Cloning from https://github.com/kaviiiii06/PremiumYol...
==> Building...
==> Building Docker image...
==> Pushing image...
==> Starting service...
==> Your service is live 🎉
```

---

## 🌐 Adım 6: Domain Alın

Deployment tamamlandıktan sonra:

1. Service sayfanızda üstte URL göreceksiniz
2. Örnek: `https://premiumyol.onrender.com`
3. Bu URL'i kopyalayın ve tarayıcıda açın

### Custom Domain (Opsiyonel)
Kendi domain'inizi bağlamak için:
1. Settings → Custom Domain
2. Domain'inizi ekleyin
3. DNS ayarlarını yapın

---

## ✅ Adım 7: Test Edin

Siteniz açıldıktan sonra:

1. Ana sayfayı kontrol edin: `https://premiumyol.onrender.com`
2. Admin paneline gidin: `https://premiumyol.onrender.com/Admin`
3. Giriş yapın:
   - Kullanıcı: `baranAdmin2025`
   - Şifre: `Baran@2025!Secure`

---

## 🔄 Otomatik Deployment

Artık her `git push` yaptığınızda:
- Render otomatik olarak yeni versiyonu algılayacak
- Yeniden build edecek
- Otomatik deploy edecek

```bash
# Kod değişikliği yaptıktan sonra
git add .
git commit -m "Update: açıklama"
git push origin main

# Render otomatik deploy edecek!
```

---

## 🐛 Sorun Giderme

### 1. Build Hatası

**Sorun**: Docker build başarısız
```
Error: failed to solve: failed to compute cache key
```

**Çözüm**: Dockerfile'ı kontrol edin
```bash
# Lokal olarak test edin
cd TrendyolClone
docker build -t test .
```

### 2. Service Başlamıyor

**Sorun**: Container başlatılamıyor
```
Error: Application failed to start
```

**Çözüm**: Logs sekmesinden detaylı hata mesajını görün
- Port ayarlarını kontrol edin (Render otomatik PORT atar)
- Environment variables'ı kontrol edin

### 3. 502 Bad Gateway

**Sorun**: Service çalışıyor ama erişilemiyor

**Çözüm**: 
- Uygulamanın `0.0.0.0` adresini dinlediğinden emin olun
- `Program.cs` içinde:
```csharp
builder.WebHost.UseUrls("http://0.0.0.0:5000");
```

### 4. Veritabanı Hatası

**Sorun**: SQLite dosyası oluşturulamıyor

**Çözüm**: Render'da dosya sistemi geçicidir
- PostgreSQL kullanın (Render ücretsiz PostgreSQL sağlar)
- Veya SQLite'ı `/tmp` klasöründe kullanın

---

## 💾 Veritabanı (PostgreSQL)

Render ücretsiz PostgreSQL sağlar:

### 1. PostgreSQL Oluşturun
1. Dashboard → "New +" → "PostgreSQL"
2. Name: `premiumyol-db`
3. Plan: Free
4. "Create Database" tıklayın

### 2. Connection String Alın
1. Database sayfasında "Internal Database URL" kopyalayın
2. Web Service'inizde Environment Variables'a ekleyin:
```
ConnectionStrings__DefaultConnection=<INTERNAL_DATABASE_URL>
```

### 3. Migration Çalıştırın
İlk deployment'ta migration otomatik çalışacak.

---

## 📈 Monitoring

Render dashboard'da:
- **Metrics**: CPU, Memory, Network kullanımı
- **Logs**: Uygulama logları
- **Events**: Deployment geçmişi

---

## 💰 Ücretsiz Tier Limitleri

- **RAM**: 512 MB
- **CPU**: 0.1 CPU
- **Bandwidth**: Sınırsız
- **Build Time**: 500 saat/ay
- **Inactivity**: 15 dakika hareketsizlikten sonra uyur
  - İlk istek 30 saniye sürebilir (cold start)

---

## 🚀 Production İyileştirmeleri

### 1. Keep-Alive (Uyumayı Önle)
Ücretsiz plan'da servis 15 dakika sonra uyur. Önlemek için:
- Cron job ile her 10 dakikada ping atın
- Veya paid plan'a geçin ($7/ay)

### 2. Custom Domain
```
1. Settings → Custom Domain
2. Domain ekleyin: premiumyol.com
3. DNS ayarları:
   - Type: CNAME
   - Name: @
   - Value: premiumyol.onrender.com
```

### 3. SSL
Render otomatik SSL sağlar (Let's Encrypt)
- Hiçbir şey yapmanıza gerek yok!

---

## 📞 Destek

Sorun yaşarsanız:
- Render Docs: https://render.com/docs
- Render Community: https://community.render.com
- Email: baran@onewearr.shop

---

## ✅ Checklist

- [ ] Render hesabı oluşturuldu
- [ ] GitHub repository bağlandı
- [ ] Web Service oluşturuldu
- [ ] Docker runtime seçildi
- [ ] Free plan seçildi
- [ ] Deploy başlatıldı
- [ ] Site açıldı
- [ ] Admin paneline giriş yapıldı
- [ ] Test edildi

---

**Hazırlayan**: Baran Akbulut  
**Tarih**: 09 Aralık 2024  
**Durum**: ✅ Production Ready

---

## 🎉 Tebrikler!

Siteniz artık canlıda! 🚀

**URL**: https://premiumyol.onrender.com  
**Admin**: https://premiumyol.onrender.com/Admin

Başarılar! 🎊
