# 🚀 Deployment Alternatifleri

Vercel .NET uygulamalarını desteklemiyor. İşte alternatif deployment seçenekleri:

---

## ✅ Önerilen: Azure App Service (Ücretsiz Tier)

### Avantajlar
- Microsoft'un resmi .NET hosting platformu
- Ücretsiz tier mevcut (F1)
- Kolay deployment
- Otomatik SSL
- GitHub entegrasyonu

### Kurulum Adımları

1. **Azure Hesabı Oluşturun**
   - https://azure.microsoft.com/free/ adresine gidin
   - Ücretsiz hesap oluşturun ($200 kredi + 12 ay ücretsiz servisler)

2. **Azure CLI Kurulumu**
   ```bash
   # Windows (PowerShell)
   winget install Microsoft.AzureCLI
   
   # Veya indirin: https://aka.ms/installazurecliwindows
   ```

3. **Azure'a Giriş Yapın**
   ```bash
   az login
   ```

4. **Resource Group Oluşturun**
   ```bash
   az group create --name PremiumYolRG --location eastus
   ```

5. **App Service Plan Oluşturun (Ücretsiz)**
   ```bash
   az appservice plan create --name PremiumYolPlan --resource-group PremiumYolRG --sku F1 --is-linux
   ```

6. **Web App Oluşturun**
   ```bash
   az webapp create --name premiumyol-app --resource-group PremiumYolRG --plan PremiumYolPlan --runtime "DOTNET|8.0"
   ```

7. **GitHub'dan Deploy Edin**
   ```bash
   az webapp deployment source config --name premiumyol-app --resource-group PremiumYolRG --repo-url https://github.com/kaviiiii06/PremiumYol --branch main --manual-integration
   ```

8. **Siteniz Hazır!**
   - URL: `https://premiumyol-app.azurewebsites.net`

---

## 🐳 Alternatif 1: Railway (Kolay ve Ücretsiz)

### Avantajlar
- Çok kolay deployment
- Ücretsiz tier ($5/ay kredi)
- Docker desteği
- Otomatik SSL

### Kurulum Adımları

1. **Railway Hesabı**
   - https://railway.app adresine gidin
   - GitHub ile giriş yapın

2. **Yeni Proje**
   - "New Project" → "Deploy from GitHub repo"
   - `kaviiiii06/PremiumYol` seçin

3. **Ayarlar**
   - Railway otomatik olarak Dockerfile'ı algılayacak
   - Deploy butonuna tıklayın

4. **Domain**
   - Settings → Generate Domain
   - Siteniz hazır!

---

## 🌊 Alternatif 2: Render (Ücretsiz)

### Avantajlar
- Tamamen ücretsiz tier
- Kolay kullanım
- Otomatik SSL
- GitHub entegrasyonu

### Kurulum Adımları

1. **Render Hesabı**
   - https://render.com adresine gidin
   - GitHub ile giriş yapın

2. **Yeni Web Service**
   - "New" → "Web Service"
   - GitHub repository'nizi bağlayın
   - `kaviiiii06/PremiumYol` seçin

3. **Ayarlar**
   - **Name**: premiumyol
   - **Environment**: Docker
   - **Plan**: Free
   - "Create Web Service" tıklayın

4. **Deploy**
   - Otomatik deploy başlayacak
   - 5-10 dakika bekleyin
   - Siteniz hazır!

---

## 🔵 Alternatif 3: DigitalOcean App Platform

### Avantajlar
- $200 ücretsiz kredi (60 gün)
- Güçlü altyapı
- Kolay ölçeklendirme

### Kurulum Adımları

1. **DigitalOcean Hesabı**
   - https://www.digitalocean.com adresine gidin
   - Hesap oluşturun ($200 kredi alın)

2. **App Platform**
   - "Create" → "Apps"
   - GitHub repository'nizi seçin

3. **Ayarlar**
   - Dockerfile'ı algılayacak
   - Basic plan seçin ($5/ay - krediyle ücretsiz)
   - Deploy edin

---

## 📊 Karşılaştırma

| Platform | Ücretsiz Tier | Kolay Kurulum | .NET Desteği | SSL |
|----------|---------------|---------------|--------------|-----|
| Azure App Service | ✅ F1 Tier | ⭐⭐⭐⭐ | ✅ Native | ✅ |
| Railway | ✅ $5/ay kredi | ⭐⭐⭐⭐⭐ | ✅ Docker | ✅ |
| Render | ✅ Tamamen ücretsiz | ⭐⭐⭐⭐⭐ | ✅ Docker | ✅ |
| DigitalOcean | ✅ $200 kredi | ⭐⭐⭐⭐ | ✅ Docker | ✅ |

---

## 🎯 Önerim

**Başlangıç için**: Railway veya Render (en kolay)
**Profesyonel kullanım için**: Azure App Service (en güçlü)

---

## 🚀 Hızlı Başlangıç: Railway

1. https://railway.app adresine gidin
2. "Start a New Project" tıklayın
3. "Deploy from GitHub repo" seçin
4. Repository'nizi seçin
5. Deploy butonuna tıklayın
6. 5 dakika bekleyin
7. Siteniz hazır! 🎉

---

**Hazırlayan**: Baran Akbulut  
**Tarih**: 09 Aralık 2024
