using Microsoft.AspNetCore.Mvc;
using TrendyolClone.Models;
using TrendyolClone.Services;

namespace TrendyolClone.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IBildirimService _bildirimService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            IBildirimService bildirimService,
            ILogger<NotificationController> logger)
        {
            _bildirimService = bildirimService;
            _logger = logger;
        }

        // Bildirim Tercihleri
        public async Task<IActionResult> Preferences()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var tercihler = await _bildirimService.TercihleriGetirAsync(kullaniciId.Value);
            return View(tercihler);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePreferences(BildirimTercihi tercih)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            tercih.KullaniciId = kullaniciId.Value;
            var sonuc = await _bildirimService.TercihleriGuncelleAsync(tercih);

            if (sonuc)
            {
                TempData["Mesaj"] = "Bildirim tercihleri güncellendi.";
            }
            else
            {
                TempData["Hata"] = "Tercihler güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Preferences));
        }

        // Bildirim Geçmişi
        public async Task<IActionResult> History(int sayfa = 1)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var bildirimler = await _bildirimService.KullaniciBildirimGecmisiAsync(kullaniciId.Value, sayfa);
            ViewBag.Sayfa = sayfa;
            
            return View(bildirimler);
        }

        // Admin: Bildirim Gönder
        [HttpGet]
        public IActionResult Send()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(string tur, string baslik, string icerik, string? hedefEmail = null)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                BildirimTuru bildirimTuru = tur.ToLower() switch
                {
                    "email" => BildirimTuru.Email,
                    "sms" => BildirimTuru.SMS,
                    _ => BildirimTuru.Email
                };

                int gonderilen = 0;

                if (!string.IsNullOrEmpty(hedefEmail))
                {
                    // Belirli bir email'e gönder
                    var sonuc = await _bildirimService.EmailGonderAsync(null, hedefEmail, baslik, icerik);
                    gonderilen = sonuc ? 1 : 0;
                }
                else
                {
                    // Tüm kullanıcılara gönder
                    gonderilen = await _bildirimService.TumKullanicilariaBildirimGonderAsync(baslik, icerik, bildirimTuru);
                }

                TempData["Mesaj"] = $"{gonderilen} kullanıcıya bildirim gönderildi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bildirim gönderme hatası");
                TempData["Hata"] = "Bildirim gönderilirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Send));
        }

        // Admin: Şablon Yönetimi
        public async Task<IActionResult> Templates()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            var sablonlar = await _bildirimService.TumSablonlariGetirAsync();
            return View(sablonlar);
        }

        [HttpGet]
        public IActionResult CreateTemplate()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate(BildirimSablonu sablon)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var sonuc = await _bildirimService.SablonOlusturAsync(sablon);
                if (sonuc)
                {
                    TempData["Mesaj"] = "Şablon oluşturuldu.";
                    return RedirectToAction(nameof(Templates));
                }
                else
                {
                    TempData["Hata"] = "Şablon oluşturulurken bir hata oluştu.";
                }
            }

            return View(sablon);
        }

        // Admin: İstatistikler
        public async Task<IActionResult> Statistics()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            var baslangic = DateTime.Now.AddDays(-30);
            var bitis = DateTime.Now;

            var istatistikler = await _bildirimService.BildirimIstatistikleriGetirAsync(baslangic, bitis);
            
            ViewBag.Baslangic = baslangic;
            ViewBag.Bitis = bitis;

            return View(istatistikler);
        }

        private bool IsAdmin()
        {
            var rolId = HttpContext.Session.GetInt32("RolId");
            return rolId == 1; // Admin rolü
        }
    }
}
