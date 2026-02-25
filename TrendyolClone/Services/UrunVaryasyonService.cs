using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public class UrunVaryasyonService : IUrunVaryasyonService
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<UrunVaryasyonService> _logger;

        public UrunVaryasyonService(UygulamaDbContext context, ILogger<UrunVaryasyonService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Varyasyon CRUD
        public async Task<List<UrunVaryasyon>> GetVaryasyonlarAsync(int urunId)
        {
            return await _context.UrunVaryasyonlari
                .Where(v => v.UrunId == urunId && v.Aktif)
                .OrderBy(v => v.Sira)
                .ToListAsync();
        }

        public async Task<UrunVaryasyon?> GetVaryasyonByIdAsync(int id)
        {
            return await _context.UrunVaryasyonlari
                .Include(v => v.Resimler)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<UrunVaryasyon?> GetVaryasyonBySKUAsync(string sku)
        {
            return await _context.UrunVaryasyonlari
                .FirstOrDefaultAsync(v => v.SKU == sku);
        }

        public async Task<UrunVaryasyon> AddVaryasyonAsync(UrunVaryasyon varyasyon)
        {
            varyasyon.OlusturmaTarihi = DateTime.Now;
            _context.UrunVaryasyonlari.Add(varyasyon);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Yeni varyasyon eklendi: {varyasyon.SKU}");
            return varyasyon;
        }

        public async Task UpdateVaryasyonAsync(UrunVaryasyon varyasyon)
        {
            _context.UrunVaryasyonlari.Update(varyasyon);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Varyasyon güncellendi: {varyasyon.SKU}");
        }

        public async Task DeleteVaryasyonAsync(int id)
        {
            var varyasyon = await GetVaryasyonByIdAsync(id);
            if (varyasyon != null)
            {
                varyasyon.Aktif = false; // Soft delete
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Varyasyon silindi: {varyasyon.SKU}");
            }
        }

        // Stok yönetimi
        public async Task<bool> UpdateStokAsync(int varyasyonId, int miktar)
        {
            var varyasyon = await GetVaryasyonByIdAsync(varyasyonId);
            if (varyasyon == null) return false;

            varyasyon.Stok += miktar;
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Stok güncellendi: {varyasyon.SKU}, Yeni stok: {varyasyon.Stok}");
            return true;
        }

        public async Task<bool> StokKontrolAsync(int varyasyonId, int miktar)
        {
            var varyasyon = await GetVaryasyonByIdAsync(varyasyonId);
            return varyasyon != null && varyasyon.Stok >= miktar;
        }

        public async Task<List<UrunVaryasyon>> GetDusukStokVaryasyonlarAsync()
        {
            return await _context.UrunVaryasyonlari
                .Where(v => v.Aktif && v.Stok <= v.MinStok)
                .Include(v => v.Urun)
                .ToListAsync();
        }

        // Resim yönetimi
        public async Task<List<UrunResim>> GetResimlerAsync(int urunId, int? varyasyonId = null)
        {
            var query = _context.UrunResimleri.Where(r => r.UrunId == urunId);
            
            if (varyasyonId.HasValue)
                query = query.Where(r => r.VaryasyonId == varyasyonId);
            
            return await query.OrderBy(r => r.Sira).ToListAsync();
        }

        public async Task<UrunResim> AddResimAsync(UrunResim resim)
        {
            resim.YuklemeTarihi = DateTime.Now;
            _context.UrunResimleri.Add(resim);
            await _context.SaveChangesAsync();
            
            return resim;
        }

        public async Task DeleteResimAsync(int resimId)
        {
            var resim = await _context.UrunResimleri.FindAsync(resimId);
            if (resim != null)
            {
                _context.UrunResimleri.Remove(resim);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetAnaResimAsync(int resimId)
        {
            var resim = await _context.UrunResimleri.FindAsync(resimId);
            if (resim == null) return;

            // Diğer resimleri ana resim olmaktan çıkar
            var digerResimler = await _context.UrunResimleri
                .Where(r => r.UrunId == resim.UrunId && r.Id != resimId)
                .ToListAsync();
            
            foreach (var r in digerResimler)
                r.AnaResim = false;

            resim.AnaResim = true;
            await _context.SaveChangesAsync();
        }

        // Özellik yönetimi
        public async Task<List<UrunOzellik>> GetOzelliklerAsync(int urunId)
        {
            return await _context.UrunOzellikleri
                .Where(o => o.UrunId == urunId)
                .OrderBy(o => o.Sira)
                .ToListAsync();
        }

        public async Task<UrunOzellik> AddOzellikAsync(UrunOzellik ozellik)
        {
            ozellik.OlusturmaTarihi = DateTime.Now;
            _context.UrunOzellikleri.Add(ozellik);
            await _context.SaveChangesAsync();
            
            return ozellik;
        }

        public async Task UpdateOzellikAsync(UrunOzellik ozellik)
        {
            _context.UrunOzellikleri.Update(ozellik);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOzellikAsync(int ozellikId)
        {
            var ozellik = await _context.UrunOzellikleri.FindAsync(ozellikId);
            if (ozellik != null)
            {
                _context.UrunOzellikleri.Remove(ozellik);
                await _context.SaveChangesAsync();
            }
        }

        // Kargo
        public async Task<KargoOlculeri?> GetKargoOlculeriAsync(int urunId)
        {
            return await _context.KargoOlculeri
                .FirstOrDefaultAsync(k => k.UrunId == urunId);
        }

        public async Task<KargoOlculeri> AddOrUpdateKargoOlculeriAsync(KargoOlculeri kargo)
        {
            var mevcut = await GetKargoOlculeriAsync(kargo.UrunId);
            
            if (mevcut == null)
            {
                kargo.OlusturmaTarihi = DateTime.Now;
                _context.KargoOlculeri.Add(kargo);
            }
            else
            {
                mevcut.En = kargo.En;
                mevcut.Boy = kargo.Boy;
                mevcut.Yukseklik = kargo.Yukseklik;
                mevcut.Agirlik = kargo.Agirlik;
                mevcut.UcretsizKargo = kargo.UcretsizKargo;
                mevcut.KargoUcreti = kargo.KargoUcreti;
                mevcut.GuncellenmeTarihi = DateTime.Now;
            }
            
            await _context.SaveChangesAsync();
            return kargo;
        }

        public async Task<decimal> HesaplaKargoUcretiAsync(int urunId, string il)
        {
            var kargo = await GetKargoOlculeriAsync(urunId);
            if (kargo == null) return 0;

            if (kargo.UcretsizKargo) return 0;
            if (kargo.KargoUcreti.HasValue) return kargo.KargoUcreti.Value;

            // Basit hesaplama (gerçekte kargo API'si kullanılacak)
            decimal temelUcret = 15.00m;
            decimal desiUcreti = kargo.Desi * 0.5m;
            decimal agirlikUcreti = kargo.Agirlik * 2.0m;

            return temelUcret + desiUcreti + agirlikUcreti;
        }
    }
}
