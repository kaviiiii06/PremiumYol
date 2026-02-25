# PremiumYol Endpoint Test Script
Write-Host "=== PremiumYol Endpoint Test Başlatılıyor ===" -ForegroundColor Cyan

$baseUrl = "https://localhost:5001"
$errors = @()
$success = @()

# Test edilecek endpoint'ler
$endpoints = @(
    @{Path="/"; Name="Ana Sayfa"},
    @{Path="/Product"; Name="Ürün Listesi"},
    @{Path="/Product/Details/1"; Name="Ürün Detay"},
    @{Path="/Account/Login"; Name="Giriş Sayfası"},
    @{Path="/Account/Register"; Name="Kayıt Sayfası"},
    @{Path="/Cart"; Name="Sepet"},
    @{Path="/Search"; Name="Arama"},
    @{Path="/Admin"; Name="Admin Panel"},
    @{Path="/Seller"; Name="Satıcı Panel"},
    @{Path="/Review"; Name="Yorumlar"},
    @{Path="/Notification"; Name="Bildirimler"}
)

Write-Host "`nProje başlatılıyor..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

foreach ($endpoint in $endpoints) {
    $url = $baseUrl + $endpoint.Path
    Write-Host "`nTest: $($endpoint.Name) - $url" -ForegroundColor White
    
    try {
        # SSL sertifika hatalarını yoksay
        [System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
        
        $response = Invoke-WebRequest -Uri $url -Method GET -TimeoutSec 10 -UseBasicParsing -ErrorAction Stop
        
        if ($response.StatusCode -eq 200) {
            Write-Host "  ✓ Başarılı (200 OK)" -ForegroundColor Green
            $success += $endpoint.Name
        } elseif ($response.StatusCode -eq 302) {
            Write-Host "  ↻ Yönlendirme (302 Redirect)" -ForegroundColor Yellow
            $success += $endpoint.Name
        } else {
            Write-Host "  ! Beklenmeyen durum kodu: $($response.StatusCode)" -ForegroundColor Yellow
        }
    }
    catch {
        $errorMsg = $_.Exception.Message
        Write-Host "  ✗ HATA: $errorMsg" -ForegroundColor Red
        $errors += @{Endpoint=$endpoint.Name; Error=$errorMsg}
    }
}

# Özet
Write-Host "`n=== TEST SONUÇLARI ===" -ForegroundColor Cyan
Write-Host "Başarılı: $($success.Count)/$($endpoints.Count)" -ForegroundColor Green
Write-Host "Hatalı: $($errors.Count)/$($endpoints.Count)" -ForegroundColor $(if($errors.Count -eq 0){"Green"}else{"Red"})

if ($errors.Count -gt 0) {
    Write-Host "`n=== HATALAR ===" -ForegroundColor Red
    foreach ($error in $errors) {
        Write-Host "  • $($error.Endpoint): $($error.Error)" -ForegroundColor Red
    }
}

Write-Host "`n=== Test Tamamlandı ===" -ForegroundColor Cyan
