using Microsoft.AspNetCore.Mvc;
using TrendyolClone.Data;
using TrendyolClone.Models.DTOs;
using TrendyolClone.Services;
using Microsoft.EntityFrameworkCore;

namespace TrendyolClone.Controllers
{
    [Route("Admin/Report")]
    public class AdminReportController : Controller
    {
        private readonly IRaporService _raporService;
        private readonly UygulamaDbContext _context;
        private readonly ILogger<AdminReportController> _logger;

        public AdminReportController(
            IRaporService raporService,
            UygulamaDbContext context,
            ILogger<AdminReportController> logger)
        {
            _raporService = raporService;
            _context = context;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(DateTime? baslangic, DateTime? bitis)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var filtre = new RaporFiltre
            {
                BaslangicTarihi = baslangic ?? DateTime.Today.AddDays(-30),
                BitisTarihi = bitis ?? DateTime.Today
            };

            var ozet = await _raporService.GetGenelOzetAsync(filtre);
            var satisRaporu = await _raporService.GetSatisRaporuAsync(filtre);

            ViewBag.Filtre = filtre;
            ViewBag.SatisRaporu = satisRaporu;

            return View(ozet);
        }

        [HttpGet("Sales")]
        public async Task<IActionResult> Sales(DateTime? baslangic, DateTime? bitis)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var filtre = new RaporFiltre
            {
                BaslangicTarihi = baslangic ?? DateTime.Today.AddDays(-30),
                BitisTarihi = bitis ?? DateTime.Today
            };

            var satisRaporu = await _raporService.GetSatisRaporuAsync(filtre);
            ViewBag.Filtre = filtre;

            return View(satisRaporu);
        }

        [HttpGet("Products")]
        public async Task<IActionResult> Products(DateTime? baslangic, DateTime? bitis, int? kategoriId)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var filtre = new RaporFiltre
            {
                BaslangicTarihi = baslangic ?? DateTime.Today.AddDays(-30),
                BitisTarihi = bitis ?? DateTime.Today,
                KategoriId = kategoriId,
                Limit = 50
            };

            var urunRaporu = await _raporService.GetUrunSatisRaporuAsync(filtre);
            var kategoriler = await _context.Kategoriler.ToListAsync();

            ViewBag.Filtre = filtre;
            ViewBag.Kategoriler = kategoriler;

            return View(urunRaporu);
        }

        [HttpGet("Categories")]
        public async Task<IActionResult> Categories(DateTime? baslangic, DateTime? bitis)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var filtre = new RaporFiltre
            {
                BaslangicTarihi = baslangic ?? DateTime.Today.AddDays(-30),
                BitisTarihi = bitis ?? DateTime.Today
            };

            var kategoriRaporu = await _raporService.GetKategoriSatisRaporuAsync(filtre);
            ViewBag.Filtre = filtre;

            return View(kategoriRaporu);
        }

        [HttpGet("Users")]
        public async Task<IActionResult> Users(DateTime? baslangic, DateTime? bitis)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var filtre = new RaporFiltre
            {
                BaslangicTarihi = baslangic ?? DateTime.Today.AddDays(-7),
                BitisTarihi = bitis ?? DateTime.Today
            };

            var kullaniciRaporu = await _raporService.GetKullaniciRaporuAsync(filtre);
            ViewBag.Filtre = filtre;

            return View(kullaniciRaporu);
        }

        [HttpGet("Financial")]
        public async Task<IActionResult> Financial(DateTime? baslangic, DateTime? bitis)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var filtre = new RaporFiltre
            {
                BaslangicTarihi = baslangic ?? DateTime.Today.AddDays(-30),
                BitisTarihi = bitis ?? DateTime.Today
            };

            var finansalRapor = await _raporService.GetFinansalRaporAsync(filtre);
            ViewBag.Filtre = filtre;

            return View(finansalRapor);
        }

        // API Endpoints for Charts
        [HttpGet("api/sales")]
        public async Task<IActionResult> GetSalesData(DateTime? baslangic, DateTime? bitis)
        {
            if (!IsAdmin()) return Unauthorized();

            var filtre = new RaporFiltre
            {
                BaslangicTarihi = baslangic ?? DateTime.Today.AddDays(-30),
                BitisTarihi = bitis ?? DateTime.Today
            };

            var data = await _raporService.GetSatisRaporuAsync(filtre);
            return Json(data);
        }

        [HttpGet("api/categories")]
        public async Task<IActionResult> GetCategoriesData(DateTime? baslangic, DateTime? bitis)
        {
            if (!IsAdmin()) return Unauthorized();

            var filtre = new RaporFiltre
            {
                BaslangicTarihi = baslangic ?? DateTime.Today.AddDays(-30),
                BitisTarihi = bitis ?? DateTime.Today
            };

            var data = await _raporService.GetKategoriSatisRaporuAsync(filtre);
            return Json(data);
        }

        private bool IsAdmin()
        {
            var rolId = HttpContext.Session.GetInt32("RolId");
            return rolId == 1;
        }
    }
}
