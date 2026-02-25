using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace TrendyolClone.Services
{
    public class UrunServisi
    {
        private readonly UygulamaDbContext _context;
        private readonly IDropshippingService _dropshippingService;
        private readonly ILogger<UrunServisi> _logger;

        public UrunServisi(
            UygulamaDbContext context, 
            IDropshippingService dropshippingService,
            ILogger<UrunServisi> logger)
        {
            _context = context;
            _dropshippingService = dropshippingService;
            _logger = logger;
        }

        public async Task<List<Urun>> UrunleriIceriAktarAsync(int tedarikciId, string aramaKelimesi, int maksimumUrun = 50)
        {
            try
            {
                var tedarikci = await _context.Tedarikciler.FindAsync(tedarikciId);
                if (tedarikci == null)
                {
                    throw new ArgumentException("Tedarikçi bulunamadı");
                }

                _logger.LogInformation($"{tedarikci.Ad} tedarikçisinden ürünler içeri aktarılıyor: {aramaKelimesi}");

                var aktarilanUrunler = new List<Urun>();

                // Not: DropshippingService hala eski Product modelini kullanıyor
                // Bu servisi de güncellemek gerekebilir veya adapter pattern kullanabiliriz
                
                // Şimdilik basit bir yaklaşım: manuel ürün oluşturma
                // Gelecekte sayfa ve sayfaBoyutu parametreleri kullanılacak

                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Başarıyla {aktarilanUrunler.Count} ürün içeri aktarıldı");
                return aktarilanUrunler;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Tedarikçi {tedarikciId} için ürün içeri aktarma hatası");
                throw;
            }
        }

        public async Task<int> UrunFiyatlariniVeStoklariniGuncelleAsync(int tedarikciId)
        {
            try
            {
                var tedarikci = await _context.Tedarikciler.FindAsync(tedarikciId);
                if (tedarikci == null) return 0;

                var urunler = await _context.Urunler
                    .Where(p => p.TedarikciId == tedarikciId && p.Aktif)
                    .ToListAsync();

                var guncellenenSayisi = 0;

                foreach (var urun in urunler)
                {
                    try
                    {
                        // Fiyat güncelleme mantığı buraya gelecek
                        // Şimdilik basit bir yaklaşım
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Ürün {urun.Id} güncellenemedi");
                    }
                }

                if (guncellenenSayisi > 0)
                {
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"{tedarikci.Ad} için {guncellenenSayisi} ürün güncellendi");
                return guncellenenSayisi;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Tedarikçi {tedarikciId} için ürün güncelleme hatası");
                return 0;
            }
        }

        public async Task<int> AktifOlmayanUrunleriTemizleAsync(int gunSayisi = 30)
        {
            var kesimTarihi = DateTime.Now.AddDays(-gunSayisi);
            
            var aktifOlmayanUrunler = await _context.Urunler
                .Where(p => !p.Aktif && p.OlusturmaTarihi < kesimTarihi)
                .ToListAsync();

            if (aktifOlmayanUrunler.Any())
            {
                _context.Urunler.RemoveRange(aktifOlmayanUrunler);
                await _context.SaveChangesAsync();
            }

            return aktifOlmayanUrunler.Count;
        }

        // AdminController için ek metodlar
        public async Task<List<Urun>> ImportProductsAsync(int supplierId, string searchTerm, int maxProducts)
        {
            return await UrunleriIceriAktarAsync(supplierId, searchTerm, maxProducts);
        }

        public async Task<int> UpdateProductPricesAndStockAsync(int supplierId)
        {
            return await UrunFiyatlariniVeStoklariniGuncelleAsync(supplierId);
        }

        // YENİ: Gelişmiş filtreleme ve sıralama
        public async Task<UrunListeDto> GetUrunlerAsync(UrunFiltreDto filtre)
        {
            var query = _context.Urunler
                .Include(u => u.Kategori)
                .Include(u => u.Marka)
                .Include(u => u.Kampanyalar)
                    .ThenInclude(uk => uk.Kampanya)
                .AsQueryable();

            query = ApplyFilters(query, filtre);
            var toplamUrun = await query.CountAsync();
            query = ApplySorting(query, filtre.Siralama);

            var urunler = await query
                .Skip((filtre.Sayfa - 1) * filtre.SayfaBoyutu)
                .Take(filtre.SayfaBoyutu)
                .ToListAsync();

            var markaFiltreleri = await GetMarkaFiltreleriAsync(filtre);
            var fiyatAraligi = await GetFiyatAraligiAsync(filtre);
            var stokluUrunSayisi = await GetStokluUrunSayisiAsync(filtre);
            var indirimliUrunSayisi = await GetIndirimliUrunSayisiAsync(filtre);

            return new UrunListeDto
            {
                Urunler = urunler,
                ToplamUrun = toplamUrun,
                MevcutSayfa = filtre.Sayfa,
                ToplamSayfa = (int)Math.Ceiling((double)toplamUrun / filtre.SayfaBoyutu),
                SonrakiSayfaVarMi = filtre.Sayfa * filtre.SayfaBoyutu < toplamUrun,
                OncekiSayfaVarMi = filtre.Sayfa > 1,
                MarkaFiltreleri = markaFiltreleri,
                FiyatAraligi = fiyatAraligi,
                ToplamStokluUrun = stokluUrunSayisi,
                ToplamIndirimliUrun = indirimliUrunSayisi
            };
        }

        private IQueryable<Urun> ApplyFilters(IQueryable<Urun> query, UrunFiltreDto filtre)
        {
            if (!string.IsNullOrWhiteSpace(filtre.AramaTerimi))
            {
                var arama = filtre.AramaTerimi.ToLower();
                query = query.Where(u => 
                    u.Ad.ToLower().Contains(arama) ||
                    u.Aciklama.ToLower().Contains(arama) ||
                    (u.MarkaAdi != null && u.MarkaAdi.ToLower().Contains(arama)) ||
                    (u.Marka != null && u.Marka.Ad.ToLower().Contains(arama)));
            }

            if (filtre.KategoriId.HasValue)
            {
                query = query.Where(u => u.KategoriId == filtre.KategoriId.Value);
            }

            if (filtre.MarkaIdleri != null && filtre.MarkaIdleri.Any())
            {
                query = query.Where(u => u.MarkaId.HasValue && filtre.MarkaIdleri.Contains(u.MarkaId.Value));
            }

            if (filtre.MinFiyat.HasValue)
            {
                query = query.Where(u => u.Fiyat >= filtre.MinFiyat.Value);
            }

            if (filtre.MaxFiyat.HasValue)
            {
                query = query.Where(u => u.Fiyat <= filtre.MaxFiyat.Value);
            }

            if (filtre.SadeceStoktakiler == true)
            {
                query = query.Where(u => u.Stok > 0);
            }

            if (filtre.SadeceIndirimliler == true)
            {
                query = query.Where(u => u.IndirimliFiyat.HasValue && u.IndirimliFiyat.Value < u.Fiyat);
            }

            if (filtre.MinPuan.HasValue)
            {
                query = query.Where(u => u.Puan >= filtre.MinPuan.Value);
            }

            return query;
        }

        private IQueryable<Urun> ApplySorting(IQueryable<Urun> query, UrunSiralamaTuru siralama)
        {
            return siralama switch
            {
                UrunSiralamaTuru.FiyatArtan => query.OrderBy(u => u.IndirimliFiyat ?? u.Fiyat),
                UrunSiralamaTuru.FiyatAzalan => query.OrderByDescending(u => u.IndirimliFiyat ?? u.Fiyat),
                UrunSiralamaTuru.YeniEklenenler => query.OrderByDescending(u => u.OlusturmaTarihi),
                UrunSiralamaTuru.PopulerOlanlar => query.OrderByDescending(u => u.PopuleriteSkor),
                UrunSiralamaTuru.EnCokSatan => query.OrderByDescending(u => u.SatinAlmaSayisi),
                UrunSiralamaTuru.EnYuksekPuan => query.OrderByDescending(u => u.Puan),
                UrunSiralamaTuru.IndirimOrani => query.OrderByDescending(u => 
                    u.IndirimliFiyat.HasValue ? ((u.Fiyat - u.IndirimliFiyat.Value) / u.Fiyat * 100) : 0),
                _ => query.OrderByDescending(u => u.PopuleriteSkor).ThenByDescending(u => u.OlusturmaTarihi)
            };
        }

        private async Task<List<MarkaFiltre>> GetMarkaFiltreleriAsync(UrunFiltreDto filtre)
        {
            var query = _context.Urunler.AsQueryable();

            if (filtre.KategoriId.HasValue)
            {
                query = query.Where(u => u.KategoriId == filtre.KategoriId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtre.AramaTerimi))
            {
                var arama = filtre.AramaTerimi.ToLower();
                query = query.Where(u => u.Ad.ToLower().Contains(arama) || u.Aciklama.ToLower().Contains(arama));
            }

            var markalar = await query
                .Where(u => u.MarkaId.HasValue)
                .GroupBy(u => new { u.MarkaId, u.Marka!.Ad })
                .Select(g => new MarkaFiltre
                {
                    Id = g.Key.MarkaId!.Value,
                    Ad = g.Key.Ad,
                    UrunSayisi = g.Count(),
                    Secili = filtre.MarkaIdleri != null && filtre.MarkaIdleri.Contains(g.Key.MarkaId!.Value)
                })
                .OrderByDescending(m => m.UrunSayisi)
                .Take(20)
                .ToListAsync();

            return markalar;
        }

        private async Task<FiyatAralik> GetFiyatAraligiAsync(UrunFiltreDto filtre)
        {
            var query = _context.Urunler.AsQueryable();

            if (filtre.KategoriId.HasValue)
            {
                query = query.Where(u => u.KategoriId == filtre.KategoriId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtre.AramaTerimi))
            {
                var arama = filtre.AramaTerimi.ToLower();
                query = query.Where(u => u.Ad.ToLower().Contains(arama) || u.Aciklama.ToLower().Contains(arama));
            }

            var minFiyat = await query.MinAsync(u => (decimal?)(u.IndirimliFiyat ?? u.Fiyat)) ?? 0;
            var maxFiyat = await query.MaxAsync(u => (decimal?)(u.IndirimliFiyat ?? u.Fiyat)) ?? 0;

            return new FiyatAralik
            {
                MinFiyat = minFiyat,
                MaxFiyat = maxFiyat
            };
        }

        private async Task<int> GetStokluUrunSayisiAsync(UrunFiltreDto filtre)
        {
            var query = _context.Urunler.AsQueryable();
            query = ApplyFilters(query, filtre);
            return await query.CountAsync(u => u.Stok > 0);
        }

        private async Task<int> GetIndirimliUrunSayisiAsync(UrunFiltreDto filtre)
        {
            var query = _context.Urunler.AsQueryable();
            query = ApplyFilters(query, filtre);
            return await query.CountAsync(u => u.IndirimliFiyat.HasValue && u.IndirimliFiyat.Value < u.Fiyat);
        }

        // SEO ve Slug işlemleri
        public async Task<string> GenerateSlugAsync(string urunAdi, int? excludeId = null)
        {
            var slug = urunAdi.ToLowerInvariant()
                .Replace('ç', 'c').Replace('ğ', 'g').Replace('ı', 'i')
                .Replace('ö', 'o').Replace('ş', 's').Replace('ü', 'u');
            
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = slug.Trim('-');
            
            var originalSlug = slug;
            var counter = 1;
            
            while (await SlugExistsAsync(slug, excludeId))
            {
                slug = $"{originalSlug}-{counter}";
                counter++;
            }
            
            return slug;
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            var query = _context.Urunler.Where(u => u.Slug == slug);
            
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task<Urun?> GetBySlugAsync(string slug)
        {
            return await _context.Urunler
                .Include(u => u.Kategori)
                .Include(u => u.Marka)
                .Include(u => u.Kampanyalar)
                    .ThenInclude(uk => uk.Kampanya)
                .FirstOrDefaultAsync(u => u.Slug == slug);
        }

        // Popülerlik güncelleme
        public async Task UpdatePopulerlikSkoruAsync(int urunId)
        {
            var urun = await _context.Urunler.FindAsync(urunId);
            if (urun == null) return;

            urun.GoruntulenmeSayisi++;
            
            // Popülerlik skoru hesaplama: görüntülenme, satış, puan
            var goruntulemePuani = Math.Min(urun.GoruntulenmeSayisi / 100.0, 40);
            var satisPuani = Math.Min(urun.SatinAlmaSayisi * 2, 40);
            var puanPuani = urun.Puan * 4;
            
            urun.PopuleriteSkor = (decimal)(goruntulemePuani + satisPuani + puanPuani);
            
            await _context.SaveChangesAsync();
        }
    }
}
