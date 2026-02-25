using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Services;

namespace TrendyolClone.Controllers
{
    public class CartController : Controller
    {
        private readonly UygulamaDbContext _db;
        private readonly IKuponService _kuponService;
        private readonly IKargoService _kargoService;
        private readonly IKayitliSepetService _kayitliSepetService;

        public CartController(
            UygulamaDbContext db,
            IKuponService kuponService,
            IKargoService kargoService,
            IKayitliSepetService kayitliSepetService)
        {
            _db = db;
            _kuponService = kuponService;
            _kargoService = kargoService;
            _kayitliSepetService = kayitliSepetService;
        }

        public IActionResult Index()
        {
            return View(SepetGetir());
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            Console.WriteLine($"=== ADD TO CART ===");
            Console.WriteLine($"Product ID: {productId}");
            Console.WriteLine($"Quantity: {quantity}");
            
            try
            {
                var urun = await _db.Urunler.FindAsync(productId);
                if (urun == null)
                {
                    Console.WriteLine("Ürün bulunamadı!");
                    return Json(new { success = false, message = "Ürün bulunamadı!" });
                }

                Console.WriteLine($"Ürün bulundu: {urun.Ad}");

                if (!urun.Aktif)
                {
                    Console.WriteLine("Ürün aktif değil!");
                    return Json(new { success = false, message = "Bu ürün şu anda satışta değil!" });
                }

                if (urun.Stok < quantity)
                {
                    Console.WriteLine($"Yetersiz stok! Stok: {urun.Stok}, İstenen: {quantity}");
                    return Json(new { success = false, message = "Yeterli stok yok!" });
                }

                // Kullanıcı giriş yapmışsa veritabanına kaydet
                var userId = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userId))
                {
                    var kullaniciId = int.Parse(userId);
                    
                    try
                    {
                        var mevcutSepetUrunu = await _db.SepetUrunleri
                            .FirstOrDefaultAsync(c => c.KullaniciId == kullaniciId && c.UrunId == productId);
                        
                        if (mevcutSepetUrunu != null)
                        {
                            Console.WriteLine($"Ürün sepette mevcut (ID: {mevcutSepetUrunu.Id}), miktar artırılıyor: {mevcutSepetUrunu.Adet} + {quantity}");
                            mevcutSepetUrunu.Adet += quantity;
                            _db.SepetUrunleri.Update(mevcutSepetUrunu);
                        }
                        else
                        {
                            Console.WriteLine("Ürün veritabanına ekleniyor...");
                            _db.SepetUrunleri.Add(new SepetUrunu
                            {
                                KullaniciId = kullaniciId,
                                UrunId = productId,
                                Adet = quantity,
                                EklenmeTarihi = DateTime.Now
                            });
                        }
                        await _db.SaveChangesAsync();
                        Console.WriteLine("Veritabanı güncellendi!");
                    }
                    catch (Exception dbEx)
                    {
                        Console.WriteLine($"Veritabanı hatası: {dbEx.Message}");
                        // Veritabanı hatası olsa bile session'a ekle
                    }
                }

                // Session'a da kaydet (hızlı erişim için)
                var sepet = SepetGetir();
                var mevcutUrun = sepet.FirstOrDefault(c => c.UrunId == productId);

                if (mevcutUrun != null)
                {
                    mevcutUrun.Miktar += quantity;
                }
                else
                {
                    sepet.Add(new SepetKalemi
                    {
                        UrunId = urun.Id,
                        UrunAdi = urun.Ad,
                        Fiyat = urun.IndirimliFiyat ?? urun.Fiyat,
                        Miktar = quantity,
                        ResimUrl = urun.ResimUrl
                    });
                }

                SepetKaydet(sepet);
                Console.WriteLine($"Sepet kaydedildi! Toplam ürün: {sepet.Sum(c => c.Miktar)}");

                return Json(new { 
                    success = true, 
                    message = "Ürün sepete eklendi!",
                    cartCount = sepet.Sum(c => c.Miktar)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            // Veritabanını güncelle
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                var kullaniciId = int.Parse(userId);
                var sepetUrunu = _db.SepetUrunleri.FirstOrDefault(c => c.KullaniciId == kullaniciId && c.UrunId == productId);
                
                if (sepetUrunu != null)
                {
                    if (quantity <= 0)
                        _db.SepetUrunleri.Remove(sepetUrunu);
                    else
                        sepetUrunu.Adet = quantity;
                    await _db.SaveChangesAsync();
                }
            }
            
            // Session'ı güncelle
            var sepet = SepetGetir();
            var urun = sepet.FirstOrDefault(c => c.UrunId == productId);
            
            if (urun != null)
            {
                if (quantity <= 0)
                    sepet.Remove(urun);
                else
                    urun.Miktar = quantity;
                SepetKaydet(sepet);
            }

            return Json(new { 
                success = true, 
                cartCount = sepet.Sum(c => c.Miktar), 
                total = sepet.Sum(c => c.ToplamFiyat) 
            });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            // Veritabanından sil
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                var kullaniciId = int.Parse(userId);
                var sepetUrunu = _db.SepetUrunleri.FirstOrDefault(c => c.KullaniciId == kullaniciId && c.UrunId == productId);
                
                if (sepetUrunu != null)
                {
                    _db.SepetUrunleri.Remove(sepetUrunu);
                    await _db.SaveChangesAsync();
                }
            }
            
            // Session'dan sil
            var sepet = SepetGetir();
            var urun = sepet.FirstOrDefault(c => c.UrunId == productId);
            
            if (urun != null)
            {
                sepet.Remove(urun);
                SepetKaydet(sepet);
            }

            return Json(new { 
                success = true, 
                cartCount = sepet.Sum(c => c.Miktar), 
                total = sepet.Sum(c => c.ToplamFiyat) 
            });
        }

        public async Task<IActionResult> Clear()
        {
            // Veritabanından sil
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                var kullaniciId = int.Parse(userId);
                var sepetUrunleri = _db.SepetUrunleri.Where(c => c.KullaniciId == kullaniciId);
                _db.SepetUrunleri.RemoveRange(sepetUrunleri);
                await _db.SaveChangesAsync();
            }
            
            // Session'dan sil
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var sepet = SepetGetir();
            return Json(new { count = sepet.Sum(c => c.Miktar) });
        }

        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var sepet = SepetGetir();
            if (!sepet.Any())
            {
                TempData["Error"] = "Sepetiniz boş!";
                return RedirectToAction("Index");
            }

            return View(sepet);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder(int addressId, string paymentMethod)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            var sepet = SepetGetir();
            if (!sepet.Any())
                return Json(new { success = false, message = "Sepetiniz boş!" });

            try
            {
                var adres = await _db.Adresler.FindAsync(addressId);
                var adresBilgi = adres != null 
                    ? $"{adres.TamAd}, {adres.AdresSatiri}, {adres.Ilce}/{adres.Sehir}"
                    : "Adres bilgisi yok";
                
                var siparis = new Siparis
                {
                    KullaniciId = int.Parse(userId),
                    SiparisTarihi = DateTime.Now,
                    ToplamTutar = sepet.Sum(c => c.ToplamFiyat),
                    Durum = SiparisDurumu.Onaylandi,
                    TeslimatAdresi = adresBilgi,
                    OdemeYontemi = string.IsNullOrEmpty(paymentMethod) ? "Kredi Kartı" : paymentMethod,
                    SiparisKalemleri = sepet.Select(c => new SiparisKalemi
                    {
                        UrunId = c.UrunId,
                        Miktar = c.Miktar,
                        BirimFiyat = c.Fiyat
                    }).ToList()
                };

                foreach (var item in sepet)
                {
                    var urun = await _db.Urunler.FindAsync(item.UrunId);
                    if (urun != null)
                    {
                        if (urun.Stok < item.Miktar)
                            return Json(new { success = false, message = $"{urun.Ad} için yeterli stok yok!" });
                        urun.Stok -= item.Miktar;
                    }
                }

                _db.Siparisler.Add(siparis);
                await _db.SaveChangesAsync();
                
                // Sepeti veritabanından temizle
                var kullaniciId = int.Parse(userId);
                var sepetUrunleri = _db.SepetUrunleri.Where(c => c.KullaniciId == kullaniciId);
                _db.SepetUrunleri.RemoveRange(sepetUrunleri);
                await _db.SaveChangesAsync();
                
                HttpContext.Session.SetString("LastOrderId", siparis.Id.ToString());
                HttpContext.Session.SetString("LastOrderTotal", siparis.ToplamTutar.ToString("F2"));
                HttpContext.Session.SetString("LastOrderPaymentMethod", siparis.OdemeYontemi);
                HttpContext.Session.Remove("Cart");

                return Json(new { success = true, message = "Siparişiniz alındı!", orderId = siparis.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        // ===== YENİ: Kupon İşlemleri =====
        
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string kuponKodu)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Giriş yapmalısınız!" });

            var sepet = SepetGetir();
            if (!sepet.Any())
                return Json(new { success = false, message = "Sepetiniz boş!" });

            var sepetTutari = sepet.Sum(s => s.ToplamFiyat);
            
            // Kategori ID'lerini topla
            var kategoriIdleri = await _db.Urunler
                .Where(u => sepet.Select(s => s.UrunId).Contains(u.Id))
                .Select(u => u.KategoriId)
                .Distinct()
                .ToListAsync();

            // Kupon kontrolü
            var hataMesaji = await _kuponService.KuponHataMesaji(kuponKodu, int.Parse(userId), sepetTutari, kategoriIdleri);
            if (hataMesaji != null)
                return Json(new { success = false, message = hataMesaji });

            // İndirim hesapla
            var indirim = await _kuponService.HesaplaIndirimiAsync(kuponKodu, sepetTutari);

            // Session'a kaydet
            HttpContext.Session.SetString("AppliedCoupon", kuponKodu.ToUpper());
            HttpContext.Session.SetString("CouponDiscount", indirim.ToString("F2"));

            return Json(new
            {
                success = true,
                indirim = indirim,
                yeniToplam = sepetTutari - indirim,
                message = $"{indirim:C} indirim uygulandı!"
            });
        }

        [HttpPost]
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.Remove("AppliedCoupon");
            HttpContext.Session.Remove("CouponDiscount");

            var sepet = SepetGetir();
            var sepetTutari = sepet.Sum(s => s.ToplamFiyat);

            return Json(new
            {
                success = true,
                yeniToplam = sepetTutari,
                message = "Kupon kaldırıldı!"
            });
        }

        // ===== YENİ: Kargo İşlemleri =====
        
        [HttpPost]
        public async Task<IActionResult> CalculateShipping(string il)
        {
            var sepet = SepetGetir();
            if (!sepet.Any())
                return Json(new { success = false, message = "Sepetiniz boş!" });

            // Ürünlerin ağırlık ve desi bilgilerini topla
            var urunIdleri = sepet.Select(s => s.UrunId).ToList();
            var kargoOlculeri = await _db.KargoOlculeri
                .Where(ko => urunIdleri.Contains(ko.UrunId))
                .ToListAsync();

            decimal toplamAgirlik = 0;
            decimal toplamDesi = 0;

            foreach (var item in sepet)
            {
                var olcu = kargoOlculeri.FirstOrDefault(ko => ko.UrunId == item.UrunId);
                if (olcu != null)
                {
                    toplamAgirlik += olcu.Agirlik * item.Miktar;
                    toplamDesi += olcu.Desi * item.Miktar;
                }
                else
                {
                    // Varsayılan değerler
                    toplamAgirlik += 0.5m * item.Miktar;
                    toplamDesi += 1m * item.Miktar;
                }
            }

            var sepetTutari = sepet.Sum(s => s.ToplamFiyat);
            var kargoSecenekleri = await _kargoService.HesaplaKargoUcretleriAsync(il, toplamAgirlik, toplamDesi, sepetTutari);

            return Json(new { success = true, secenekler = kargoSecenekleri });
        }

        [HttpPost]
        public IActionResult SelectShipping(int kargoFirmaId, decimal kargoUcreti)
        {
            HttpContext.Session.SetString("SelectedShipping", kargoFirmaId.ToString());
            HttpContext.Session.SetString("ShippingCost", kargoUcreti.ToString("F2"));

            var sepet = SepetGetir();
            var sepetTutari = sepet.Sum(s => s.ToplamFiyat);
            
            // Kupon indirimi varsa uygula
            var kuponIndirim = 0m;
            var kuponIndirimiStr = HttpContext.Session.GetString("CouponDiscount");
            if (!string.IsNullOrEmpty(kuponIndirimiStr))
                decimal.TryParse(kuponIndirimiStr, out kuponIndirim);

            var genelToplam = sepetTutari - kuponIndirim + kargoUcreti;

            return Json(new
            {
                success = true,
                kargoUcreti = kargoUcreti,
                genelToplam = genelToplam,
                message = "Kargo seçimi kaydedildi!"
            });
        }

        // ===== YENİ: Kayıtlı Sepet İşlemleri =====
        
        [HttpPost]
        public async Task<IActionResult> SaveCart(string ad, string aciklama)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Giriş yapmalısınız!" });

            var sepet = SepetGetir();
            if (!sepet.Any())
                return Json(new { success = false, message = "Sepetiniz boş!" });

            var kayitliSepet = new KayitliSepet
            {
                KullaniciId = int.Parse(userId),
                Ad = ad,
                Aciklama = aciklama,
                Urunler = sepet.Select(s => new KayitliSepetUrunu
                {
                    UrunId = s.UrunId,
                    VaryasyonId = null, // Şimdilik varyasyon desteği yok
                    Adet = s.Miktar
                }).ToList()
            };

            await _kayitliSepetService.KaydetAsync(kayitliSepet);

            return Json(new { success = true, message = "Sepet kaydedildi!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetSavedCarts()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Giriş yapmalısınız!" });

            var sepetler = await _kayitliSepetService.GetKullaniciSepetleriAsync(int.Parse(userId));

            return Json(new
            {
                success = true,
                sepetler = sepetler.Select(s => new
                {
                    id = s.Id,
                    ad = s.Ad,
                    aciklama = s.Aciklama,
                    urunSayisi = s.ToplamUrunSayisi,
                    toplam = s.ToplamTutar,
                    tarih = s.OlusturmaTarihi.ToString("dd.MM.yyyy")
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadSavedCart(int sepetId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Giriş yapmalısınız!" });

            var basarili = await _kayitliSepetService.SepeteYukleAsync(sepetId, int.Parse(userId));
            if (!basarili)
                return Json(new { success = false, message = "Sepet yüklenemedi!" });

            // Session'ı güncelle
            HttpContext.Session.Remove("Cart");

            return Json(new { success = true, message = "Sepet yüklendi!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSavedCart(int sepetId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Giriş yapmalısınız!" });

            var basarili = await _kayitliSepetService.SilAsync(sepetId);
            if (!basarili)
                return Json(new { success = false, message = "Sepet silinemedi!" });

            return Json(new { success = true, message = "Sepet silindi!" });
        }

        public IActionResult OrderSuccess(int? orderId = null)
        {
            var siparisId = orderId?.ToString() ?? HttpContext.Session.GetString("LastOrderId");
            if (string.IsNullOrEmpty(siparisId))
                return RedirectToAction("Index", "Home");

            ViewBag.OrderId = siparisId;
            ViewBag.OrderTotal = HttpContext.Session.GetString("LastOrderTotal");
            ViewBag.PaymentMethod = HttpContext.Session.GetString("LastOrderPaymentMethod");
            
            return View();
        }

        private List<SepetKalemi> SepetGetir()
        {
            // Önce session'dan kontrol et
            var json = HttpContext.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<List<SepetKalemi>>(json) ?? new List<SepetKalemi>();
                }
                catch { }
            }
            
            // Session'da yoksa ve kullanıcı giriş yapmışsa veritabanından yükle
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                var kullaniciId = int.Parse(userId);
                var sepetUrunleri = _db.SepetUrunleri
                    .Where(c => c.KullaniciId == kullaniciId)
                    .Select(c => new { c.UrunId, c.Adet, c.Urun })
                    .ToList();
                
                if (sepetUrunleri.Any())
                {
                    var sepet = sepetUrunleri.Select(c => new SepetKalemi
                    {
                        UrunId = c.UrunId,
                        UrunAdi = c.Urun.Ad,
                        Fiyat = c.Urun.IndirimliFiyat ?? c.Urun.Fiyat,
                        Miktar = c.Adet,
                        ResimUrl = c.Urun.ResimUrl
                    }).ToList();
                    
                    // Session'a da kaydet
                    SepetKaydet(sepet);
                    return sepet;
                }
            }
            
            return new List<SepetKalemi>();
        }

        private void SepetKaydet(List<SepetKalemi> sepet)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(sepet);
            HttpContext.Session.SetString("Cart", json);
        }
    }
}