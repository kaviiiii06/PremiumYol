using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrendyolClone.Services;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IYorumService _yorumService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IYorumService yorumService, ILogger<ReviewController> logger)
        {
            _yorumService = yorumService;
            _logger = logger;
        }

        // Ürün yorumlarını listele
        public async Task<IActionResult> Index(int urunId, string siralama = "Tarih", int sayfa = 1)
        {
            try
            {
                var yorumlar = await _yorumService.UrunYorumlariniGetirAsync(urunId, siralama, sayfa, 10);
                ViewBag.UrunId = urunId;
                ViewBag.Siralama = siralama;
                ViewBag.Sayfa = sayfa;
                return View(yorumlar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yorumlar yüklenirken hata");
                return View("Error");
            }
        }

        // Yorum yazma formu
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create(int urunId, int siparisId)
        {
            var kullaniciId = User.Identity?.Name ?? "";
            var yapilabilirMi = await _yorumService.YorumYapilabilirMiAsync(urunId, kullaniciId);
            
            if (!yapilabilirMi)
            {
                TempData["Error"] = "Bu ürün için yorum yapamazsınız.";
                return RedirectToAction("Details", "Product", new { id = urunId });
            }

            ViewBag.UrunId = urunId;
            ViewBag.SiparisId = siparisId;
            return View();
        }

        // Yorum kaydet
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(YorumEkleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                dto.KullaniciId = User.Identity?.Name ?? "";
                var sonuc = await _yorumService.YorumEkleAsync(dto);
                
                if (sonuc)
                {
                    TempData["Success"] = "Yorumunuz başarıyla kaydedildi.";
                    return RedirectToAction("Details", "Product", new { id = dto.UrunId });
                }
                
                ModelState.AddModelError("", "Yorum eklenirken hata oluştu.");
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yorum eklenirken hata");
                ModelState.AddModelError("", "Bir hata oluştu.");
                return View(dto);
            }
        }

        // Yorumu faydalı bul
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MarkHelpful(int yorumId)
        {
            try
            {
                var kullaniciId = User.Identity?.Name ?? "";
                await _yorumService.YorumFaydaliIsaretle(yorumId, kullaniciId, true);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faydalı işaretlenirken hata");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Yorumu faydasız bul
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MarkUnhelpful(int yorumId)
        {
            try
            {
                var kullaniciId = User.Identity?.Name ?? "";
                await _yorumService.YorumFaydaliIsaretle(yorumId, kullaniciId, false);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faydasız işaretlenirken hata");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Yorum rapor et
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Report(int yorumId, string sebep, string aciklama)
        {
            try
            {
                var kullaniciId = User.Identity?.Name ?? "";
                await _yorumService.YorumRaporEtAsync(yorumId, kullaniciId, sebep, aciklama);
                return Json(new { success = true, message = "Rapor gönderildi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rapor gönderilirken hata");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Kullanıcının yorumları
        [Authorize]
        public async Task<IActionResult> MyReviews(int sayfa = 1)
        {
            try
            {
                var kullaniciId = User.Identity?.Name ?? "";
                var yorumlar = await _yorumService.KullaniciYorumlariniGetirAsync(kullaniciId, sayfa, 10);
                ViewBag.Sayfa = sayfa;
                return View(yorumlar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı yorumları yüklenirken hata");
                return View("Error");
            }
        }

        // Yorum sil
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int yorumId)
        {
            try
            {
                var kullaniciId = User.Identity?.Name ?? "";
                var sonuc = await _yorumService.YorumSilAsync(yorumId, kullaniciId);
                
                if (sonuc)
                {
                    return Json(new { success = true, message = "Yorum silindi." });
                }
                
                return Json(new { success = false, message = "Yorum silinemedi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yorum silinirken hata");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
