using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrendyolClone.Models;
using TrendyolClone.Services;

namespace TrendyolClone.Controllers
{
    [Authorize]
    public class SellerController : Controller
    {
        private readonly ISaticiService _saticiService;
        private readonly ISaticiUrunService _saticiUrunService;
        private readonly ISaticiSiparisService _saticiSiparisService;
        private readonly ISaticiFinansService _saticiFinansService;
        
        public SellerController(
            ISaticiService saticiService,
            ISaticiUrunService saticiUrunService,
            ISaticiSiparisService saticiSiparisService,
            ISaticiFinansService saticiFinansService)
        {
            _saticiService = saticiService;
            _saticiUrunService = saticiUrunService;
            _saticiSiparisService = saticiSiparisService;
            _saticiFinansService = saticiFinansService;
        }
        
        private async Task<Satici?> GetCurrentSaticiAsync()
        {
            var kullaniciId = int.Parse(User.FindFirst("KullaniciId")?.Value ?? "0");
            return await _saticiService.GetByKullaniciIdAsync(kullaniciId);
        }
        
        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var satici = await GetCurrentSaticiAsync();
            if (satici == null)
            {
                return RedirectToAction("Register");
            }
            
            if (satici.Durum != SaticiDurum.Onaylandi)
            {
                return View("Pending", satici);
            }
            
            var dashboard = await _saticiService.GetDashboardAsync(satici.Id);
            ViewBag.Satici = satici;
            
            return View(dashboard);
        }
        
        // Satıcı Kayıt
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(Satici model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var kullaniciId = int.Parse(User.FindFirst("KullaniciId")?.Value ?? "0");
            model.KullaniciId = kullaniciId;
            
            await _saticiService.CreateAsync(model);
            
            TempData["Mesaj"] = "Satıcı başvurunuz alındı. Onay sürecinden sonra bilgilendirileceksiniz.";
            return RedirectToAction("Dashboard");
        }
        
        // Profil
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var satici = await GetCurrentSaticiAsync();
            if (satici == null) return RedirectToAction("Register");
            
            return View(satici);
        }
        
        [HttpPost]
        public async Task<IActionResult> Profile(Satici model)
        {
            var satici = await GetCurrentSaticiAsync();
            if (satici == null) return RedirectToAction("Register");
            
            satici.MagazaAdi = model.MagazaAdi;
            satici.Aciklama = model.Aciklama;
            satici.Logo = model.Logo;
            satici.KapakResmi = model.KapakResmi;
            satici.Telefon = model.Telefon;
            satici.Email = model.Email;
            satici.Adres = model.Adres;
            
            await _saticiService.UpdateAsync(satici);
            
            TempData["Mesaj"] = "Profil güncellendi.";
            return RedirectToAction("Profile");
        }
        
        // Ürünler
        [HttpGet]
        public async Task<IActionResult> Products(SaticiUrunDurum? durum = null)
        {
            var satici = await GetCurrentSaticiAsync();
            if (satici == null) return RedirectToAction("Register");
            
            var urunler = await _saticiUrunService.GetBySaticiAsync(satici.Id, durum);
            ViewBag.Satici = satici;
            ViewBag.Durum = durum;
            
            return View(urunler);
        }
        
        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var satici = await GetCurrentSaticiAsync();
            if (satici == null) return RedirectToAction("Register");
            
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateProduct(SaticiUrun model)
        {
            var satici = await GetCurrentSaticiAsync();
            if (satici == null) return RedirectToAction("Register");
            
            model.SaticiId = satici.Id;
            await _saticiUrunService.CreateAsync(model);
            
            TempData["Mesaj"] = "Ürün eklendi.";
            return RedirectToAction("Products");
        }
        
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var urun = await _saticiUrunService.GetByIdAsync(id);
            if (urun == null) return NotFound();
            
            var satici = await GetCurrentSaticiAsync();
            if (satici == null || urun.SaticiId != satici.Id)
            {
                return Forbid();
            }
            
            return View(urun);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditProduct(SaticiUrun model)
        {
            var urun = await _saticiUrunService.GetByIdAsync(model.Id);
            if (urun == null) return NotFound();
            
            var satici = await GetCurrentSaticiAsync();
            if (satici == null || urun.SaticiId != satici.Id)
            {
                return Forbid();
            }
            
            urun.Stok = model.Stok;
            urun.Fiyat = model.Fiyat;
            urun.IndirimliFiyat = model.IndirimliFiyat;
            urun.KargoSuresi = model.KargoSuresi;
            
            await _saticiUrunService.UpdateAsync(urun);
            
            TempData["Mesaj"] = "Ürün güncellendi.";
            return RedirectToAction("Products");
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var urun = await _saticiUrunService.GetByIdAsync(id);
            if (urun == null) return NotFound();
            
            var satici = await GetCurrentSaticiAsync();
            if (satici == null || urun.SaticiId != satici.Id)
            {
                return Forbid();
            }
            
            await _saticiUrunService.DeleteAsync(id);
            
            TempData["Mesaj"] = "Ürün silindi.";
            return RedirectToAction("Products");
        }
        
        [HttpPost]
        public async Task<IActionResult> SubmitForApproval(int id)
        {
            var urun = await _saticiUrunService.GetByIdAsync(id);
            if (urun == null) return NotFound();
            
            var satici = await GetCurrentSaticiAsync();
            if (satici == null || urun.SaticiId != satici.Id)
            {
                return Forbid();
            }
            
            await _saticiUrunService.OnayaGonderAsync(id);
            
            TempData["Mesaj"] = "Ürün onaya gönderildi.";
            return RedirectToAction("Products");
        }
        
        // Siparişler
        [HttpGet]
        public async Task<IActionResult> Orders(SaticiSiparisDurum? durum = null)
        {
            var satici = await GetCurrentSaticiAsync();
            if (satici == null) return RedirectToAction("Register");
            
            var siparisler = await _saticiSiparisService.GetBySaticiAsync(satici.Id, durum);
            ViewBag.Satici = satici;
            ViewBag.Durum = durum;
            
            return View(siparisler);
        }
        
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var siparis = await _saticiSiparisService.GetByIdAsync(id);
            if (siparis == null) return NotFound();
            
            var satici = await GetCurrentSaticiAsync();
            if (satici == null || siparis.SaticiId != satici.Id)
            {
                return Forbid();
            }
            
            return View(siparis);
        }
        
        [HttpPost]
        public async Task<IActionResult> PrepareShipping(int id, string kargoTakipNo)
        {
            var siparis = await _saticiSiparisService.GetByIdAsync(id);
            if (siparis == null) return NotFound();
            
            var satici = await GetCurrentSaticiAsync();
            if (satici == null || siparis.SaticiId != satici.Id)
            {
                return Forbid();
            }
            
            await _saticiSiparisService.KargoHazirlaAsync(id, kargoTakipNo);
            
            TempData["Mesaj"] = "Sipariş kargoya verildi.";
            return RedirectToAction("Orders");
        }
        
        // Finansal
        [HttpGet]
        public async Task<IActionResult> Finance()
        {
            var satici = await GetCurrentSaticiAsync();
            if (satici == null) return RedirectToAction("Register");
            
            var rapor = await _saticiFinansService.GetFinansRaporuAsync(satici.Id);
            ViewBag.Satici = satici;
            
            return View(rapor);
        }
        
        [HttpGet]
        public async Task<IActionResult> Payments()
        {
            var satici = await GetCurrentSaticiAsync();
            if (satici == null) return RedirectToAction("Register");
            
            var odemeler = await _saticiFinansService.GetOdemelerAsync(satici.Id);
            ViewBag.Satici = satici;
            
            return View(odemeler);
        }
        
        [HttpPost]
        public async Task<IActionResult> RequestPayment(int donem)
        {
            var satici = await GetCurrentSaticiAsync();
            if (satici == null) return RedirectToAction("Register");
            
            var basarili = await _saticiFinansService.OdemeTalebiOlusturAsync(satici.Id, donem);
            
            if (basarili)
            {
                TempData["Mesaj"] = "Ödeme talebi oluşturuldu.";
            }
            else
            {
                TempData["Hata"] = "Ödeme talebi oluşturulamadı.";
            }
            
            return RedirectToAction("Payments");
        }
    }
}
