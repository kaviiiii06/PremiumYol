# Deployment Guide - PremiumYol

## 🚀 Deployment Seçenekleri

### 1. Docker ile Deployment

#### Gereksinimler
- Docker Desktop veya Docker Engine
- Docker Compose

#### Adımlar

1. **Docker Image Oluşturma**
```bash
docker build -t premiumyol:latest .
```

2. **Docker Compose ile Çalıştırma**
```bash
docker-compose up -d
```

3. **Logları İzleme**
```bash
docker-compose logs -f
```

4. **Durdurma**
```bash
docker-compose down
```

### 2. Windows Server Deployment

#### IIS Kurulumu

1. **IIS ve ASP.NET Core Hosting Bundle Kurulumu**
```powershell
# IIS'i etkinleştir
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment
Enable-WindowsOptionalFeature -Online -FeatureName IIS-NetFxExtensibility45
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HealthAndDiagnostics
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Security
Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestFiltering
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Performance
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerManagementTools
Enable-WindowsOptionalFeature -Online -FeatureName IIS-StaticContent
Enable-WindowsOptionalFeature -Online -FeatureName IIS-DefaultDocument
Enable-WindowsOptionalFeature -Online -FeatureName IIS-DirectoryBrowsing
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpCompressionStatic
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpCompressionDynamic

# ASP.NET Core Hosting Bundle indir ve kur
# https://dotnet.microsoft.com/download/dotnet/8.0
```

2. **Uygulamayı Publish Et**
```bash
dotnet publish -c Release -o C:\inetpub\wwwroot\premiumyol
```

3. **IIS'de Site Oluştur**
- IIS Manager'ı aç
- Sites > Add Website
- Site name: PremiumYol
- Physical path: C:\inetpub\wwwroot\premiumyol
- Binding: http, port 80 veya https, port 443

4. **Application Pool Ayarları**
- .NET CLR Version: No Managed Code
- Managed Pipeline Mode: Integrated
- Identity: ApplicationPoolIdentity

### 3. Linux Server Deployment (Ubuntu/Debian)

#### Nginx + Systemd

1. **.NET Runtime Kurulumu**
```bash
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-8.0
```

2. **Uygulamayı Kopyala**
```bash
sudo mkdir -p /var/www/premiumyol
sudo dotnet publish -c Release -o /var/www/premiumyol
```

3. **Systemd Service Oluştur**
```bash
sudo nano /etc/systemd/system/premiumyol.service
```

```ini
[Unit]
Description=PremiumYol E-Commerce Application
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

4. **Service'i Başlat**
```bash
sudo systemctl enable premiumyol.service
sudo systemctl start premiumyol.service
sudo systemctl status premiumyol.service
```

5. **Nginx Reverse Proxy**
```bash
sudo nano /etc/nginx/sites-available/premiumyol
```

```nginx
server {
    listen 80;
    server_name premiumyol.com www.premiumyol.com;
    
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
}
```

```bash
sudo ln -s /etc/nginx/sites-available/premiumyol /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

6. **SSL Sertifikası (Let's Encrypt)**
```bash
sudo apt-get install certbot python3-certbot-nginx
sudo certbot --nginx -d premiumyol.com -d www.premiumyol.com
```

### 4. Azure App Service Deployment

1. **Azure CLI ile Login**
```bash
az login
```

2. **Resource Group Oluştur**
```bash
az group create --name premiumyol-rg --location westeurope
```

3. **App Service Plan Oluştur**
```bash
az appservice plan create --name premiumyol-plan --resource-group premiumyol-rg --sku B1 --is-linux
```

4. **Web App Oluştur**
```bash
az webapp create --resource-group premiumyol-rg --plan premiumyol-plan --name premiumyol --runtime "DOTNETCORE:8.0"
```

5. **Deploy Et**
```bash
az webapp deployment source config-zip --resource-group premiumyol-rg --name premiumyol --src publish.zip
```

### 5. AWS EC2 Deployment

1. **EC2 Instance Oluştur**
- Ubuntu 22.04 LTS
- t2.micro veya daha büyük
- Security Group: HTTP (80), HTTPS (443), SSH (22)

2. **SSH ile Bağlan**
```bash
ssh -i your-key.pem ubuntu@your-ec2-ip
```

3. **Gerekli Paketleri Kur**
```bash
sudo apt update
sudo apt install -y nginx
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y aspnetcore-runtime-8.0
```

4. **Uygulamayı Deploy Et** (yukarıdaki Linux deployment adımlarını takip et)

## 🔒 Production Güvenlik Kontrol Listesi

- [ ] HTTPS zorunlu
- [ ] Güçlü şifreler kullan
- [ ] Environment variables ile hassas bilgileri sakla
- [ ] Rate limiting aktif
- [ ] CORS ayarları yapılandır
- [ ] Security headers ekle
- [ ] Database backup stratejisi oluştur
- [ ] Logging ve monitoring aktif
- [ ] Firewall kuralları yapılandır
- [ ] SSL sertifikası yükle
- [ ] API key'leri güvenli sakla
- [ ] Session timeout ayarla
- [ ] CSRF koruması aktif
- [ ] XSS koruması aktif
- [ ] SQL injection koruması (EF Core ile otomatik)

## 📊 Monitoring ve Logging

### Application Insights (Azure)
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

### Serilog
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

### Health Checks
Uygulama `/health` endpoint'inde health check sağlar:
```bash
curl http://localhost/health
```

## 🔄 Güncelleme Stratejisi

### Zero-Downtime Deployment

1. **Blue-Green Deployment**
- İki ayrı environment (blue ve green)
- Yeni versiyonu green'e deploy et
- Test et
- Traffic'i green'e yönlendir
- Blue'yu güncelle

2. **Rolling Update**
- Load balancer arkasında birden fazla instance
- Her instance'ı sırayla güncelle
- Health check ile kontrol et

## 📝 Environment Variables

Production'da şu environment variable'ları ayarla:

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="your-connection-string"
export Payment__Iyzico__ApiKey="your-api-key"
export Payment__Iyzico__SecretKey="your-secret-key"
export DropshippingAPIs__CJDropshipping__Email="your-email"
export DropshippingAPIs__CJDropshipping__Password="your-password"
```

## 🐛 Troubleshooting

### Uygulama Başlamıyor
```bash
# Logları kontrol et
journalctl -u premiumyol.service -n 50

# Port kullanımda mı?
sudo netstat -tulpn | grep :5000

# Dosya izinleri
sudo chown -R www-data:www-data /var/www/premiumyol
```

### Database Bağlantı Hatası
```bash
# Connection string'i kontrol et
# SQLite dosya izinlerini kontrol et
sudo chmod 644 /var/www/premiumyol/PremiumYol.db
```

### 502 Bad Gateway (Nginx)
```bash
# Uygulama çalışıyor mu?
sudo systemctl status premiumyol.service

# Nginx logları
sudo tail -f /var/log/nginx/error.log
```

## 📞 Destek

Deployment sorunları için:
- Email: baran@onewearr.shop
- Telefon: 0538 969 36 06
