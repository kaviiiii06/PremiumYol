using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public class SaticiUrunService : ISaticiUrunService
    {
        private readonly UygulamaDbContext _context;
        
        public SaticiUrunService(UygulamaDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<SaticiUrunDto>> GetBySaticiAsync(int saticiId, SaticiUrunDurum? durum = null)
        {
            var query = _context.SaticiUrunler
                .Where(s => s.SaticiId == saticiId);
            
            if (durum.HasValue)
            {
                query = query.Where(s => s.Durum == durum.Value);
            }
            
            return await query
                .Include(s => s.Urun)
                .Select(s => new SaticiUrunDto
                {
                    Id = s.Id,
                    UrunId = s.UrunId,
                    UrunAdi = s.Urun!.Ad,
                    Resim = s.Urun.ResimUrl,
                    Stok = s.Stok,
                    Fiyat = s.Fiyat,
                    IndirimliFiyat = s.IndirimliFiyat,
                    Durum = s.Durum,
                    GoruntulemeSayisi = s.GoruntulemeSayisi,
                    SatisSayisi = s.SatisSayisi,
                    EklenmeTarihi = s.EklenmeTarihi
                })
                .OrderByDescending(s => s.EklenmeTarihi)
                .ToListAsync();
        }
        
        public async Task<SaticiUrun?> GetByIdAsync(int id)
        {
            return await _context.SaticiUrunler
                .Include(s => s.Urun)
                .Include(s => s.Satici)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        
        public async Task<SaticiUrun> CreateAsync(SaticiUrun urun)
        {
            urun.EklenmeTarihi = DateTime.Now;
            urun.Durum = SaticiUrunDurum.Taslak;
            
            _context.SaticiUrunler.Add(urun);
            await _context.SaveChangesAsync();
            
            return urun;
        }
        
        public async Task UpdateAsync(SaticiUrun urun)
        {
            urun.GuncellenmeTarihi = DateTime.Now;
            _context.SaticiUrunler.Update(urun);
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(int id)
        {
            var urun = await GetByIdAsync(id);
            if (urun != null)
            {
                _context.SaticiUrunler.Remove(urun);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<bool> StokGuncelleAsync(int id, int stok)
        {
            var urun = await GetByIdAsync(id);
            if (urun == null) return false;
            
            urun.Stok = stok;
            if (stok == 0)
            {
                urun.Durum = SaticiUrunDurum.StokTukendi;
            }
            else if (urun.Durum == SaticiUrunDurum.StokTukendi)
            {
                urun.Durum = SaticiUrunDurum.Yayinda;
            }
            
            await UpdateAsync(urun);
            return true;
        }
        
        public async Task<bool> FiyatGuncelleAsync(int id, decimal fiyat, decimal? indirimliFiyat)
        {
            var urun = await GetByIdAsync(id);
            if (urun == null) return false;
            
            urun.Fiyat = fiyat;
            urun.IndirimliFiyat = indirimliFiyat;
            
            await UpdateAsync(urun);
            return true;
        }
        
        public async Task<bool> DurumGuncelleAsync(int id, SaticiUrunDurum durum)
        {
            var urun = await GetByIdAsync(id);
            if (urun == null) return false;
            
            urun.Durum = durum;
            if (durum == SaticiUrunDurum.Yayinda)
            {
                urun.OnayTarihi = DateTime.Now;
            }
            
            await UpdateAsync(urun);
            return true;
        }
        
        public async Task<bool> OnayaGonderAsync(int id)
        {
            return await DurumGuncelleAsync(id, SaticiUrunDurum.OnayBekliyor);
        }
        
        public async Task<bool> YayindanKaldirAsync(int id)
        {
            return await DurumGuncelleAsync(id, SaticiUrunDurum.Pasif);
        }
        
        public async Task<bool> GoruntulemeSayisiArtirAsync(int id)
        {
            var urun = await GetByIdAsync(id);
            if (urun == null) return false;
            
            urun.GoruntulemeSayisi++;
            await UpdateAsync(urun);
            return true;
        }
        
        public async Task<bool> SatisKaydetAsync(int id, int adet, decimal tutar)
        {
            var urun = await GetByIdAsync(id);
            if (urun == null) return false;
            
            urun.SatisSayisi += adet;
            urun.ToplamSatisTutari += tutar;
            urun.Stok -= adet;
            
            if (urun.Stok == 0)
            {
                urun.Durum = SaticiUrunDurum.StokTukendi;
            }
            
            await UpdateAsync(urun);
            return true;
        }
    }
}
