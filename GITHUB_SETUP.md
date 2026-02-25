# 🚀 GitHub ve Vercel Deployment Rehberi

## 📝 Adım 1: GitHub Repository Oluşturun

1. [GitHub](https://github.com) sitesine gidin
2. Sağ üstteki "+" butonuna tıklayın
3. "New repository" seçin
4. Repository bilgilerini girin:
   - **Repository name**: `PremiumYol` (veya istediğiniz isim)
   - **Description**: `Modern E-Ticaret Platformu - ASP.NET Core 8.0`
   - **Visibility**: Public veya Private
   - **Initialize**: HAYIR (boş bırakın, zaten kodunuz var)
5. "Create repository" butonuna tıklayın

---

## 📤 Adım 2: GitHub'a Push Edin

Repository oluşturduktan sonra, GitHub size bir URL verecek. Örnek:
```
https://github.com/baranakbulut/PremiumYol.git
```

Aşağıdaki komutları çalıştırın (URL'i kendi repository URL'inizle değiştirin):

```bash
# Remote ekleyin
git remote add origin https://github.com/KULLANICI_ADINIZ/PremiumYol.git

# Branch'i main olarak ayarlayın
git branch -M main

# Push edin
git push -u origin main
```

---

## 🚀 Adım 3: Vercel'e Deploy Edin

### 3.1. Vercel Hesabı
1. [Vercel](https://vercel.com) sitesine gidin
2. "Sign Up" butonuna tıklayın
3. "Continue with GitHub" seçin
4. GitHub hesabınızla giriş yapın

### 3.2. Yeni Proje Oluşturun
1. Vercel dashboard'da "Add New..." → "Project" tıklayın
2. GitHub repository'nizi seçin (PremiumYol)
3. "Import" butonuna tıklayın

### 3.3. Build Ayarları
Vercel otomatik olarak .NET projesini algılayacak, ancak manuel ayarlar:

- **Framework Preset**: Other
- **Build Command**: 
  ```bash
  cd TrendyolClone && dotnet publish -c Release -o bin/Release/net8.0/publish
  ```
- **Output Directory**: `TrendyolClone/bin/Release/net8.0/publish`
- **Install Command**: Boş bırakın

### 3.4. Environment Variables
"Environment Variables" bölümünde ekleyin:

```
ASPNETCORE_ENVIRONMENT=Production
```

### 3.5. Deploy
1. "Deploy" butonuna tıklayın
2. 2-3 dakika bekleyin
3. Deployment tamamlandığında URL'iniz hazır!

---

## ✅ Adım 4: Test Edin

Deployment tamamlandıktan sonra:

1. Vercel'in verdiği URL'i açın (örn: `https://premiumyol.vercel.app`)
2. Admin paneline gidin: `/Admin`
3. Giriş yapın:
   - Kullanıcı: `baranAdmin2025`
   - Şifre: `Baran@2025!Secure`

---

## 🔄 Adım 5: Güncellemeler

Kod değişikliği yaptığınızda:

```bash
git add .
git commit -m "Update: açıklama"
git push origin main
```

Vercel otomatik olarak yeni versiyonu deploy edecek!

---

## 🐛 Sorun Giderme

### Git Push Hatası
```bash
# Eğer authentication hatası alırsanız:
# 1. GitHub Personal Access Token oluşturun
# 2. Settings → Developer settings → Personal access tokens
# 3. "Generate new token" → repo yetkisi verin
# 4. Token'ı kopyalayın
# 5. Push ederken şifre yerine token'ı kullanın
```

### Vercel Build Hatası
1. Vercel dashboard'da "Deployments" sekmesine gidin
2. Başarısız deployment'a tıklayın
3. "Build Logs" sekmesinden hataları görün
4. Hatayı düzeltin ve tekrar push edin

---

## 📞 Yardım

Sorun yaşarsanız:
- Email: baran@onewearr.shop
- GitHub Issues: Repository'nizde issue açın
- Vercel Support: https://vercel.com/support

---

**Hazırlayan**: Baran Akbulut  
**Tarih**: 09 Aralık 2024
