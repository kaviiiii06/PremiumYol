using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public class AramaService : IAramaService
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<AramaService> _logger;

        public AramaService(UygulamaDbContext context, ILogger<AramaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<OtomatikTamamlamaDto>> OtomatikTamamlamaGetirAsync(string terim, int kullaniciId = 0)
        {
            if (string.IsNullOrWhiteSpace(terim) || terim.Length < 2)
                return new List<OtomatikTamamlamaDto>();

            var sonuclar = new List<OtomatikTamamlamaDto>();
            terim = terim.ToLower().Trim();

            // Ürün adlarından öneriler
            var urunler = await _context.Urunler
                .Where(u => u.Ad.ToLower().Contains(terim))
                .Select(u => new { UrunAd = u.Ad, KategoriAd = u.Kategori.Ad })
                .Take(5)
                .ToListAsync();

            foreach (var urun in urunler)
            {
                sonuclar.Add(new OtomatikTamamlamaDto
                {
                    Terim = urun.UrunAd,
                    Tip = "Ürün",
                    Kategori = urun.KategoriAd
                });
            }

            // Kategori adlarından öneriler
            var kategoriler = await _context.Kategoriler
                .Where(k => k.Ad.ToLower().Contains(terim))
                .Select(k => k.Ad)
                .Take(3)
                .ToListAsync();

            foreach (var kategori in kategoriler)
            {
                sonuclar.Add(new OtomatikTamamlamaDto
                {
                    Terim = kategori,
                    Tip = "Kategori"
                });
            }

            // Marka adlarından öneriler
            var markalar = await _context.Markalar
                .Where(m => m.Ad.ToLower().Contains(terim))
                .Select(m => m.Ad)
                .Take(3)
                .ToListAsync();

            foreach (var marka in markalar)
            {
                sonuclar.Add(new OtomatikTamamlamaDto
                {
                    Terim = marka,
                    Tip = "Marka"
                });
            }

            // Kullanıcının arama geçmişinden öneriler
            if (kullaniciId > 0)
            {
                var gecmis = await _context.AramaGecmisleri
                    .Where(a => a.KullaniciId == kullaniciId && a.AramaTerimi.ToLower().Contains(terim))
                    .OrderByDescending(a => a.AramaTarihi)
                    .Select(a => a.AramaTerimi)
                    .Distinct()
                    .Take(2)
                    .ToListAsync();

                foreach (var g in gecmis)
                {
                    sonuclar.Add(new OtomatikTamamlamaDto
                    {
                        Terim = g,
                        Tip = "Geçmiş"
                    });
                }
            }

            return sonuclar.Take(10).ToList();
        }

        public async Task<AramaSonucDto> AramaYapAsync(string terim, int kullaniciId = 0, int sayfa = 1, int sayfaBoyutu = 20)
        {
            var sonuc = new AramaSonucDto
            {
                AramaTerimi = terim,
                Sayfa = sayfa,
                SayfaBoyutu = sayfaBoyutu
            };

            if (string.IsNullOrWhiteSpace(terim))
                return sonuc;

            terim = terim.ToLower().Trim();

            // Arama geçmişine ekle
            if (kullaniciId > 0)
            {
                await AramaGecmisiEkleAsync(kullaniciId, terim);
            }

            // Popüler aramayı güncelle
            await PopulerAramaGuncelleAsync(terim);

            // Ürünleri ara
            var query = _context.Urunler
                .Include(u => u.Kategori)
                .Include(u => u.Marka)
                .Where(u => u.Ad.ToLower().Contains(terim) || 
                           u.Aciklama.ToLower().Contains(terim) ||
                           u.Kategori.Ad.ToLower().Contains(terim) ||
                           (u.Marka != null && u.Marka.Ad.ToLower().Contains(terim)));

            sonuc.ToplamSonuc = await query.CountAsync();
            sonuc.ToplamSayfa = (int)Math.Ceiling((double)sonuc.ToplamSonuc / sayfaBoyutu);

            sonuc.Urunler = await query
                .OrderByDescending(u => u.Id)
                .Skip((sayfa - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .ToListAsync();

            // İlgili kategoriler
            sonuc.Filtreler.Kategoriler = await _context.Urunler
                .Where(u => u.Ad.ToLower().Contains(terim) || u.Aciklama.ToLower().Contains(terim))
                .GroupBy(u => new { u.Kategori.Id, u.Kategori.Ad })
                .Select(g => new KategoriFiltre
                {
                    Id = g.Key.Id,
                    Ad = g.Key.Ad,
                    UrunSayisi = g.Count()
                })
                .Take(5)
                .ToListAsync();

            // İlgili markalar
            sonuc.Filtreler.Markalar = await _context.Urunler
                .Where(u => u.Ad.ToLower().Contains(terim) || u.Aciklama.ToLower().Contains(terim))
                .Where(u => u.Marka != null)
                .GroupBy(u => new { u.Marka!.Id, u.Marka.Ad })
                .Select(g => new MarkaFiltre
                {
                    Id = g.Key.Id,
                    Ad = g.Key.Ad,
                    UrunSayisi = g.Count()
                })
                .Take(5)
                .ToListAsync();

            return sonuc;
        }

        public async Task AramaGecmisiEkleAsync(int kullaniciId, string terim)
        {
            try
            {
                var gecmis = new AramaGecmisi
                {
                    KullaniciId = kullaniciId,
                    AramaTerimi = terim,
                    AramaTarihi = DateTime.Now
                };

                _context.AramaGecmisleri.Add(gecmis);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arama geçmişi eklenirken hata oluştu");
            }
        }

        public async Task<List<string>> AramaGecmisiGetirAsync(int kullaniciId, int adet = 10)
        {
            return await _context.AramaGecmisleri
                .Where(a => a.KullaniciId == kullaniciId)
                .OrderByDescending(a => a.AramaTarihi)
                .Select(a => a.AramaTerimi)
                .Distinct()
                .Take(adet)
                .ToListAsync();
        }

        public async Task AramaGecmisiTemizleAsync(int kullaniciId)
        {
            var gecmis = await _context.AramaGecmisleri
                .Where(a => a.KullaniciId == kullaniciId)
                .ToListAsync();

            _context.AramaGecmisleri.RemoveRange(gecmis);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> PopulerAramalarGetirAsync(int adet = 10)
        {
            return await _context.PopulerAramalar
                .OrderByDescending(p => p.AramaSayisi)
                .ThenByDescending(p => p.SonAramaTarihi)
                .Select(p => p.AramaTerimi)
                .Take(adet)
                .ToListAsync();
        }

        public async Task PopulerAramaGuncelleAsync(string terim)
        {
            try
            {
                var populer = await _context.PopulerAramalar
                    .FirstOrDefaultAsync(p => p.AramaTerimi == terim);

                if (populer != null)
                {
                    populer.AramaSayisi++;
                    populer.SonAramaTarihi = DateTime.Now;
                }
                else
                {
                    populer = new PopulerArama
                    {
                        AramaTerimi = terim,
                        AramaSayisi = 1,
                        SonAramaTarihi = DateTime.Now
                    };
                    _context.PopulerAramalar.Add(populer);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Popüler arama güncellenirken hata oluştu");
            }
        }

        public async Task AramaTiklamaKaydetAsync(string terim, int urunId, int kullaniciId = 0)
        {
            try
            {
                // Önce arama geçmişini bul veya oluştur
                var aramaGecmisi = await _context.AramaGecmisleri
                    .Where(a => a.KullaniciId == kullaniciId && a.AramaTerimi == terim)
                    .OrderByDescending(a => a.AramaTarihi)
                    .FirstOrDefaultAsync();

                if (aramaGecmisi == null)
                {
                    aramaGecmisi = new AramaGecmisi
                    {
                        KullaniciId = kullaniciId > 0 ? kullaniciId : null,
                        AramaTerimi = terim,
                        AramaTarihi = DateTime.Now
                    };
                    _context.AramaGecmisleri.Add(aramaGecmisi);
                    await _context.SaveChangesAsync();
                }

                var tiklama = new AramaTiklama
                {
                    AramaGecmisiId = aramaGecmisi.Id,
                    UrunId = urunId,
                    TiklamaTarihi = DateTime.Now,
                    Sira = 0
                };

                _context.AramaTiklamalari.Add(tiklama);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arama tıklama kaydedilirken hata oluştu");
            }
        }

        public async Task<List<string>> AramaOnerileriGetirAsync(int kullaniciId, int adet = 5)
        {
            var oneriler = new List<string>();

            // Kullanıcının son aramalarından
            var sonAramalar = await _context.AramaGecmisleri
                .Where(a => a.KullaniciId == kullaniciId)
                .OrderByDescending(a => a.AramaTarihi)
                .Select(a => a.AramaTerimi)
                .Distinct()
                .Take(3)
                .ToListAsync();

            oneriler.AddRange(sonAramalar);

            // Popüler aramalardan
            if (oneriler.Count < adet)
            {
                var populer = await PopulerAramalarGetirAsync(adet - oneriler.Count);
                oneriler.AddRange(populer.Where(p => !oneriler.Contains(p)));
            }

            return oneriler.Take(adet).ToList();
        }
    }
}
