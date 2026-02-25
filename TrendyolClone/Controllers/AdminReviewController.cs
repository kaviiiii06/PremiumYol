using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrendyolClone.Services;

namespace TrendyolClone.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminReviewController : Controller
    {
        private readonly IYorumService _yorumService;
        private readonly ILogger<AdminReviewController> _logger;

        public AdminReviewController(IYorumService yorumService, ILogger<AdminReviewController> logger)
        {
            _yorumService = yorumService;
            _logger = logger;
        }

        // Yorum yönetim paneli
        public async Task<IActionResult> Index(string durum = "Bekleyen", int sayfa = 1)
        {
            try
            {
                var yorumlar = await _yorumService.AdminYorumListesiAsync(durum, sayfa, 20);
                ViewBag.Durum = durum;
                ViewBag.Sayfa = sayfa;
                return View(yorumlar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin yorum listesi yüklenirken hata");
                return View("Error");
            }
        }

        // Yorumu onayla
        [HttpPost]
        public async Task<IActionResult> Approve(int yorumId)
        {
            try
            {
                await _yorumService.YorumOnaylaAsync(yorumId);
                return Json(new { success = true, message = "Yorum onaylandı." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yorum onaylanırken hata");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Yorumu reddet
        [HttpPost]
        public async Task<IActionResult> Reject(int yorumId, string sebep)
        {
            try
            {
                await _yorumService.YorumReddetAsync(yorumId, sebep);
                return Json(new { success = true, message = "Yorum reddedildi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yorum reddedilirken hata");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Yorumu sil
        [HttpPost]
        public async Task<IActionResult> Delete(int yorumId)
        {
            try
            {
                await _yorumService.AdminYorumSilAsync(yorumId);
                return Json(new { success = true, message = "Yorum silindi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yorum silinirken hata");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Raporlar
        public async Task<IActionResult> Reports(string durum = "Bekleyen", int sayfa = 1)
        {
            try
            {
                var raporlar = await _yorumService.YorumRaporlariniGetirAsync(durum, sayfa, 20);
                ViewBag.Durum = durum;
                ViewBag.Sayfa = sayfa;
                return View(raporlar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Raporlar yüklenirken hata");
                return View("Error");
            }
        }

        // Raporu işle
        [HttpPost]
        public async Task<IActionResult> ProcessReport(int raporId, string karar, string aciklama)
        {
            try
            {
                await _yorumService.RaporIsleAsync(raporId, karar, aciklama);
                return Json(new { success = true, message = "Rapor işlendi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rapor işlenirken hata");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // İstatistikler
        public async Task<IActionResult> Statistics()
        {
            try
            {
                var istatistikler = await _yorumService.YorumIstatistikleriAsync();
                return View(istatistikler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstatistikler yüklenirken hata");
                return View("Error");
            }
        }
    }
}
