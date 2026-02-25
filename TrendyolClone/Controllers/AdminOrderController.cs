using Microsoft.AspNetCore.Mvc;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;
using TrendyolClone.Services;

namespace TrendyolClone.Controllers
{
    [Route("Admin/Order")]
    public class AdminOrderController : Controller
    {
        private readonly IAdminOrderService _orderService;
        private readonly ILogger<AdminOrderController> _logger;

        public AdminOrderController(
            IAdminOrderService orderService,
            ILogger<AdminOrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(
            string? arama,
            SiparisDurumu? durum,
            DateTime? baslangic,
            DateTime? bitis,
            int sayfa = 1,
            string siralama = "tarih_desc")
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var filtre = new SiparisFiltre
            {
                AramaTerimi = arama,
                Durum = durum,
                BaslangicTarihi = baslangic,
                BitisTarihi = bitis,
                Sayfa = sayfa,
                Siralama = siralama
            };

            var result = await _orderService.GetSiparislerAsync(filtre);
            var durumIstatistikleri = await _orderService.GetDurumIstatistikleriAsync();

            ViewBag.Filtre = filtre;
            ViewBag.DurumIstatistikleri = durumIstatistikleri;

            return View(result);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var siparis = await _orderService.GetSiparisDetayAsync(id);
            if (siparis == null)
            {
                TempData["Hata"] = "Sipariş bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(siparis);
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(SiparisDurumGuncelleDto dto)
        {
            if (!IsAdmin()) return Unauthorized();

            var adminId = HttpContext.Session.GetInt32("KullaniciId") ?? 0;
            var sonuc = await _orderService.DurumGuncelleAsync(dto, adminId);

            if (sonuc)
            {
                TempData["Mesaj"] = "Sipariş durumu güncellendi.";
            }
            else
            {
                TempData["Hata"] = "Sipariş durumu güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Details), new { id = dto.SiparisId });
        }

        [HttpPost("AddNote")]
        public async Task<IActionResult> AddNote(int siparisId, string not)
        {
            if (!IsAdmin()) return Unauthorized();

            var sonuc = await _orderService.AdminNotuEkleAsync(siparisId, not);

            if (sonuc)
            {
                TempData["Mesaj"] = "Not eklendi.";
            }
            else
            {
                TempData["Hata"] = "Not eklenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Details), new { id = siparisId });
        }

        [HttpPost("Cancel")]
        public async Task<IActionResult> Cancel(int siparisId, string iptalNedeni)
        {
            if (!IsAdmin()) return Unauthorized();

            var adminId = HttpContext.Session.GetInt32("KullaniciId") ?? 0;
            var sonuc = await _orderService.SiparisIptalEtAsync(siparisId, iptalNedeni, adminId);

            if (sonuc)
            {
                TempData["Mesaj"] = "Sipariş iptal edildi.";
            }
            else
            {
                TempData["Hata"] = "Sipariş iptal edilirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Details), new { id = siparisId });
        }

        [HttpGet("Pending")]
        public async Task<IActionResult> Pending()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var bekleyenSiparisler = await _orderService.GetBekleyenSiparislerAsync();
            return View(bekleyenSiparisler);
        }

        private bool IsAdmin()
        {
            var rolId = HttpContext.Session.GetInt32("RolId");
            return rolId == 1;
        }
    }
}
