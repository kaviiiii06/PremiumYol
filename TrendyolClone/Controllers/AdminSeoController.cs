using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models.DTOs;
using TrendyolClone.Services;

namespace TrendyolClone.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSeoController : Controller
    {
        private readonly ISeoService _seoService;
        private readonly UygulamaDbContext _context;

        public AdminSeoController(ISeoService seoService, UygulamaDbContext context)
        {
            _seoService = seoService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Products()
        {
            var urunler = await _context.Urunler
                .Include(u => u.Kategori)
                .OrderByDescending(u => u.OlusturmaTarihi)
                .Take(100)
                .ToListAsync();

            return View(urunler);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null)
                return NotFound();

            var seoMeta = await _seoService.GetSeoMetaAsync("Product", id);
            ViewBag.Urun = urun;
            return View(seoMeta);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, SeoMetaDto seoMeta)
        {
            if (!ModelState.IsValid)
            {
                var urun = await _context.Urunler.FindAsync(id);
                ViewBag.Urun = urun;
                return View(seoMeta);
            }

            await _seoService.UpdateSeoMetaAsync("Product", id, seoMeta);
            TempData["Success"] = "SEO ayarları güncellendi.";
            return RedirectToAction(nameof(Products));
        }

        [HttpPost]
        public async Task<IActionResult> GenerateProductSeo(int id)
        {
            var seoMeta = await _seoService.GenerateProductSeoAsync(id);
            await _seoService.UpdateSeoMetaAsync("Product", id, seoMeta);
            
            return Json(new { success = true, message = "SEO otomatik oluşturuldu" });
        }

        [HttpPost]
        public IActionResult GenerateSeoUrl([FromBody] string text)
        {
            var url = _seoService.GenerateSeoFriendlyUrl(text);
            return Json(new { url });
        }
    }
}
