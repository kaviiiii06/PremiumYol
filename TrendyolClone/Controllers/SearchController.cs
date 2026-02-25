using Microsoft.AspNetCore.Mvc;
using TrendyolClone.Services;
using System.Security.Claims;

namespace TrendyolClone.Controllers
{
    public class SearchController : Controller
    {
        private readonly IAramaService _aramaService;
        private readonly ILogger<SearchController> _logger;

        public SearchController(IAramaService aramaService, ILogger<SearchController> logger)
        {
            _aramaService = aramaService;
            _logger = logger;
        }

        private int GetKullaniciId()
        {
            var kullaniciIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(kullaniciIdClaim, out int id) ? id : 0;
        }

        // GET: /Search
        public async Task<IActionResult> Index(string q, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return View("Empty");
            }

            var kullaniciId = GetKullaniciId();
            var sonuc = await _aramaService.AramaYapAsync(q, kullaniciId, page);

            ViewBag.SearchTerm = q;
            return View(sonuc);
        }

        // GET: /Search/Autocomplete
        [HttpGet]
        public async Task<IActionResult> Autocomplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }

            var kullaniciId = GetKullaniciId();
            var sonuclar = await _aramaService.OtomatikTamamlamaGetirAsync(term, kullaniciId);

            return Json(sonuclar);
        }

        // GET: /Search/History
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == 0)
            {
                return Json(new List<string>());
            }

            var gecmis = await _aramaService.AramaGecmisiGetirAsync(kullaniciId);
            return Json(gecmis);
        }

        // POST: /Search/ClearHistory
        [HttpPost]
        public async Task<IActionResult> ClearHistory()
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == 0)
            {
                return Json(new { success = false, message = "Giriş yapmanız gerekiyor" });
            }

            await _aramaService.AramaGecmisiTemizleAsync(kullaniciId);
            return Json(new { success = true, message = "Arama geçmişi temizlendi" });
        }

        // GET: /Search/Popular
        [HttpGet]
        public async Task<IActionResult> Popular()
        {
            var populer = await _aramaService.PopulerAramalarGetirAsync();
            return Json(populer);
        }

        // POST: /Search/TrackClick
        [HttpPost]
        public async Task<IActionResult> TrackClick(string term, int productId)
        {
            var kullaniciId = GetKullaniciId();
            await _aramaService.AramaTiklamaKaydetAsync(term, productId, kullaniciId);
            return Json(new { success = true });
        }

        // GET: /Search/Suggestions
        [HttpGet]
        public async Task<IActionResult> Suggestions()
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == 0)
            {
                var populer = await _aramaService.PopulerAramalarGetirAsync(5);
                return Json(populer);
            }

            var oneriler = await _aramaService.AramaOnerileriGetirAsync(kullaniciId);
            return Json(oneriler);
        }
    }
}
