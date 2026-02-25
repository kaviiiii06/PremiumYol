using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrendyolClone.Services;
using TrendyolClone.Models.DTOs;
using TrendyolClone.Models;

namespace TrendyolClone.Controllers
{
    public class OrderController : Controller
    {
        private readonly IGelismisSiparisService _siparisService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IGelismisSiparisService siparisService, ILogger<OrderController> logger)
        {
            _siparisService = siparisService;
            _logger = logger;
        }

        private int GetKullaniciId()
        {
            var kullaniciIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(kullaniciIdClaim, out int id) ? id : 0;
        }

        // GET: /Order
        public async Task<IActionResult> Index()
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var siparisler = await _siparisService.KullaniciSiparisleriniGetirAsync(kullaniciId);
            return View(siparisler);
        }

        // GET: /Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var siparis = await _siparisService.SiparisDetayGetirAsync(id, kullaniciId);
            if (siparis == null)
            {
                return NotFound();
            }

            return View(siparis);
        }

        // GET: /Order/Track/5
        public async Task<IActionResult> Track(int id)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var siparis = await _siparisService.SiparisDetayGetirAsync(id, kullaniciId);
            if (siparis == null)
            {
                return NotFound();
            }

            var kargoTakip = await _siparisService.KargoTakipGetirAsync(id);
            
            ViewBag.Siparis = siparis;
            return View(kargoTakip);
        }

        // GET: /Order/Return/5
        public async Task<IActionResult> Return(int id)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var siparis = await _siparisService.SiparisDetayGetirAsync(id, kullaniciId);
            if (siparis == null)
            {
                return NotFound();
            }

            if (siparis.Durum != SiparisDurumu.TeslimEdildi)
            {
                TempData["Error"] = "Sadece teslim edilmiş siparişler için iade talebi oluşturulabilir.";
                return RedirectToAction("Details", new { id });
            }

            return View(siparis);
        }

        // POST: /Order/Return
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(IadeTalebiDto dto)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var iade = await _siparisService.IadeTalebiOlusturAsync(dto, kullaniciId);
                TempData["Success"] = "İade talebiniz başarıyla oluşturuldu. En kısa sürede değerlendirilecektir.";
                return RedirectToAction("Details", new { id = dto.SiparisId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İade talebi oluşturulurken hata");
                TempData["Error"] = ex.Message;
                return RedirectToAction("Return", new { id = dto.SiparisId });
            }
        }

        // POST: /Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string neden)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == 0)
            {
                return Json(new { success = false, message = "Giriş yapmanız gerekiyor" });
            }

            try
            {
                await _siparisService.SiparisIptalEtAsync(id, neden);
                return Json(new { success = true, message = "Siparişiniz iptal edildi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sipariş iptal edilirken hata");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Order/Invoice/5
        public async Task<IActionResult> Invoice(int id)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var siparis = await _siparisService.SiparisDetayGetirAsync(id, kullaniciId);
            if (siparis == null)
            {
                return NotFound();
            }

            var fatura = await _siparisService.FaturaGetirAsync(id);
            if (fatura == null)
            {
                // Fatura yoksa oluştur
                fatura = await _siparisService.FaturaOlusturAsync(id);
            }

            ViewBag.Siparis = siparis;
            return View(fatura);
        }

        // GET: /Order/Returns
        public async Task<IActionResult> Returns()
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var iadeler = await _siparisService.KullaniciIadeleriniGetirAsync(kullaniciId);
            return View(iadeler);
        }
    }
}
