using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;
using TrendyolClone.Services;

namespace TrendyolClone.Controllers
{
    [Route("[controller]")]
    [Route("Urun")] // Türkçe route desteği
    public class ProductController : Controller
    {
        private readonly UygulamaDbContext _db;
        private readonly UrunServisi _urunServisi;

        public ProductController(
            UygulamaDbContext db,
            UrunServisi urunServisi)
        {
            _db = db;
            _urunServisi = urunServisi;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(
            string search,
            int? categoryId,
            string brands,
            decimal? minPrice,
            decimal? maxPrice,
            bool? inStock,
            bool? onSale,
            double? minRating,
            string sort = "default",
            int page = 1)
        {
            // Filtre oluştur
            var filtre = new UrunFiltreDto
            {
                AramaTerimi = search,
                KategoriId = categoryId,
                MarkaIdleri = string.IsNullOrEmpty(brands) 
                    ? null 
                    : brands.Split(',').Select(int.Parse).ToList(),
                MinFiyat = minPrice,
                MaxFiyat = maxPrice,
                SadeceStoktakiler = inStock,
                SadeceIndirimliler = onSale,
                MinPuan = minRating,
                Siralama = sort switch
                {
                    "price-asc" => UrunSiralamaTuru.FiyatArtan,
                    "price-desc" => UrunSiralamaTuru.FiyatAzalan,
                    "newest" => UrunSiralamaTuru.YeniEklenenler,
                    "popular" => UrunSiralamaTuru.PopulerOlanlar,
                    "best-selling" => UrunSiralamaTuru.EnCokSatan,
                    "rating" => UrunSiralamaTuru.EnYuksekPuan,
                    "discount" => UrunSiralamaTuru.IndirimOrani,
                    _ => UrunSiralamaTuru.Varsayilan
                },
                Sayfa = page,
                SayfaBoyutu = 20
            };

            // Ürünleri getir
            var sonuc = await _urunServisi.GetUrunlerAsync(filtre);

            // Kategorileri getir
            var kategoriler = await _db.Kategoriler.Where(c => c.Aktif).ToListAsync();

            // ViewBag'e aktar
            ViewBag.Categories = kategoriler;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.SearchTerm = search;
            ViewBag.SortBy = sort;
            ViewBag.SelectedBrands = brands;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.InStock = inStock;
            ViewBag.OnSale = onSale;
            ViewBag.MinRating = minRating;

            return View(sonuc);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var urun = await _db.Urunler
                .Include(p => p.Kategori)
                .FirstOrDefaultAsync(p => p.Id == id && p.Aktif);

            if (urun == null)
                return NotFound();

            // ViewModel oluştur
            var viewModel = new TrendyolClone.Models.ViewModels.UrunDetayViewModel
            {
                Urun = urun,
                Varyasyonlar = new List<UrunVaryasyon>(),
                Resimler = new List<UrunResim>(),
                Ozellikler = new List<UrunOzellik>()
            };

            var yorumlar = await _db.Yorumlar
                .Include(r => r.Kullanici)
                    .ThenInclude(u => u.Rol)
                .Where(r => r.UrunId == id && r.Onaylandi)
                .OrderByDescending(r => r.OlusturmaTarihi)
                .ToListAsync();

            ViewBag.Reviews = yorumlar;
            ViewBag.AverageRating = yorumlar.Any() ? yorumlar.Average(r => (double)r.Puan) : 0;
            ViewBag.ReviewCount = yorumlar.Count;

            return View(viewModel);
        }

        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var userId = HttpContext.Session.GetString("UserId");
            
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Değerlendirme yapmak için giriş yapmalısınız!" });

            try
            {
                var uid = int.Parse(userId);
                
                var satinAldiMi = await _db.Siparisler
                    .Include(o => o.SiparisKalemleri)
                    .Where(o => o.KullaniciId == uid && 
                               o.Durum == SiparisDurumu.TeslimEdildi &&
                               o.SiparisKalemleri.Any(oi => oi.UrunId == productId))
                    .AnyAsync();
                
                if (!satinAldiMi)
                    return Json(new { success = false, message = "Sadece satın aldığınız ürünleri değerlendirebilirsiniz!" });

                if (await _db.Yorumlar.AnyAsync(r => r.UrunId == productId && r.KullaniciId == uid))
                    return Json(new { success = false, message = "Bu ürün için zaten değerlendirme yaptınız!" });

                _db.Yorumlar.Add(new Yorum
                {
                    UrunId = productId,
                    KullaniciId = uid,
                    Puan = rating,
                    Icerik = comment,
                    OlusturmaTarihi = DateTime.Now,
                    Onaylandi = true,
                    DogrulanmisSatin = true
                });
                await _db.SaveChangesAsync();

                var tumYorumlar = await _db.Yorumlar
                    .Where(r => r.UrunId == productId && r.Onaylandi)
                    .ToListAsync();

                var urun = await _db.Urunler.FindAsync(productId);
                if (urun != null)
                {
                    urun.Puan = tumYorumlar.Average(r => r.Puan);
                    urun.YorumSayisi = tumYorumlar.Count;
                    await _db.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Değerlendirmeniz başarıyla eklendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpGet("GetReviews")]
        public async Task<IActionResult> GetReviews(int productId)
        {
            var yorumlar = await _db.Yorumlar
                .Include(r => r.Kullanici)
                .Where(r => r.UrunId == productId && r.Onaylandi)
                .OrderByDescending(r => r.OlusturmaTarihi)
                .Select(r => new
                {
                    r.Id,
                    Rating = r.Puan,
                    Comment = r.Icerik,
                    CreatedDate = r.OlusturmaTarihi,
                    UserName = r.Kullanici.Ad + " " + r.Kullanici.Soyad
                })
                .ToListAsync();

            return Json(yorumlar);
        }

        [HttpPost("AddToCart")]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var urun = _db.Urunler.Find(productId);
            
            if (urun == null || !urun.Aktif)
                return Json(new { success = false, message = "Ürün bulunamadı!" });

            if (urun.Stok < quantity)
                return Json(new { success = false, message = "Yeterli stok yok!" });

            var sepet = SepetGetir();
            var item = sepet.FirstOrDefault(c => c.UrunId == productId);
            
            if (item != null)
                item.Miktar += quantity;
            else
            {
                sepet.Add(new SepetKalemi
                {
                    UrunId = productId,
                    UrunAdi = urun.Ad,
                    Fiyat = urun.IndirimliFiyat ?? urun.Fiyat,
                    Miktar = quantity,
                    ResimUrl = urun.ResimUrl
                });
            }

            SepetKaydet(sepet);

            return Json(new { success = true, message = "Ürün sepete eklendi!", cartCount = sepet.Sum(c => c.Miktar) });
        }

        [HttpPost("AskQuestion")]
        public async Task<IActionResult> AskQuestion(int productId, string question)
        {
            var userId = HttpContext.Session.GetString("UserId");
            
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Soru sormak için giriş yapmalısınız!" });

            try
            {
                _db.UrunSorulari.Add(new UrunSoru
                {
                    UrunId = productId,
                    KullaniciId = int.Parse(userId),
                    Soru = question,
                    OlusturmaTarihi = DateTime.Now,
                    Cevaplandi = false
                });
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Sorunuz alındı! Kısa süre içinde cevaplanacaktır." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet("GetQuestions")]
        public async Task<IActionResult> GetQuestions(int productId)
        {
            var sorular = await _db.UrunSorulari
                .Include(q => q.Kullanici)
                .Include(q => q.Cevaplar)
                    .ThenInclude(a => a.Kullanici)
                .Include(q => q.Cevaplar)
                    .ThenInclude(a => a.Yonetici)
                .Where(q => q.UrunId == productId)
                .OrderByDescending(q => q.OlusturmaTarihi)
                .Select(q => new
                {
                    q.Id,
                    Question = q.Soru,
                    CreatedDate = q.OlusturmaTarihi,
                    IsAnswered = q.Cevaplandi,
                    UserName = q.Kullanici.Ad + " " + q.Kullanici.Soyad,
                    Answers = q.Cevaplar.Select(a => new
                    {
                        a.Id,
                        Answer = a.Cevap,
                        CreatedDate = a.OlusturmaTarihi,
                        IsOfficial = a.Resmi,
                        AnswererName = a.Resmi 
                            ? (a.Yonetici != null ? a.Yonetici.KullaniciAdi : "Satıcı")
                            : (a.Kullanici != null ? a.Kullanici.Ad + " " + a.Kullanici.Soyad : "Kullanıcı")
                    }).ToList()
                })
                .ToListAsync();

            return Json(sorular);
        }

        [HttpPost("AnswerQuestion")]
        public async Task<IActionResult> AnswerQuestion(int questionId, string answer)
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            var userId = HttpContext.Session.GetString("UserId");
            
            if (string.IsNullOrEmpty(adminId) && string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Cevap vermek için giriş yapmalısınız!" });

            try
            {
                var cevap = new UrunCevap
                {
                    SoruId = questionId,
                    Cevap = answer,
                    OlusturmaTarihi = DateTime.Now
                };

                if (!string.IsNullOrEmpty(adminId))
                {
                    cevap.YoneticiId = int.Parse(adminId);
                    cevap.Resmi = true;
                }
                else
                {
                    cevap.KullaniciId = int.Parse(userId);
                    cevap.Resmi = false;
                }

                _db.UrunCevaplari.Add(cevap);
                
                var soru = await _db.UrunSorulari.FindAsync(questionId);
                if (soru != null)
                    soru.Cevaplandi = true;
                
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Cevabınız eklendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        private List<SepetKalemi> SepetGetir()
        {
            var json = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(json))
                return new List<SepetKalemi>();
            return System.Text.Json.JsonSerializer.Deserialize<List<SepetKalemi>>(json) ?? new List<SepetKalemi>();
        }

        private void SepetKaydet(List<SepetKalemi> sepet)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(sepet);
            HttpContext.Session.SetString("Cart", json);
        }
    }
}
