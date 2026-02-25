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
    public class SaticiSiparisService : ISaticiSiparisService
    {
        private readonly UygulamaDbContext _context;
        
        public SaticiSiparisService(UygulamaDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<SaticiSiparisDto>> GetBySaticiAsync(int saticiId, SaticiSiparisDurum? durum = null)
        {
            var query = _context.SaticiSiparisler
                .Where(s => s.SaticiId == saticiId);
            
            if (durum.HasValue)
            {
                query = query.Where(s => s.Durum == durum.Value);
            }
            
            return await query
                .Include(s => s.Siparis).ThenInclude(s => s!.Kullanici)
                .Include(s => s.Urun)
                .OrderByDescending(s => s.SiparisTarihi)
                .Select(s => new SaticiSiparisDto
                {
                    Id = s.Id,
                    SiparisNo = s.Siparis!.SiparisNo,
                    UrunAdi = s.Urun!.Ad,
                    UrunResmi = s.Urun.ResimUrl,
                    Adet = s.Adet,
                    ToplamTutar = s.ToplamTutar,
                    SaticiKazanci = s.SaticiKazanci,
                    Durum = s.Durum,
                    SiparisTarihi = s.SiparisTarihi,
                    MusteriAdi = s.Siparis.Kullanici!.Ad + " " + s.Siparis.Kullanici.Soyad,
                    KargoTakipNo = s.KargoTakipNo
                })
                .ToListAsync();
        }
        
        public async Task<SaticiSiparis?> GetByIdAsync(int id)
        {
            return await _context.SaticiSiparisler
                .Include(s => s.Siparis).ThenInclude(s => s!.Kullanici)
                .Include(s => s.Urun)
                .Include(s => s.Satici)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        
        public async Task<List<SaticiSiparisDto>> GetBekleyenlerAsync(int saticiId)
        {
            return await GetBySaticiAsync(saticiId, SaticiSiparisDurum.YeniSiparis);
        }
        
        public async Task<bool> KargoHazirlaAsync(int id, string kargoTakipNo)
        {
            var siparis = await GetByIdAsync(id);
            if (siparis == null) return false;
            
            siparis.Durum = SaticiSiparisDurum.KargoyaVerildi;
            siparis.KargoTakipNo = kargoTakipNo;
            siparis.KargoTarihi = DateTime.Now;
            
            _context.SaticiSiparisler.Update(siparis);
            await _context.SaveChangesAsync();
            
            return true;
        }
        
        public async Task<bool> TeslimEdildiAsync(int id)
        {
            var siparis = await GetByIdAsync(id);
            if (siparis == null) return false;
            
            siparis.Durum = SaticiSiparisDurum.TeslimEdildi;
            siparis.TeslimTarihi = DateTime.Now;
            
            _context.SaticiSiparisler.Update(siparis);
            await _context.SaveChangesAsync();
            
            return true;
        }
        
        public async Task<bool> IptalEtAsync(int id, string sebep)
        {
            var siparis = await GetByIdAsync(id);
            if (siparis == null) return false;
            
            siparis.Durum = SaticiSiparisDurum.IptalEdildi;
            siparis.Notlar = sebep;
            
            _context.SaticiSiparisler.Update(siparis);
            await _context.SaveChangesAsync();
            
            return true;
        }
        
        public async Task<SaticiSiparis> CreateAsync(int saticiId, int siparisId, int urunId, int adet, decimal birimFiyat, decimal komisyonOrani)
        {
            var toplamTutar = adet * birimFiyat;
            var komisyonTutari = toplamTutar * (komisyonOrani / 100);
            var saticiKazanci = toplamTutar - komisyonTutari;
            
            var saticiSiparis = new SaticiSiparis
            {
                SaticiId = saticiId,
                SiparisId = siparisId,
                UrunId = urunId,
                Adet = adet,
                BirimFiyat = birimFiyat,
                ToplamTutar = toplamTutar,
                KomisyonOrani = komisyonOrani,
                KomisyonTutari = komisyonTutari,
                SaticiKazanci = saticiKazanci,
                Durum = SaticiSiparisDurum.YeniSiparis,
                SiparisTarihi = DateTime.Now
            };
            
            _context.SaticiSiparisler.Add(saticiSiparis);
            await _context.SaveChangesAsync();
            
            return saticiSiparis;
        }
    }
}
