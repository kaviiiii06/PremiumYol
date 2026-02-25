using Microsoft.AspNetCore.Mvc;
using TrendyolClone.Services;

namespace TrendyolClone.Controllers
{
    [Route("Admin/Dashboard")]
    public class AdminDashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<AdminDashboardController> _logger;

        public AdminDashboardController(
            IDashboardService dashboardService,
            ILogger<AdminDashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var dashboardData = await _dashboardService.GetDashboardDataAsync();
                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard yüklenirken hata oluştu");
                TempData["Hata"] = "Dashboard yüklenirken bir hata oluştu.";
                return View();
            }
        }

        [HttpGet("Istatistikler")]
        public async Task<IActionResult> GetIstatistikler()
        {
            if (!IsAdmin())
            {
                return Unauthorized();
            }

            try
            {
                var istatistikler = await _dashboardService.GetIstatistiklerAsync();
                return Json(istatistikler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstatistikler yüklenirken hata oluştu");
                return StatusCode(500, "İstatistikler yüklenirken bir hata oluştu.");
            }
        }

        [HttpGet("GunlukSatislar")]
        public async Task<IActionResult> GetGunlukSatislar(int gunSayisi = 30)
        {
            if (!IsAdmin())
            {
                return Unauthorized();
            }

            try
            {
                var satislar = await _dashboardService.GetGunlukSatislarAsync(gunSayisi);
                return Json(satislar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Günlük satışlar yüklenirken hata oluştu");
                return StatusCode(500, "Günlük satışlar yüklenirken bir hata oluştu.");
            }
        }

        [HttpGet("PopulerUrunler")]
        public async Task<IActionResult> GetPopulerUrunler(int adet = 10)
        {
            if (!IsAdmin())
            {
                return Unauthorized();
            }

            try
            {
                var urunler = await _dashboardService.GetPopulerUrunlerAsync(adet);
                return Json(urunler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Popüler ürünler yüklenirken hata oluştu");
                return StatusCode(500, "Popüler ürünler yüklenirken bir hata oluştu.");
            }
        }

        private bool IsAdmin()
        {
            var rolId = HttpContext.Session.GetInt32("RolId");
            return rolId == 1; // Admin rolü
        }
    }
}
