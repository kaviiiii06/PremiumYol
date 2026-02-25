using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly UygulamaDbContext _context;
        private readonly ICacheService _cache;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            UygulamaDbContext context,
            ICacheService cache,
            ILogger<DashboardService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var cacheKey = "dashboard_data";
            var cachedData = _cache.Get<DashboardDto>(cacheKey);
            
            if (cachedData != null)
            {
                return cachedData;
            }

            var dashboard = new DashboardDto
            {
                Istatistikler = await GetIstatistiklerAsync(),
                GunlukSatislar = await GetGunlukSatislarAsync(30),
                PopulerUrunler = await GetPopulerUrunlerAsync(10),
                SonSiparisler = await GetSonSiparisleriAsync(10),
                StokUyarilari = await GetStokUyarilariAsync(),
                YeniKullanicilar = await GetYeniKullanicilariAsync(7)
            };

            _cache.Set(cacheKey, dashboard, TimeSpan.FromMinutes(5));

            return dashboard;
        }

        public async Task<DashboardIstatistikler> GetIstatistiklerAsync()
        {
            var bugun = DateTime.Today;
            var ayBaslangic = new DateTime(bugun.Year, bugun.Month, 1);
            var oncekiAyBaslangic = ayBaslangic.AddMonths(-1);
            var oncekiAyBitis = ayBaslangic.AddDays(-1);

            // Bugünkü satışlar
            var bugunkuSiparisler = await _context.Siparisler
                .Where(s => s.SiparisTarihi.Date == bugun)
                .ToListAsync();

            var bugunkuSatis = bugunkuSiparisler.Sum(s => s.ToplamTutar);
            var bugunkuSiparisSayisi = bugunkuSiparisler.Count;

            // Aylık satışlar
            var aylikSiparisler = await _context.Siparisler
                .Where(s => s.SiparisTarihi >= ayBaslangic)
                .ToListAsync();

            var aylikSatis = aylikSiparisler.Sum(s => s.ToplamTutar);
            var aylikSiparisSayisi = aylikSiparisler.Count;

            // Önceki ay satışlar (karşılaştırma için)
            var oncekiAySiparisler = await _context.Siparisler
                .Where(s => s.SiparisTarihi >= oncekiAyBaslangic && s.SiparisTarihi <= oncekiAyBitis)
                .ToListAsync();

            var oncekiAySatis = oncekiAySiparisler.Sum(s => s.ToplamTutar);
            var oncekiAySiparisSayisi = oncekiAySiparisler.Count;

            // Artış yüzdeleri
            var satisArtisi = oncekiAySatis > 0 
                ? ((aylikSatis - oncekiAySatis) / oncekiAySatis) * 100 
                : 0;

            var siparisArtisi = oncekiAySiparisSayisi > 0 
                ? ((decimal)(aylikSiparisSayisi - oncekiAySiparisSayisi) / oncekiAySiparisSayisi) * 100 
                : 0;

            // Kullanıcı istatistikleri
            var toplamKullanici = await _context.Kullanicilar.CountAsync();
            var aktifKullanici = await _context.Kullanicilar
                .Where(k => k.Aktif)
                .CountAsync();

            // Bekleyen siparişler
            var bekleyenSiparis = await _context.Siparisler
                .Where(s => s.Durum == SiparisDurumu.Hazirlaniyor || s.Durum == SiparisDurumu.Onaylandi)
                .CountAsync();

            // Bekleyen iadeler
            var bekleyenIade = await _context.Iadeler
                .Where(i => i.Durum == IadeDurumu.TalepEdildi || i.Durum == IadeDurumu.OnayBekliyor)
                .CountAsync();

            // Stok uyarıları
            var dusukStokUrun = await _context.Urunler
                .Where(u => u.Stok > 0 && u.Stok <= 10)
                .CountAsync();

            var tukenenUrun = await _context.Urunler
                .Where(u => u.Stok == 0)
                .CountAsync();

            return new DashboardIstatistikler
            {
                BugunkuSatis = bugunkuSatis,
                BugunkuSiparisSayisi = bugunkuSiparisSayisi,
                AylikSatis = aylikSatis,
                AylikSiparisSayisi = aylikSiparisSayisi,
                ToplamKullanici = toplamKullanici,
                AktifKullanici = aktifKullanici,
                BekleyenSiparis = bekleyenSiparis,
                BekleyenIade = bekleyenIade,
                DusukStokUrun = dusukStokUrun,
                TukenenUrun = tukenenUrun,
                SatisArtisi = satisArtisi,
                SiparisArtisi = siparisArtisi
            };
        }

        public async Task<List<GunlukSatis>> GetGunlukSatislarAsync(int gunSayisi = 30)
        {
            var baslangicTarihi = DateTime.Today.AddDays(-gunSayisi);

            var siparisler = await _context.Siparisler
                .Where(s => s.SiparisTarihi >= baslangicTarihi)
                .GroupBy(s => s.SiparisTarihi.Date)
                .Select(g => new GunlukSatis
                {
                    Tarih = g.Key,
                    Tutar = g.Sum(s => s.ToplamTutar),
                    SiparisSayisi = g.Count()
                })
                .OrderBy(g => g.Tarih)
                .ToListAsync();

            return siparisler;
        }

        public async Task<List<PopulerUrun>> GetPopulerUrunlerAsync(int adet = 10)
        {
            var sonAy = DateTime.Today.AddDays(-30);

            var populerUrunler = await _context.SiparisKalemleri
                .Where(sk => sk.Siparis.SiparisTarihi >= sonAy)
                .GroupBy(sk => new { sk.UrunId, sk.Urun.Ad, sk.Urun.ResimUrl })
                .Select(g => new PopulerUrun
                {
                    UrunId = g.Key.UrunId,
                    UrunAdi = g.Key.Ad,
                    ResimUrl = g.Key.ResimUrl ?? "/images/no-image.jpg",
                    SatisSayisi = g.Sum(sk => sk.Adet),
                    ToplamGelir = g.Sum(sk => sk.BirimFiyat * sk.Adet)
                })
                .OrderByDescending(p => p.SatisSayisi)
                .Take(adet)
                .ToListAsync();

            return populerUrunler;
        }

        public async Task<List<SonSiparis>> GetSonSiparisleriAsync(int adet = 10)
        {
            var sonSiparisler = await _context.Siparisler
                .Include(s => s.Kullanici)
                .OrderByDescending(s => s.SiparisTarihi)
                .Take(adet)
                .Select(s => new SonSiparis
                {
                    SiparisId = s.Id,
                    SiparisNo = s.SiparisNo,
                    KullaniciAdi = s.Kullanici.Ad + " " + s.Kullanici.Soyad,
                    ToplamTutar = s.ToplamTutar,
                    Durum = s.Durum.ToString(),
                    Tarih = s.SiparisTarihi
                })
                .ToListAsync();

            return sonSiparisler;
        }

        public async Task<List<StokUyari>> GetStokUyarilariAsync()
        {
            var dusukStokUrunler = await _context.Urunler
                .Where(u => u.Stok <= 10)
                .OrderBy(u => u.Stok)
                .Take(20)
                .Select(u => new StokUyari
                {
                    UrunId = u.Id,
                    UrunAdi = u.Ad,
                    ResimUrl = u.ResimUrl ?? "/images/no-image.jpg",
                    MevcutStok = u.Stok,
                    MinimumStok = 10,
                    UyariSeviyesi = u.Stok == 0 ? "Kritik" : u.Stok <= 5 ? "Çok Düşük" : "Düşük"
                })
                .ToListAsync();

            return dusukStokUrunler;
        }

        public async Task<List<YeniKullanici>> GetYeniKullanicilariAsync(int gunSayisi = 7)
        {
            var baslangicTarihi = DateTime.Today.AddDays(-gunSayisi);

            var yeniKullanicilar = await _context.Kullanicilar
                .Where(k => k.OlusturmaTarihi >= baslangicTarihi)
                .OrderByDescending(k => k.OlusturmaTarihi)
                .Take(10)
                .Select(k => new YeniKullanici
                {
                    KullaniciId = k.Id,
                    AdSoyad = k.Ad + " " + k.Soyad,
                    Email = k.Email,
                    KayitTarihi = k.OlusturmaTarihi,
                    SiparisSayisi = k.Siparisler != null ? k.Siparisler.Count : 0
                })
                .ToListAsync();

            return yeniKullanicilar;
        }
    }
}
