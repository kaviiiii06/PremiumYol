using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public class RaporService : IRaporService
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<RaporService> _logger;

        public RaporService(UygulamaDbContext context, ILogger<RaporService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GenelRaporOzetiDto> GetGenelOzetAsync(RaporFiltre filtre)
        {
            var siparisler = await _context.Siparisler
                .Where(s => s.SiparisTarihi >= filtre.BaslangicTarihi && s.SiparisTarihi <= filtre.BitisTarihi)
                .Include(s => s.SiparisKalemleri)
                .ToListAsync();

            var toplamSatis = siparisler.Sum(s => s.ToplamTutar);
            var toplamSiparis = siparisler.Count;
            var ortalamaSeperTutari = toplamSiparis > 0 ? toplamSatis / toplamSiparis : 0;
            var toplamUrunSatisi = siparisler.SelectMany(s => s.SiparisKalemleri).Sum(sk => sk.Miktar);

            var yeniKullanici = await _context.Kullanicilar
                .Where(k => k.OlusturmaTarihi >= filtre.BaslangicTarihi && k.OlusturmaTarihi <= filtre.BitisTarihi)
                .CountAsync();

            var iadeTutari = await _context.Iadeler
                .Where(i => i.TalepTarihi >= filtre.BaslangicTarihi && i.TalepTarihi <= filtre.BitisTarihi)
                .SumAsync(i => i.IadeTutari);

            // Önceki dönem karşılaştırması
            var donemUzunlugu = (filtre.BitisTarihi - filtre.BaslangicTarihi).Days;
            var oncekiBaslangic = filtre.BaslangicTarihi.AddDays(-donemUzunlugu);
            var oncekiBitis = filtre.BaslangicTarihi.AddDays(-1);

            var oncekiSiparisler = await _context.Siparisler
                .Where(s => s.SiparisTarihi >= oncekiBaslangic && s.SiparisTarihi <= oncekiBitis)
                .ToListAsync();

            var oncekiToplamSatis = oncekiSiparisler.Sum(s => s.ToplamTutar);
            var oncekiToplamSiparis = oncekiSiparisler.Count;

            var satisArtisi = oncekiToplamSatis > 0 
                ? ((toplamSatis - oncekiToplamSatis) / oncekiToplamSatis) * 100 
                : 0;

            var siparisArtisi = oncekiToplamSiparis > 0 
                ? ((decimal)(toplamSiparis - oncekiToplamSiparis) / oncekiToplamSiparis) * 100 
                : 0;

            return new GenelRaporOzetiDto
            {
                ToplamSatis = toplamSatis,
                ToplamSiparis = toplamSiparis,
                OrtalamaSeperTutari = ortalamaSeperTutari,
                ToplamUrunSatisi = toplamUrunSatisi,
                YeniKullanici = yeniKullanici,
                IadeTutari = iadeTutari,
                NetKar = toplamSatis - iadeTutari,
                SatisArtisi = satisArtisi,
                SiparisArtisi = siparisArtisi
            };
        }

        public async Task<List<SatisRaporuDto>> GetSatisRaporuAsync(RaporFiltre filtre)
        {
            var siparisler = await _context.Siparisler
                .Where(s => s.SiparisTarihi >= filtre.BaslangicTarihi && s.SiparisTarihi <= filtre.BitisTarihi)
                .Include(s => s.SiparisKalemleri)
                .ToListAsync();

            var grupluSiparisler = siparisler
                .GroupBy(s => s.SiparisTarihi.Date)
                .Select(g => new SatisRaporuDto
                {
                    Tarih = g.Key,
                    ToplamSatis = g.Sum(s => s.ToplamTutar),
                    SiparisSayisi = g.Count(),
                    OrtalamaSeperTutari = g.Count() > 0 ? g.Sum(s => s.ToplamTutar) / g.Count() : 0,
                    UrunSayisi = g.SelectMany(s => s.SiparisKalemleri).Sum(sk => sk.Miktar)
                })
                .OrderBy(r => r.Tarih)
                .ToList();

            return grupluSiparisler;
        }

        public async Task<List<UrunSatisRaporuDto>> GetUrunSatisRaporuAsync(RaporFiltre filtre)
        {
            var query = _context.SiparisKalemleri
                .Include(sk => sk.Siparis)
                .Include(sk => sk.Urun)
                    .ThenInclude(u => u.Kategori)
                .Where(sk => sk.Siparis.SiparisTarihi >= filtre.BaslangicTarihi && 
                             sk.Siparis.SiparisTarihi <= filtre.BitisTarihi);

            if (filtre.KategoriId.HasValue)
            {
                query = query.Where(sk => sk.Urun.KategoriId == filtre.KategoriId.Value);
            }

            var urunSatislari = await query
                .GroupBy(sk => new { sk.UrunId, UrunAdi = sk.Urun.Ad, KategoriAdi = sk.Urun.Kategori.Ad, sk.Urun.ResimUrl, sk.Urun.Stok })
                .Select(g => new UrunSatisRaporuDto
                {
                    UrunId = g.Key.UrunId,
                    UrunAdi = g.Key.UrunAdi,
                    KategoriAdi = g.Key.KategoriAdi,
                    ResimUrl = g.Key.ResimUrl,
                    SatisSayisi = g.Sum(sk => sk.Miktar),
                    ToplamGelir = g.Sum(sk => sk.BirimFiyat * sk.Miktar),
                    StokMiktari = g.Key.Stok
                })
                .OrderByDescending(u => u.SatisSayisi)
                .Take(filtre.Limit)
                .ToListAsync();

            return urunSatislari;
        }

        public async Task<List<KategoriSatisRaporuDto>> GetKategoriSatisRaporuAsync(RaporFiltre filtre)
        {
            var kategoriSatislari = await _context.SiparisKalemleri
                .Include(sk => sk.Siparis)
                .Include(sk => sk.Urun)
                    .ThenInclude(u => u.Kategori)
                .Where(sk => sk.Siparis.SiparisTarihi >= filtre.BaslangicTarihi && 
                             sk.Siparis.SiparisTarihi <= filtre.BitisTarihi)
                .GroupBy(sk => new { sk.Urun.KategoriId, sk.Urun.Kategori.Ad })
                .Select(g => new KategoriSatisRaporuDto
                {
                    KategoriId = g.Key.KategoriId,
                    KategoriAdi = g.Key.Ad,
                    SatisSayisi = g.Sum(sk => sk.Miktar),
                    ToplamGelir = g.Sum(sk => sk.BirimFiyat * sk.Miktar)
                })
                .OrderByDescending(k => k.ToplamGelir)
                .ToListAsync();

            // Ürün sayısı ve yüzde hesapla
            var toplamGelir = kategoriSatislari.Sum(k => k.ToplamGelir);
            foreach (var kategori in kategoriSatislari)
            {
                kategori.UrunSayisi = await _context.Urunler.CountAsync(u => u.KategoriId == kategori.KategoriId);
                kategori.Yuzde = toplamGelir > 0 ? (kategori.ToplamGelir / toplamGelir) * 100 : 0;
            }

            return kategoriSatislari;
        }

        public async Task<List<KullaniciRaporuDto>> GetKullaniciRaporuAsync(RaporFiltre filtre)
        {
            var tarihler = Enumerable.Range(0, (filtre.BitisTarihi - filtre.BaslangicTarihi).Days + 1)
                .Select(i => filtre.BaslangicTarihi.AddDays(i).Date)
                .ToList();

            var rapor = new List<KullaniciRaporuDto>();

            foreach (var tarih in tarihler)
            {
                var yeniKayit = await _context.Kullanicilar
                    .CountAsync(k => k.OlusturmaTarihi.Date == tarih);

                var siparisVerenKullanici = await _context.Siparisler
                    .Where(s => s.SiparisTarihi.Date == tarih)
                    .Select(s => s.KullaniciId)
                    .Distinct()
                    .CountAsync();

                rapor.Add(new KullaniciRaporuDto
                {
                    Tarih = tarih,
                    YeniKayit = yeniKayit,
                    SiparisVerenKullanici = siparisVerenKullanici,
                    AktifKullanici = siparisVerenKullanici // Basitleştirilmiş
                });
            }

            return rapor;
        }

        public async Task<List<FinansalRaporDto>> GetFinansalRaporAsync(RaporFiltre filtre)
        {
            var tarihler = Enumerable.Range(0, (filtre.BitisTarihi - filtre.BaslangicTarihi).Days + 1)
                .Select(i => filtre.BaslangicTarihi.AddDays(i).Date)
                .ToList();

            var rapor = new List<FinansalRaporDto>();

            foreach (var tarih in tarihler)
            {
                var siparisler = await _context.Siparisler
                    .Where(s => s.SiparisTarihi.Date == tarih)
                    .ToListAsync();

                var iadeler = await _context.Iadeler
                    .Where(i => i.TalepTarihi.Date == tarih)
                    .ToListAsync();

                var brutSatis = siparisler.Sum(s => s.ToplamTutar);
                var kargoGelirleri = siparisler.Sum(s => s.KargoUcreti);
                var indirimTutari = siparisler.Sum(s => s.IndirimTutari);
                var iadeTutari = iadeler.Sum(i => i.IadeTutari);

                rapor.Add(new FinansalRaporDto
                {
                    Tarih = tarih,
                    BrutSatis = brutSatis,
                    KargoGelirleri = kargoGelirleri,
                    IndirimTutari = indirimTutari,
                    IadeTutari = iadeTutari,
                    NetSatis = brutSatis - iadeTutari
                });
            }

            return rapor;
        }
    }
}
