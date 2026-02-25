using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public class KayitliSepetService : IKayitliSepetService
    {
        private readonly UygulamaDbContext _context;

        public KayitliSepetService(UygulamaDbContext context)
        {
            _context = context;
        }

        public async Task<KayitliSepet> KaydetAsync(KayitliSepet sepet)
        {
            sepet.OlusturmaTarihi = DateTime.Now;
            
            // Ürün fiyatlarını kaydet
            foreach (var urun in sepet.Urunler)
            {
                var urunEntity = await _context.Urunler.FindAsync(urun.UrunId);
                if (urunEntity != null)
                {
                    if (urun.VaryasyonId.HasValue)
                    {
                        var varyasyon = await _context.UrunVaryasyonlari.FindAsync(urun.VaryasyonId.Value);
                        urun.BirimFiyat = varyasyon?.GecerliFiyat ?? urunEntity.IndirimliFiyat ?? urunEntity.Fiyat;
                    }
                    else
                    {
                        urun.BirimFiyat = urunEntity.IndirimliFiyat ?? urunEntity.Fiyat;
                    }
                }
                urun.EklenmeTarihi = DateTime.Now;
            }
            
            _context.KayitliSepetler.Add(sepet);
            await _context.SaveChangesAsync();
            return sepet;
        }

        public async Task<KayitliSepet?> GetByIdAsync(int id)
        {
            return await _context.KayitliSepetler
                .Include(ks => ks.Urunler)
                    .ThenInclude(ksu => ksu.Urun)
                .Include(ks => ks.Urunler)
                    .ThenInclude(ksu => ksu.Varyasyon)
                .FirstOrDefaultAsync(ks => ks.Id == id);
        }

        public async Task<List<KayitliSepet>> GetKullaniciSepetleriAsync(int kullaniciId)
        {
            return await _context.KayitliSepetler
                .Include(ks => ks.Urunler)
                    .ThenInclude(ksu => ksu.Urun)
                .Where(ks => ks.KullaniciId == kullaniciId)
                .OrderByDescending(ks => ks.OlusturmaTarihi)
                .ToListAsync();
        }

        public async Task<bool> SilAsync(int id)
        {
            var sepet = await _context.KayitliSepetler.FindAsync(id);
            if (sepet == null) return false;
            
            _context.KayitliSepetler.Remove(sepet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<KayitliSepet> GuncelleAsync(KayitliSepet sepet)
        {
            sepet.GuncellenmeTarihi = DateTime.Now;
            
            _context.KayitliSepetler.Update(sepet);
            await _context.SaveChangesAsync();
            return sepet;
        }

        public async Task<bool> SepeteYukleAsync(int kayitliSepetId, int kullaniciId)
        {
            var kayitliSepet = await GetByIdAsync(kayitliSepetId);
            if (kayitliSepet == null || kayitliSepet.KullaniciId != kullaniciId)
                return false;

            // Mevcut sepeti temizle
            var mevcutSepet = await _context.SepetUrunleri
                .Where(su => su.KullaniciId == kullaniciId)
                .ToListAsync();
            _context.SepetUrunleri.RemoveRange(mevcutSepet);

            // Kayıtlı sepetteki ürünleri sepete ekle
            foreach (var urun in kayitliSepet.Urunler)
            {
                // Stok kontrolü
                var stokVarMi = urun.VaryasyonId.HasValue
                    ? await _context.UrunVaryasyonlari
                        .AnyAsync(v => v.Id == urun.VaryasyonId.Value && v.Stok >= urun.Adet)
                    : await _context.Urunler
                        .AnyAsync(u => u.Id == urun.UrunId && u.Stok >= urun.Adet);

                if (stokVarMi)
                {
                    var sepetUrunu = new SepetUrunu
                    {
                        KullaniciId = kullaniciId,
                        UrunId = urun.UrunId,
                        Adet = urun.Adet,
                        EklenmeTarihi = DateTime.Now
                    };
                    _context.SepetUrunleri.Add(sepetUrunu);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
