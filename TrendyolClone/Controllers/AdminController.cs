using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Services;

namespace TrendyolClone.Controllers
{
    public class AdminController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly IDropshippingService _dropshippingService;
        private readonly UrunServisi _productService;

        public AdminController(
            UygulamaDbContext context, 
            IDropshippingService dropshippingService,
            UrunServisi productService)
        {
            _context = context;
            _dropshippingService = dropshippingService;
            _productService = productService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AdminId") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            Console.WriteLine($"=== ADMIN LOGIN ATTEMPT ===");
            Console.WriteLine($"Username: {username}");
            
            // Admin'i bul
            var admin = await _context.Yoneticiler
                .Where(a => a.KullaniciAdi == username && a.Aktif)
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                Console.WriteLine("Admin bulunamadı!");
                ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
                return View();
            }

            Console.WriteLine($"Admin bulundu: {admin.KullaniciAdi}");
            
            // Şifre kontrolü - BCrypt hash veya plain text
            bool sifreDogruMu = false;
            
            try
            {
                // Önce BCrypt ile dene
                sifreDogruMu = BCrypt.Net.BCrypt.Verify(password, admin.Sifre);
                Console.WriteLine($"BCrypt sonuç: {sifreDogruMu}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BCrypt hatası: {ex.Message}");
                // BCrypt başarısız olursa, plain text karşılaştır
                sifreDogruMu = (admin.Sifre == password);
                Console.WriteLine($"Plain text sonuç: {sifreDogruMu}");
                
                // Eğer plain text doğruysa, şifreyi BCrypt ile hashle ve güncelle
                if (sifreDogruMu)
                {
                    Console.WriteLine("Admin şifresi BCrypt'e çevriliyor...");
                    admin.Sifre = BCrypt.Net.BCrypt.HashPassword(password);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Admin şifresi güncellendi!");
                }
            }

            // Admin var mı ve şifre doğru mu kontrol et
            if (sifreDogruMu)
            {
                Console.WriteLine("Admin girişi başarılı!");
                // session'a kaydet
                HttpContext.Session.SetString("AdminId", admin.Id.ToString());
                HttpContext.Session.SetString("AdminKullaniciAdi", admin.KullaniciAdi);
                
                // Admin'in email'ine sahip bir User var mı kontrol et (yorum yapabilmesi için)
                var kullanici = await _context.Kullanicilar
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Email == admin.Email && u.Aktif);
                
                if (kullanici != null)
                {
                    // User bilgilerini de session'a ekle (yorum yapabilmesi için)
                    HttpContext.Session.SetString("UserId", kullanici.Id.ToString());
                    HttpContext.Session.SetString("UserName", kullanici.Ad + " " + kullanici.Soyad);
                    HttpContext.Session.SetString("Username", kullanici.KullaniciAdi);
                    if (kullanici.Rol != null)
                    {
                        HttpContext.Session.SetString("UserRole", kullanici.Rol.Ad);
                    }
                }
                else
                {
                    // User yoksa, Admin bilgilerini User olarak set et
                    HttpContext.Session.SetString("UserId", admin.Id.ToString());
                    HttpContext.Session.SetString("UserName", "Admin");
                    HttpContext.Session.SetString("Username", admin.KullaniciAdi);
                    HttpContext.Session.SetString("UserRole", "Admin");
                }
                
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            if (await _context.Yoneticiler.AnyAsync())
            {
                return Json(new { success = false, message = "Admin hesabı zaten mevcut!" });
            }

            var admin = new Yonetici
            {
                KullaniciAdi = "baranAdmin2025",
                Sifre = "Baran@2025!Secure",
                Email = "admin@premiumyol.com",
                OlusturmaTarihi = DateTime.Now,
                Aktif = true
            };

            _context.Yoneticiler.Add(admin);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Admin hesabı oluşturuldu!" });
        }

        public async Task<IActionResult> Dashboard()
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId))
            {
                return RedirectToAction("Login");
            }

            // istatistikler
            int productCount = await _context.Urunler.CountAsync();
            int categoryCount = await _context.Kategoriler.CountAsync();
            int orderCount = await _context.Siparisler.CountAsync();
            int userCount = await _context.Kullanicilar.CountAsync();
            int questionCount = await _context.UrunSorulari.CountAsync();
            int supplierCount = await _context.Tedarikciler.CountAsync();
            int roleCount = await _context.Roller.CountAsync();

            ViewBag.ProductCount = productCount;
            ViewBag.CategoryCount = categoryCount;
            ViewBag.OrderCount = orderCount;
            ViewBag.UserCount = userCount;
            ViewBag.QuestionCount = questionCount;
            ViewBag.SupplierCount = supplierCount;
            ViewBag.RoleCount = roleCount;

            return View();
        }

        public async Task<IActionResult> Orders()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
                return RedirectToAction("Login");

            var siparisler = await _context.Siparisler
                .Include(o => o.Kullanici)
                .Include(o => o.SiparisKalemleri)
                    .ThenInclude(oi => oi.Urun)
                .OrderByDescending(o => o.SiparisTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, SiparisDurumu status)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
                return Json(new { success = false, message = "Yetkiniz yok!" });

            try
            {
                var siparis = await _context.Siparisler.FindAsync(orderId);
                if (siparis == null)
                    return Json(new { success = false, message = "Sipariş bulunamadı!" });

                siparis.Durum = (SiparisDurumu)status;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Sipariş durumu güncellendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
                return Json(new { success = false, message = "Yetkiniz yok!" });

            try
            {
                var siparis = await _context.Siparisler
                    .Include(o => o.SiparisKalemleri)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (siparis == null)
                    return Json(new { success = false, message = "Sipariş bulunamadı!" });

                _context.SiparisKalemleri.RemoveRange(siparis.SiparisKalemleri);
                _context.Siparisler.Remove(siparis);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Sipariş silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        public async Task<IActionResult> Products()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var products = await _context.Urunler.Include(p => p.Kategori).ToListAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Categories = await _context.Kategoriler.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Urun urun)
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId))
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Kategoriler.ToListAsync();
                return View(urun);
            }

            urun.OlusturmaTarihi = DateTime.Now;
            urun.Aktif = true;

            _context.Urunler.Add(urun);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Ürün eklendi!";
            return RedirectToAction("Products");
        }

        // Ürün düzenleme sayfası
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.Kategoriler.ToListAsync();
            return View(urun);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Urun urun)
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId))
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Kategoriler.ToListAsync();
                return View(urun);
            }

            try
            {
                _context.Urunler.Update(urun);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün güncellendi!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Products");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId))
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı!" });
            }

            _context.Urunler.Remove(urun);
            await _context.SaveChangesAsync();
            
            return Json(new { success = true, message = "Ürün silindi!" });
        }

        // Kategori yönetimi
        public async Task<IActionResult> Categories()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var kategoriler = await _context.Kategoriler.ToListAsync();
            return View(kategoriler);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string name, string description)
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId))
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var kategori = new Kategori
            {
                Ad = name,
                Aciklama = description,
                Aktif = true
            };

            _context.Kategoriler.Add(kategori);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Kategori eklendi!" });
        }

        // Kategori silme
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori != null)
            {
                _context.Kategoriler.Remove(kategori);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Kategori silindi!" });
            }

            return Json(new { success = false, message = "Kategori bulunamadı!" });
        }

        // Kullanıcı yönetimi
        public async Task<IActionResult> Users()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var kullanicilar = await _context.Kullanicilar.Include(u => u.Rol).OrderByDescending(u => u.OlusturmaTarihi).ToListAsync();
            ViewBag.Roles = await _context.Roller.Where(r => r.Aktif).ToListAsync();
            return View(kullanicilar);
        }

        // Kullanıcı düzenleme sayfası
        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var kullanici = await _context.Kullanicilar.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Id == id);
            if (kullanici == null)
            {
                return NotFound();
            }

            ViewBag.Roles = await _context.Roller.Where(r => r.Aktif).ToListAsync();
            return View(kullanici);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(Kullanici kullanici)
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId))
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _context.Roller.Where(r => r.Aktif).ToListAsync();
                return View(kullanici);
            }

            try
            {
                var dbKullanici = await _context.Kullanicilar.FindAsync(kullanici.Id);
                if (dbKullanici == null)
                {
                    ViewBag.Error = "Kullanıcı bulunamadı!";
                    ViewBag.Roles = await _context.Roller.Where(r => r.Aktif).ToListAsync();
                    return View(kullanici);
                }

                // bilgileri güncelle
                dbKullanici.Ad = kullanici.Ad;
                dbKullanici.Soyad = kullanici.Soyad;
                dbKullanici.TelefonNumarasi = kullanici.TelefonNumarasi;
                dbKullanici.RolId = kullanici.RolId;
                dbKullanici.Aktif = kullanici.Aktif;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Kullanıcı güncellendi!";
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Hata: " + ex.Message;
                ViewBag.Roles = await _context.Roller.Where(r => r.Aktif).ToListAsync();
                return View(kullanici);
            }
        }

        // Kullanıcı silme
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici != null)
            {
                _context.Kullanicilar.Remove(kullanici);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Kullanıcı silindi!" });
            }

            return Json(new { success = false, message = "Kullanıcı bulunamadı!" });
        }

        // Kullanıcı aktif/pasif yapma
        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici != null)
            {
                kullanici.Aktif = !kullanici.Aktif;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Kullanıcı durumu güncellendi!", isActive = kullanici.Aktif });
            }

            return Json(new { success = false, message = "Kullanıcı bulunamadı!" });
        }

        // Rol yönetimi
        public async Task<IActionResult> Roles()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var roller = await _context.Roller.Include(r => r.Kullanicilar).ToListAsync();
            return View(roller);
        }

        // Rol ekleme
        [HttpPost]
        public async Task<IActionResult> AddRole(string name, string description)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var rol = new Rol
            {
                Ad = name,
                Aciklama = description,
                Aktif = true
            };

            _context.Roller.Add(rol);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Rol eklendi!" });
        }

        // Rol silme
        [HttpPost]
        public async Task<IActionResult> DeleteRole(int id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var rol = await _context.Roller.Include(r => r.Kullanicilar).FirstOrDefaultAsync(r => r.Id == id);
            if (rol != null)
            {
                if (rol.Kullanicilar.Any())
                {
                    return Json(new { success = false, message = "Bu role sahip kullanıcılar var, önce onları başka role atayın!" });
                }

                _context.Roller.Remove(rol);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Rol silindi!" });
            }

            return Json(new { success = false, message = "Rol bulunamadı!" });
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(int userId, int roleId)
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId))
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            try
            {
                var kullanici = await _context.Kullanicilar.FindAsync(userId);
                if (kullanici == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı!" });
                }

                kullanici.RolId = roleId;
                await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Rol atandı!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        // Ödeme Yöntemleri Yönetimi
        public async Task<IActionResult> PaymentMethods()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
                return RedirectToAction("Login");

            var yontemler = await _context.OdemeYontemleri
                .OrderBy(o => o.Sira)
                .ToListAsync();

            return View(yontemler);
        }

        [HttpPost]
        public async Task<IActionResult> SavePaymentMethod([FromBody] OdemeYontemiAyari yontem)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
                return Json(new { success = false, message = "Yetkiniz yok!" });

            try
            {
                if (yontem.Id == 0)
                {
                    _context.OdemeYontemleri.Add(yontem);
                }
                else
                {
                    var mevcut = await _context.OdemeYontemleri.FindAsync(yontem.Id);
                    if (mevcut == null)
                        return Json(new { success = false, message = "Ödeme yöntemi bulunamadı!" });

                    mevcut.Ad = yontem.Ad;
                    mevcut.Aciklama = yontem.Aciklama;
                    mevcut.Tip = yontem.Tip;
                    mevcut.BankaAdi = yontem.BankaAdi;
                    mevcut.IbanNo = yontem.IbanNo;
                    mevcut.HesapSahibi = yontem.HesapSahibi;
                    mevcut.EkBilgi = yontem.EkBilgi;
                    mevcut.Aktif = yontem.Aktif;
                    mevcut.Sira = yontem.Sira;
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Ödeme yöntemi kaydedildi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
                return Json(new { success = false, message = "Yetkiniz yok!" });

            try
            {
                var yontem = await _context.OdemeYontemleri.FindAsync(id);
                if (yontem == null)
                    return Json(new { success = false, message = "Ödeme yöntemi bulunamadı!" });

                _context.OdemeYontemleri.Remove(yontem);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Ödeme yöntemi silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentMethod(int id)
        {
            var yontem = await _context.OdemeYontemleri.FindAsync(id);
            if (yontem == null)
                return NotFound();

            return Json(yontem);
        }

        [HttpPost]
        public async Task<IActionResult> TogglePaymentMethod(int id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
                return Json(new { success = false, message = "Yetkiniz yok!" });

            try
            {
                var yontem = await _context.OdemeYontemleri.FindAsync(id);
                if (yontem == null)
                    return Json(new { success = false, message = "Ödeme yöntemi bulunamadı!" });

                yontem.Aktif = !yontem.Aktif;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Ödeme yöntemi {(yontem.Aktif ? "aktif" : "pasif")} edildi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        // Tedarikçi yönetimi
        public async Task<IActionResult> Suppliers()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var tedarikciler = await _context.Tedarikciler.Include(s => s.Urunler).ToListAsync();
            return View(tedarikciler);
        }

        // Tedarikçi ekleme sayfası
        [HttpGet]
        public IActionResult AddSupplier()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // Tedarikçi ekleme işlemi
        [HttpPost]
        public async Task<IActionResult> AddSupplier(Tedarikci tedarikci)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                _context.Tedarikciler.Add(tedarikci);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tedarikçi başarıyla eklendi!";
                return RedirectToAction("Suppliers");
            }

            return View(tedarikci);
        }

        [HttpPost]
        public async Task<IActionResult> ImportProducts(int supplierId, string searchTerm = "", int page = 1)
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId))
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var tedarikci = await _context.Tedarikciler.FindAsync(supplierId);
            if (tedarikci == null)
            {
                return Json(new { success = false, message = "Tedarikçi bulunamadı!" });
            }

            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(new { success = false, message = "Arama terimi gerekli!" });
            }

            try
            {
                // API'den ürünleri çek
                var productList = await _dropshippingService.SearchProductsAsync(tedarikci, searchTerm, page, 20);
                
                return Json(new { 
                    success = true, 
                    message = $"{productList.Count} ürün bulundu!",
                    products = productList.Select(p => new {
                        p.Ad,
                        Fiyat = p.Fiyat.ToString("F2"),
                        IndirimliFiyat = p.IndirimliFiyat?.ToString("F2"),
                        p.ResimUrl,
                        p.TedarikciUrunId,
                        p.TedarikciUrl,
                        p.Stok,
                        Puan = p.Puan.ToString("F1"),
                        p.YorumSayisi
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveImportedProduct(int supplierId, string supplierProductId, string searchTerm)
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId))
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            try
            {
                var tedarikci = await _context.Tedarikciler.FindAsync(supplierId);
                if (tedarikci == null)
                {
                    return Json(new { success = false, message = "Tedarikçi bulunamadı!" });
                }

                // ürün zaten var mı?
                var existing = await _context.Urunler
                    .Where(p => p.TedarikciId == supplierId && p.TedarikciUrunId == supplierProductId)
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    return Json(new { success = false, message = "Bu ürün zaten mevcut!" });
                }

                // API'den ürünü tekrar çek
                var productList = await _dropshippingService.SearchProductsAsync(tedarikci, searchTerm, 1, 50);
                var urun = productList.FirstOrDefault(p => p.TedarikciUrunId == supplierProductId);

                if (urun == null)
                {
                    return Json(new { success = false, message = "Ürün bulunamadı!" });
                }

                // kaydet
                _context.Urunler.Add(urun);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Ürün kaydedildi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        // Toplu ürün içe aktarma
        [HttpPost]
        public async Task<IActionResult> BulkImportProducts(int supplierId, string searchTerm, int maxProducts = 50)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            try
            {
                var importedProducts = await _productService.ImportProductsAsync(supplierId, searchTerm, maxProducts);
                
                return Json(new { 
                    success = true, 
                    message = $"{importedProducts.Count} ürün başarıyla içe aktarıldı!",
                    count = importedProducts.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Toplu import hatası: " + ex.Message });
            }
        }

        // Fiyat ve stok güncelleme
        [HttpPost]
        public async Task<IActionResult> UpdateSupplierProducts(int supplierId)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            try
            {
                var updatedCount = await _productService.UpdateProductPricesAndStockAsync(supplierId);
                
                return Json(new { 
                    success = true, 
                    message = $"{updatedCount} ürün güncellendi!",
                    count = updatedCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Güncelleme hatası: " + ex.Message });
            }
        }

        // Site ayarları sayfası
        public async Task<IActionResult> SiteSettings()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var ayarlar = await _context.SiteAyarlari.FirstOrDefaultAsync();
            if (ayarlar == null)
            {
                ayarlar = new SiteAyarlari();
            }

            return View(ayarlar);
        }

        // Site ayarları güncelleme
        [HttpPost]
        public async Task<IActionResult> UpdateSiteSettings(SiteAyarlari model)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                var mevcutAyarlar = await _context.SiteAyarlari.FirstOrDefaultAsync();
                
                if (mevcutAyarlar == null)
                {
                    // Yeni ayar oluştur
                    model.GuncellemeTarihi = DateTime.Now;
                    _context.SiteAyarlari.Add(model);
                }
                else
                {
                    // Mevcut ayarları güncelle
                    mevcutAyarlar.SiteAdi = model.SiteAdi;
                    mevcutAyarlar.SiteAciklamasi = model.SiteAciklamasi;
                    mevcutAyarlar.LogoUrl = model.LogoUrl;
                    mevcutAyarlar.LogoIcon = model.LogoIcon;
                    mevcutAyarlar.IletisimEmail = model.IletisimEmail;
                    mevcutAyarlar.IletisimTelefon = model.IletisimTelefon;
                    mevcutAyarlar.AltBilgiMetni = model.AltBilgiMetni;
                    mevcutAyarlar.GuncellemeTarihi = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Site ayarları başarıyla güncellendi!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Güncelleme sırasında hata oluştu: " + ex.Message;
            }

            return RedirectToAction("SiteSettings");
        }

        // GenerateDemoProducts metodu artık kullanılmıyor - DropshippingService kullanılıyor

        // Soru Yönetimi
        public async Task<IActionResult> Questions()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
                return RedirectToAction("Login");

            var sorular = await _context.UrunSorulari
                .Include(q => q.Kullanici)
                .Include(q => q.Urun)
                .Include(q => q.Cevaplar)
                .OrderByDescending(q => q.OlusturmaTarihi)
                .ToListAsync();

            return View(sorular);
        }

        [HttpPost]
        public async Task<IActionResult> AnswerQuestionAdmin(int questionId, string answer)
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId))
                return Json(new { success = false, message = "Yetkiniz yok!" });

            try
            {
                _context.UrunCevaplari.Add(new UrunCevap
                {
                    SoruId = questionId,
                    YoneticiId = int.Parse(adminId),
                    Cevap = answer,
                    OlusturmaTarihi = DateTime.Now,
                    Resmi = true
                });

                var soru = await _context.UrunSorulari.FindAsync(questionId);
                if (soru != null)
                    soru.Cevaplandi = true;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cevap eklendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
                return Json(new { success = false, message = "Yetkiniz yok!" });

            try
            {
                var soru = await _context.UrunSorulari
                    .Include(q => q.Cevaplar)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (soru == null)
                    return Json(new { success = false, message = "Soru bulunamadı!" });

                // Önce cevapları sil
                _context.UrunCevaplari.RemoveRange(soru.Cevaplar);
                
                // Sonra soruyu sil
                _context.UrunSorulari.Remove(soru);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Soru silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }
    }
}
