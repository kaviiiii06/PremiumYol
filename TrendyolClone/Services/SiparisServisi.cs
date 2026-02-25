using TrendyolClone.Data;
using TrendyolClone.Models;
using Microsoft.EntityFrameworkCore;

namespace TrendyolClone.Services
{
    public class SiparisServisi
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<SiparisServisi> _logger;

        public SiparisServisi(UygulamaDbContext context, ILogger<SiparisServisi> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Siparis> SiparisOlusturAsync(int kullaniciId, List<SepetKalemi> sepetKalemleri, int adresId)
        {
            try
            {
                var adres = await _context.Adresler.FindAsync(adresId);
                if (adres == null || adres.KullaniciId != kullaniciId)
                {
                    throw new ArgumentException("Geçersiz adres");
                }

                var siparis = new Siparis
                {
                    KullaniciId = kullaniciId,
                    SiparisTarihi = DateTime.Now,
                    Durum = SiparisDurumu.Beklemede,
                    TeslimatAdresi = $"{adres.AdresSatiri}, {adres.Ilce}, {adres.Sehir}",
                    SiparisKalemleri = new List<SiparisKalemi>()
                };

                decimal toplamTutar = 0;

                foreach (var sepetKalemi in sepetKalemleri)
                {
                    var urun = await _context.Urunler.FindAsync(sepetKalemi.UrunId);
                    if (urun == null || !urun.Aktif)
                    {
                        throw new ArgumentException($"Ürün bulunamadı: {sepetKalemi.UrunAdi}");
                    }

                    if (urun.Stok < sepetKalemi.Miktar)
                    {
                        throw new ArgumentException($"Yetersiz stok: {urun.Ad}");
                    }

                    var siparisKalemi = new SiparisKalemi
                    {
                        UrunId = urun.Id,
                        Miktar = sepetKalemi.Miktar,
                        BirimFiyat = urun.IndirimliFiyat ?? urun.Fiyat
                    };

                    siparis.SiparisKalemleri.Add(siparisKalemi);
                    toplamTutar += siparisKalemi.ToplamFiyat;

                    // Stok düş
                    urun.Stok -= sepetKalemi.Miktar;
                }

                siparis.ToplamTutar = toplamTutar;

                _context.Siparisler.Add(siparis);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Sipariş oluşturuldu: {siparis.Id}, Kullanıcı: {kullaniciId}");

                return siparis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Sipariş oluşturma hatası, Kullanıcı: {kullaniciId}");
                throw;
            }
        }

        public async Task<bool> SiparisDurumunuGuncelleAsync(int siparisId, SiparisDurumu yeniDurum)
        {
            try
            {
                var siparis = await _context.Siparisler.FindAsync(siparisId);
                if (siparis == null)
                {
                    return false;
                }

                siparis.Durum = yeniDurum;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Sipariş durumu güncellendi: {siparisId}, Yeni durum: {yeniDurum}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Sipariş durumu güncelleme hatası: {siparisId}");
                return false;
            }
        }

        public async Task<List<Siparis>> KullaniciSiparisleriniGetirAsync(int kullaniciId)
        {
            return await _context.Siparisler
                .Include(s => s.SiparisKalemleri)
                    .ThenInclude(sk => sk.Urun)
                .Where(s => s.KullaniciId == kullaniciId)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();
        }

        public async Task<Siparis> SiparisDetayiniGetirAsync(int siparisId, int kullaniciId)
        {
            return await _context.Siparisler
                .Include(s => s.SiparisKalemleri)
                    .ThenInclude(sk => sk.Urun)
                .Include(s => s.Kullanici)
                .FirstOrDefaultAsync(s => s.Id == siparisId && s.KullaniciId == kullaniciId);
        }
    }
}
