using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;

namespace TrendyolClone.Controllers
{
    public class AccountController : Controller
    {
        private readonly UygulamaDbContext _db;

        public AccountController(UygulamaDbContext db)
        {
            _db = db;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string emailOrUsername, string password)
        {
            // Debug
            Console.WriteLine($"=== LOGIN ATTEMPT ===");
            Console.WriteLine($"Email/Username: {emailOrUsername}");
            Console.WriteLine($"Password Length: {password?.Length ?? 0}");
            
            var kullanici = _db.Kullanicilar
                .Include(u => u.Rol)
                .FirstOrDefault(u => (u.Email == emailOrUsername || u.KullaniciAdi == emailOrUsername) && u.Aktif);

            if (kullanici == null)
            {
                Console.WriteLine("Kullanıcı bulunamadı veya aktif değil!");
                
                // Kullanıcı var mı ama aktif değil mi kontrol et
                var inaktifKullanici = _db.Kullanicilar
                    .FirstOrDefault(u => u.Email == emailOrUsername || u.KullaniciAdi == emailOrUsername);
                
                if (inaktifKullanici != null && !inaktifKullanici.Aktif)
                {
                    ViewBag.Error = "Hesabınız aktif değil!";
                }
                else
                {
                    ViewBag.Error = "Kullanıcı adı/E-posta veya şifre hatalı!";
                }
                return View();
            }

            Console.WriteLine($"Kullanıcı bulundu: {kullanici.KullaniciAdi}");
            Console.WriteLine($"Şifre DB: {kullanici.Sifre?.Substring(0, Math.Min(20, kullanici.Sifre.Length))}...");

            // Şifre kontrolü - BCrypt hash veya plain text
            bool sifreDogruMu = false;
            
            try
            {
                // Önce BCrypt ile dene
                Console.WriteLine("BCrypt ile deneniyor...");
                sifreDogruMu = BCrypt.Net.BCrypt.Verify(password, kullanici.Sifre);
                Console.WriteLine($"BCrypt sonuç: {sifreDogruMu}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BCrypt hatası: {ex.Message}");
                // BCrypt başarısız olursa, plain text karşılaştır (eski kullanıcılar için)
                Console.WriteLine("Plain text ile deneniyor...");
                sifreDogruMu = (kullanici.Sifre == password);
                Console.WriteLine($"Plain text sonuç: {sifreDogruMu}");
                
                // Eğer plain text doğruysa, şifreyi BCrypt ile hashle ve güncelle
                if (sifreDogruMu)
                {
                    Console.WriteLine("Şifre BCrypt'e çevriliyor...");
                    kullanici.Sifre = BCrypt.Net.BCrypt.HashPassword(password);
                    _db.SaveChanges();
                    Console.WriteLine("Şifre güncellendi!");
                }
            }
            
            if (sifreDogruMu)
            {
                Console.WriteLine("Giriş başarılı! Session oluşturuluyor...");
                HttpContext.Session.SetString("UserId", kullanici.Id.ToString());
                HttpContext.Session.SetString("UserName", kullanici.Ad + " " + kullanici.Soyad);
                HttpContext.Session.SetString("UserEmail", kullanici.Email);
                HttpContext.Session.SetString("Username", kullanici.KullaniciAdi);
                
                if (!string.IsNullOrEmpty(kullanici.ProfilFotoUrl))
                    HttpContext.Session.SetString("UserPhoto", kullanici.ProfilFotoUrl);
                
                if (kullanici.Rol != null)
                    HttpContext.Session.SetString("UserRole", kullanici.Rol.Ad);

                // Kullanıcının sepetini veritabanından yükle
                YukleSepetVeriTabanindan(kullanici.Id);

                Console.WriteLine("Yönlendiriliyor...");
                return RedirectToAction("Index", "Home");
            }

            Console.WriteLine("Şifre yanlış!");
            ViewBag.Error = "Kullanıcı adı/E-posta veya şifre hatalı!";
            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(Kullanici kullanici)
        {
            Console.WriteLine("=== REGISTER ATTEMPT ===");
            Console.WriteLine($"Ad: {kullanici.Ad}");
            Console.WriteLine($"Soyad: {kullanici.Soyad}");
            Console.WriteLine($"KullaniciAdi: {kullanici.KullaniciAdi}");
            Console.WriteLine($"Email: {kullanici.Email}");
            Console.WriteLine($"ModelState Valid: {ModelState.IsValid}");
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState geçersiz!");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Hata: {error.ErrorMessage}");
                }
                return View(kullanici);
            }

            if (_db.Kullanicilar.Any(u => u.Email == kullanici.Email))
            {
                Console.WriteLine("Email zaten kayıtlı!");
                ViewBag.Error = "Bu e-posta adresi zaten kayıtlı!";
                return View(kullanici);
            }

            if (_db.Kullanicilar.Any(u => u.KullaniciAdi == kullanici.KullaniciAdi))
            {
                Console.WriteLine("Kullanıcı adı zaten kullanılıyor!");
                ViewBag.Error = "Bu kullanıcı adı zaten kullanılıyor!";
                return View(kullanici);
            }

            if (_db.Yoneticiler.Any(a => a.Email == kullanici.Email))
            {
                Console.WriteLine("Email admin hesabında mevcut!");
                ViewBag.Error = "Bu e-posta ile admin hesabı mevcut!";
                return View(kullanici);
            }

            Console.WriteLine("Şifre hashleniyor...");
            kullanici.Sifre = BCrypt.Net.BCrypt.HashPassword(kullanici.Sifre);
            
            var rol = _db.Roller.FirstOrDefault(r => r.Ad == "Müşteri");
            if (rol != null)
            {
                Console.WriteLine($"Rol bulundu: {rol.Ad}");
                kullanici.RolId = rol.Id;
            }
            else
            {
                Console.WriteLine("Müşteri rolü bulunamadı!");
            }

            kullanici.OlusturmaTarihi = DateTime.Now;
            kullanici.Aktif = true;

            Console.WriteLine("Kullanıcı veritabanına ekleniyor...");
            _db.Kullanicilar.Add(kullanici);
            _db.SaveChanges();
            Console.WriteLine($"Kullanıcı kaydedildi! ID: {kullanici.Id}");

            HttpContext.Session.SetString("UserId", kullanici.Id.ToString());
            HttpContext.Session.SetString("UserName", kullanici.Ad + " " + kullanici.Soyad);
            HttpContext.Session.SetString("UserEmail", kullanici.Email);
            HttpContext.Session.SetString("Username", kullanici.KullaniciAdi);
            
            if (!string.IsNullOrEmpty(kullanici.ProfilFotoUrl))
                HttpContext.Session.SetString("UserPhoto", kullanici.ProfilFotoUrl);
            
            if (rol != null)
                HttpContext.Session.SetString("UserRole", rol.Ad);

            Console.WriteLine("Kayıt başarılı! Ana sayfaya yönlendiriliyor...");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var kullanici = _db.Kullanicilar
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Id == int.Parse(userId));

            if (kullanici == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(kullanici);
        }

        [HttpPost]
        public IActionResult UpdateProfile(Kullanici model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var kullanici = _db.Kullanicilar.Include(u => u.Rol).FirstOrDefault(u => u.Id == int.Parse(userId));
            if (kullanici == null)
                return RedirectToAction("Login");

            kullanici.Ad = model.Ad;
            kullanici.Soyad = model.Soyad;
            kullanici.TelefonNumarasi = model.TelefonNumarasi;

            _db.SaveChanges();

            HttpContext.Session.SetString("UserName", kullanici.Ad + " " + kullanici.Soyad);
            if (kullanici.Rol != null)
                HttpContext.Session.SetString("UserRole", kullanici.Rol.Ad);

            ViewBag.Success = "Bilgileriniz güncellendi!";
            return View("Profile", kullanici);
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            var kullanici = _db.Kullanicilar.Find(int.Parse(userId));
            if (kullanici == null)
                return Json(new { success = false, message = "Kullanıcı bulunamadı!" });

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, kullanici.Sifre))
                return Json(new { success = false, message = "Mevcut şifreniz hatalı!" });

            if (newPassword != confirmPassword)
                return Json(new { success = false, message = "Yeni şifreler eşleşmiyor!" });

            if (newPassword.Length < 6)
                return Json(new { success = false, message = "Şifre en az 6 karakter olmalı!" });

            kullanici.Sifre = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _db.SaveChanges();

            return Json(new { success = true, message = "Şifreniz değiştirildi!" });
        }

        public IActionResult Orders()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var uid = int.Parse(userId);
            var siparisler = _db.Siparisler
                .Include(o => o.SiparisKalemleri)
                    .ThenInclude(oi => oi.Urun)
                .Where(o => o.KullaniciId == uid)
                .OrderByDescending(o => o.SiparisTarihi)
                .ToList();

            var degerlendirilenUrunler = _db.Yorumlar
                .Where(r => r.KullaniciId == uid)
                .Select(r => r.UrunId)
                .ToList();

            ViewBag.ReviewedProducts = degerlendirilenUrunler;

            return View(siparisler);
        }

        public IActionResult Addresses()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Login");
            return View();
        }

        [HttpGet]
        public IActionResult GetAddresses()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            var adresler = _db.Adresler
                .Where(a => a.KullaniciId == int.Parse(userId))
                .OrderByDescending(a => a.Varsayilan)
                .ThenByDescending(a => a.Id)
                .ToList();

            return Json(adresler);
        }

        [HttpGet]
        public IActionResult GetAddress(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            var adres = _db.Adresler
                .FirstOrDefault(a => a.Id == id && a.KullaniciId == int.Parse(userId));

            if (adres == null)
                return Json(new { success = false, message = "Adres bulunamadı!" });

            return Json(adres);
        }

        [HttpPost]
        public IActionResult AddAddress([FromBody] Adres adres)
        {
            Console.WriteLine("=== ADD ADDRESS ===");
            Console.WriteLine($"Received object: {System.Text.Json.JsonSerializer.Serialize(adres)}");
            Console.WriteLine($"Baslik: {adres?.Baslik}");
            Console.WriteLine($"TamAd: {adres?.TamAd}");
            Console.WriteLine($"Sehir: {adres?.Sehir}");
            Console.WriteLine($"Ilce: {adres?.Ilce}");
            
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("UserId bulunamadı!");
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });
            }

            Console.WriteLine($"UserId: {userId}");
            adres.KullaniciId = int.Parse(userId);

            if (adres.Varsayilan)
            {
                var mevcutAdresler = _db.Adresler.Where(a => a.KullaniciId == adres.KullaniciId);
                foreach (var a in mevcutAdresler)
                    a.Varsayilan = false;
            }

            try
            {
                _db.Adresler.Add(adres);
                _db.SaveChanges();
                Console.WriteLine("Adres başarıyla kaydedildi!");
                return Json(new { success = true, message = "Adres başarıyla eklendi!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateAddress([FromBody] Adres guncellenenAdres)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            var adres = _db.Adresler
                .FirstOrDefault(a => a.Id == guncellenenAdres.Id && a.KullaniciId == int.Parse(userId));

            if (adres == null)
                return Json(new { success = false, message = "Adres bulunamadı!" });

            if (guncellenenAdres.Varsayilan && !adres.Varsayilan)
            {
                var digerAdresler = _db.Adresler.Where(a => a.KullaniciId == int.Parse(userId) && a.Id != adres.Id);
                foreach (var a in digerAdresler)
                    a.Varsayilan = false;
            }

            adres.Baslik = guncellenenAdres.Baslik;
            adres.TamAd = guncellenenAdres.TamAd;
            adres.TelefonNumarasi = guncellenenAdres.TelefonNumarasi;
            adres.Sehir = guncellenenAdres.Sehir;
            adres.Ilce = guncellenenAdres.Ilce;
            adres.PostaKodu = guncellenenAdres.PostaKodu;
            adres.AdresSatiri = guncellenenAdres.AdresSatiri;
            adres.Varsayilan = guncellenenAdres.Varsayilan;

            _db.SaveChanges();

            return Json(new { success = true, message = "Adres başarıyla güncellendi!" });
        }

        [HttpPost]
        public IActionResult DeleteAddress(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            var adres = _db.Adresler
                .FirstOrDefault(a => a.Id == id && a.KullaniciId == int.Parse(userId));

            if (adres == null)
                return Json(new { success = false, message = "Adres bulunamadı!" });

            _db.Adresler.Remove(adres);
            _db.SaveChanges();

            return Json(new { success = true, message = "Adres başarıyla silindi!" });
        }

        public IActionResult Favorites()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var favoriler = _db.Favoriler
                .Include(f => f.Urun)
                    .ThenInclude(p => p.Kategori)
                .Where(f => f.KullaniciId == int.Parse(userId))
                .OrderByDescending(f => f.OlusturmaTarihi)
                .ToList();

            return View(favoriler);
        }

        [HttpPost]
        public IActionResult AddToFavorites(int productId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Giriş yapmalısınız!" });

            var mevcutFavori = _db.Favoriler
                .FirstOrDefault(f => f.KullaniciId == int.Parse(userId) && f.UrunId == productId);

            if (mevcutFavori != null)
                return Json(new { success = false, message = "Bu ürün zaten favorilerinizde!" });

            _db.Favoriler.Add(new Favori
            {
                KullaniciId = int.Parse(userId),
                UrunId = productId,
                OlusturmaTarihi = DateTime.Now
            });
            _db.SaveChanges();

            return Json(new { success = true, message = "Ürün favorilere eklendi!" });
        }

        [HttpPost]
        public IActionResult RemoveFromFavorites(int productId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Giriş yapmalısınız!" });

            var favori = _db.Favoriler
                .FirstOrDefault(f => f.KullaniciId == int.Parse(userId) && f.UrunId == productId);

            if (favori == null)
                return Json(new { success = false, message = "Ürün favorilerinizde bulunamadı!" });

            _db.Favoriler.Remove(favori);
            _db.SaveChanges();

            return Json(new { success = true, message = "Ürün favorilerden çıkarıldı!" });
        }

        [HttpGet]
        public IActionResult IsFavorite(int productId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { isFavorite = false });

            var favoriMi = _db.Favoriler
                .Any(f => f.KullaniciId == int.Parse(userId) && f.UrunId == productId);

            return Json(new { isFavorite = favoriMi });
        }

        public IActionResult Reviews()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Login");
            return View();
        }

        public IActionResult Settings()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Login");
            return View();
        }

        public IActionResult DeleteAccount()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var kullanici = _db.Kullanicilar
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Id == int.Parse(userId));

            if (kullanici == null)
                return RedirectToAction("Login");

            return View(kullanici);
        }

        [HttpPost]
        public IActionResult ConfirmDeleteAccount(string password)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            var kullanici = _db.Kullanicilar.FirstOrDefault(u => u.Id == int.Parse(userId));
            if (kullanici == null)
                return Json(new { success = false, message = "Kullanıcı bulunamadı!" });

            if (!BCrypt.Net.BCrypt.Verify(password, kullanici.Sifre))
                return Json(new { success = false, message = "Şifre hatalı!" });

            try
            {
                var uid = int.Parse(userId);
                _db.Favoriler.RemoveRange(_db.Favoriler.Where(f => f.KullaniciId == uid));
                _db.Adresler.RemoveRange(_db.Adresler.Where(a => a.KullaniciId == uid));
                _db.Yorumlar.RemoveRange(_db.Yorumlar.Where(r => r.KullaniciId == uid));

                var siparisler = _db.Siparisler.Where(o => o.KullaniciId == uid).ToList();
                foreach (var s in siparisler)
                    s.KullaniciId = 0;

                _db.Kullanicilar.Remove(kullanici);
                _db.SaveChanges();

                HttpContext.Session.Clear();

                return Json(new { success = true, message = "Hesabınız başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hesap silinirken bir hata oluştu: " + ex.Message });
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Geçici şifre sıfırlama metodu (sadece development için!)
        [HttpGet]
        public IActionResult ResetPasswordDev(string username, string newPassword)
        {
            #if DEBUG
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newPassword))
            {
                return Content("Kullanım: /Account/ResetPasswordDev?username=mba0625&newPassword=yenisifre");
            }

            var kullanici = _db.Kullanicilar.FirstOrDefault(u => u.KullaniciAdi == username);
            if (kullanici == null)
            {
                return Content($"Kullanıcı bulunamadı: {username}");
            }

            kullanici.Sifre = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _db.SaveChanges();

            return Content($"Şifre güncellendi! Kullanıcı: {username}, Yeni Şifre: {newPassword}");
            #else
            return NotFound();
            #endif
        }

        // Admin oluşturma metodu (sadece development için!)
        [HttpGet]
        public IActionResult CreateAdminDev(string username, string email, string password)
        {
            #if DEBUG
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Content("Kullanım: /Account/CreateAdminDev?username=admin&email=admin@test.com&password=admin123");
            }

            // Admin zaten var mı?
            var mevcutAdmin = _db.Yoneticiler.FirstOrDefault(a => a.KullaniciAdi == username);
            if (mevcutAdmin != null)
            {
                return Content($"Admin zaten var: {username}. Şifre sıfırlamak için: /Account/ResetAdminPasswordDev?username={username}&newPassword=yenisifre");
            }

            // Yeni admin oluştur
            var admin = new Yonetici
            {
                KullaniciAdi = username,
                Email = email ?? $"{username}@admin.com",
                Sifre = BCrypt.Net.BCrypt.HashPassword(password),
                OlusturmaTarihi = DateTime.Now,
                Aktif = true
            };

            _db.Yoneticiler.Add(admin);
            _db.SaveChanges();

            return Content($"Admin oluşturuldu! Kullanıcı Adı: {username}, Email: {admin.Email}, Şifre: {password}");
            #else
            return NotFound();
            #endif
        }

        // Kullanıcıları listele (sadece development için!)
        [HttpGet]
        public IActionResult ListUsers()
        {
            #if DEBUG
            var users = _db.Kullanicilar
                .Select(u => new { u.Id, u.KullaniciAdi, u.Email, u.Aktif })
                .ToList();
            
            var html = "<h2>Kullanıcılar</h2><table border='1'><tr><th>ID</th><th>Kullanıcı Adı</th><th>Email</th><th>Aktif</th></tr>";
            foreach (var user in users)
            {
                html += $"<tr><td>{user.Id}</td><td>{user.KullaniciAdi}</td><td>{user.Email}</td><td>{user.Aktif}</td></tr>";
            }
            html += "</table>";
            
            return Content(html, "text/html");
            #else
            return NotFound();
            #endif
        }

        // Sepeti veritabanından yükle
        private void YukleSepetVeriTabanindan(int kullaniciId)
        {
            try
            {
                var sepetUrunleri = _db.SepetUrunleri
                    .Include(c => c.Urun)
                    .Where(c => c.KullaniciId == kullaniciId)
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
                    
                    var json = System.Text.Json.JsonSerializer.Serialize(sepet);
                    HttpContext.Session.SetString("Cart", json);
                    Console.WriteLine($"Sepet yüklendi: {sepet.Count} ürün, toplam {sepet.Sum(s => s.Miktar)} adet");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sepet yüklenirken hata: {ex.Message}");
            }
        }

        // Admin şifre sıfırlama metodu (sadece development için!)
        [HttpGet]
        public IActionResult ResetAdminPasswordDev(string username, string newPassword)
        {
            #if DEBUG
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newPassword))
            {
                return Content("Kullanım: /Account/ResetAdminPasswordDev?username=admin&newPassword=yenisifre");
            }

            var admin = _db.Yoneticiler.FirstOrDefault(a => a.KullaniciAdi == username);
            if (admin == null)
            {
                return Content($"Admin bulunamadı: {username}. Oluşturmak için: /Account/CreateAdminDev?username={username}&password=admin123");
            }

            admin.Sifre = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _db.SaveChanges();

            return Content($"Admin şifresi güncellendi! Kullanıcı Adı: {username}, Yeni Şifre: {newPassword}");
            #else
            return NotFound();
            #endif
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile photo)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            if (photo == null || photo.Length == 0)
                return Json(new { success = false, message = "Lütfen bir fotoğraf seçin!" });

            // Dosya boyutu kontrolü (max 5MB)
            if (photo.Length > 5 * 1024 * 1024)
                return Json(new { success = false, message = "Fotoğraf boyutu 5MB'dan küçük olmalıdır!" });

            // Dosya uzantısı kontrolü
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return Json(new { success = false, message = "Sadece JPG, PNG ve GIF formatları desteklenir!" });

            try
            {
                var kullanici = await _db.Kullanicilar.FindAsync(int.Parse(userId));
                if (kullanici == null)
                    return Json(new { success = false, message = "Kullanıcı bulunamadı!" });

                // Uploads klasörünü oluştur
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Eski fotoğrafı sil
                if (!string.IsNullOrEmpty(kullanici.ProfilFotoUrl))
                {
                    var oldPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", kullanici.ProfilFotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPhotoPath))
                        System.IO.File.Delete(oldPhotoPath);
                }

                // Yeni dosya adı oluştur
                var fileName = $"{userId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // Veritabanını güncelle
                kullanici.ProfilFotoUrl = $"/uploads/profiles/{fileName}";
                await _db.SaveChangesAsync();
                
                // Session'ı güncelle
                HttpContext.Session.SetString("UserPhoto", kullanici.ProfilFotoUrl);

                return Json(new { success = true, message = "Profil fotoğrafı güncellendi!", photoUrl = kullanici.ProfilFotoUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fotoğraf yüklenirken hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProfilePhoto()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            try
            {
                var kullanici = await _db.Kullanicilar.FindAsync(int.Parse(userId));
                if (kullanici == null)
                    return Json(new { success = false, message = "Kullanıcı bulunamadı!" });

                // Fotoğrafı sil
                if (!string.IsNullOrEmpty(kullanici.ProfilFotoUrl))
                {
                    var photoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", kullanici.ProfilFotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(photoPath))
                        System.IO.File.Delete(photoPath);

                    kullanici.ProfilFotoUrl = null;
                    await _db.SaveChangesAsync();
                    
                    // Session'dan kaldır
                    HttpContext.Session.Remove("UserPhoto");
                }

                return Json(new { success = true, message = "Profil fotoğrafı silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fotoğraf silinirken hata oluştu: " + ex.Message });
            }
        }
    }
}
