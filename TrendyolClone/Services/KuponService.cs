using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public class KuponService : IKuponService
    {
        private readonly UygulamaDbContext _context;

        public KuponService(UygulamaDbContext context)
        {
            _context = context;
        }

        public async Task<Kupon?> GetByKodAsync(string kod)
        {
            return await _context.Kuponlar
                .Include(k => k.GecerliKategoriler)
                .Include(k => k.Kullanimlar)
                .FirstOrDefaultAsync(k => k.Kod == kod.ToUpper());
        }

        public async Task<Kupon?> GetByIdAsync(int id)
        {
            return await _context.Kuponlar
                .Include(k => k.GecerliKategoriler)
                .Include(k => k.Kullanimlar)
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<List<Kupon>> GetAktifKuponlarAsync()
        {
            return await _context.Kuponlar
                .Where(k => k.Aktif && 
                           k.BaslangicTarihi <= DateTime.Now && 
                           k.BitisTarihi >= DateTime.Now)
                .OrderBy(k => k.BaslangicTarihi)
                .ToListAsync();
        }

        public async Task<List<Kupon>> GetKullaniciKuponlariAsync(int kullaniciId)
        {
            var kullanilanKuponlar = await _context.KuponKullanimlari
                .Where(kk => kk.KullaniciId == kullaniciId)
                .Select(kk => kk.KuponId)
                .Distinct()
                .ToListAsync();

            return await _context.Kuponlar
                .Where(k => kullanilanKuponlar.Contains(k.Id))
                .ToListAsync();
        }

        public async Task<bool> KuponGecerliMi(string kod, int kullaniciId, decimal sepetTutari, List<int>? kategoriIdleri = null)
        {
            var hata = await KuponHataMesaji(kod, kullaniciId, sepetTutari, kategoriIdleri);
            return hata == null;
        }

        public async Task<string?> KuponHataMesaji(string kod, int kullaniciId, decimal sepetTutari, List<int>? kategoriIdleri = null)
        {
            var kupon = await GetByKodAsync(kod);
            
            if (kupon == null)
                return "Kupon bulunamadı!";
            
            if (!kupon.Aktif)
                return "Kupon aktif değil!";
            
            if (DateTime.Now < kupon.BaslangicTarihi)
                return $"Kupon {kupon.BaslangicTarihi:dd.MM.yyyy} tarihinde geçerli olacak!";
            
            if (DateTime.Now > kupon.BitisTarihi)
                return "Kupon süresi dolmuş!";
            
            if (kupon.MinimumSepetTutari.HasValue && sepetTutari < kupon.MinimumSepetTutari.Value)
                return $"Minimum sepet tutarı {kupon.MinimumSepetTutari.Value:C} olmalıdır!";
            
            if (kupon.MaksimumKullanimSayisi.HasValue && kupon.KullanimSayisi >= kupon.MaksimumKullanimSayisi.Value)
                return "Kupon kullanım limiti dolmuş!";
            
            if (kupon.KullaniciBasinaKullanimSayisi.HasValue)
            {
                var kullanimSayisi = await GetKullaniciKullanimSayisiAsync(kupon.Id, kullaniciId);
                if (kullanimSayisi >= kupon.KullaniciBasinaKullanimSayisi.Value)
                    return "Bu kuponu daha önce kullandınız!";
            }
            
            // Kategori kontrolü
            if (kupon.GecerliKategoriler.Any() && kategoriIdleri != null && kategoriIdleri.Any())
            {
                var gecerliKategoriIdleri = kupon.GecerliKategoriler.Select(kk => kk.KategoriId).ToList();
                if (!kategoriIdleri.Any(kid => gecerliKategoriIdleri.Contains(kid)))
                    return "Kupon sepetinizdeki ürünler için geçerli değil!";
            }
            
            return null;
        }

        public async Task<decimal> HesaplaIndirimiAsync(string kod, decimal sepetTutari)
        {
            var kupon = await GetByKodAsync(kod);
            if (kupon == null) return 0;
            
            return await HesaplaIndirimiAsync(kupon, sepetTutari);
        }

        public async Task<decimal> HesaplaIndirimiAsync(Kupon kupon, decimal sepetTutari)
        {
            decimal indirim = 0;
            
            switch (kupon.IndirimTuru)
            {
                case IndirimTuru.Yuzde:
                    indirim = sepetTutari * (kupon.IndirimMiktari / 100);
                    if (kupon.MaksimumIndirim.HasValue && indirim > kupon.MaksimumIndirim.Value)
                        indirim = kupon.MaksimumIndirim.Value;
                    break;
                    
                case IndirimTuru.Tutar:
                    indirim = kupon.IndirimMiktari;
                    if (indirim > sepetTutari)
                        indirim = sepetTutari;
                    break;
                    
                case IndirimTuru.UcretsizKargo:
                    // Kargo ücreti ayrıca hesaplanacak
                    indirim = 0;
                    break;
            }
            
            return await Task.FromResult(indirim);
        }

        public async Task<KuponKullanimi> KuponKullaniminiKaydetAsync(int kuponId, int kullaniciId, int? siparisId, decimal indirimTutari, decimal sepetTutari, string? ipAdresi = null)
        {
            var kullanim = new KuponKullanimi
            {
                KuponId = kuponId,
                KullaniciId = kullaniciId,
                SiparisId = siparisId,
                IndirimTutari = indirimTutari,
                SepetTutari = sepetTutari,
                IpAdresi = ipAdresi,
                KullanimTarihi = DateTime.Now
            };
            
            _context.KuponKullanimlari.Add(kullanim);
            
            // Kupon kullanım sayısını artır
            var kupon = await _context.Kuponlar.FindAsync(kuponId);
            if (kupon != null)
            {
                kupon.KullanimSayisi++;
                _context.Kuponlar.Update(kupon);
            }
            
            await _context.SaveChangesAsync();
            return kullanim;
        }

        public async Task<int> GetKullaniciKullanimSayisiAsync(int kuponId, int kullaniciId)
        {
            return await _context.KuponKullanimlari
                .CountAsync(kk => kk.KuponId == kuponId && kk.KullaniciId == kullaniciId);
        }

        public async Task<Kupon> OlusturAsync(Kupon kupon)
        {
            kupon.Kod = kupon.Kod.ToUpper();
            kupon.OlusturmaTarihi = DateTime.Now;
            
            _context.Kuponlar.Add(kupon);
            await _context.SaveChangesAsync();
            return kupon;
        }

        public async Task<Kupon> GuncelleAsync(Kupon kupon)
        {
            kupon.Kod = kupon.Kod.ToUpper();
            kupon.GuncellenmeTarihi = DateTime.Now;
            
            _context.Kuponlar.Update(kupon);
            await _context.SaveChangesAsync();
            return kupon;
        }

        public async Task<bool> SilAsync(int id)
        {
            var kupon = await _context.Kuponlar.FindAsync(id);
            if (kupon == null) return false;
            
            _context.Kuponlar.Remove(kupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AktiflikDurumunuDegistirAsync(int id, bool aktif)
        {
            var kupon = await _context.Kuponlar.FindAsync(id);
            if (kupon == null) return false;
            
            kupon.Aktif = aktif;
            kupon.GuncellenmeTarihi = DateTime.Now;
            
            _context.Kuponlar.Update(kupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> ToplamKullanimSayisiAsync(int kuponId)
        {
            return await _context.KuponKullanimlari
                .CountAsync(kk => kk.KuponId == kuponId);
        }

        public async Task<decimal> ToplamIndirimTutariAsync(int kuponId)
        {
            return await _context.KuponKullanimlari
                .Where(kk => kk.KuponId == kuponId)
                .SumAsync(kk => kk.IndirimTutari);
        }

        public async Task<List<KuponKullanimi>> GetKullanimGecmisiAsync(int kuponId, int? limit = null)
        {
            var query = _context.KuponKullanimlari
                .Include(kk => kk.Kullanici)
                .Include(kk => kk.Siparis)
                .Where(kk => kk.KuponId == kuponId)
                .OrderByDescending(kk => kk.KullanimTarihi);
            
            if (limit.HasValue)
                return await query.Take(limit.Value).ToListAsync();
            
            return await query.ToListAsync();
        }
    }
}
