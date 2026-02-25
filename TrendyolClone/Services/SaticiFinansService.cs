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
    public class SaticiFinansService : ISaticiFinansService
    {
        private readonly UygulamaDbContext _context;
        
        public SaticiFinansService(UygulamaDbContext context)
        {
            _context = context;
        }
        
        public async Task<SaticiFinansDto> GetFinansRaporuAsync(int saticiId, DateTime? baslangic = null, DateTime? bitis = null)
        {
            baslangic ??= DateTime.Now.AddMonths(-12);
            bitis ??= DateTime.Now;
            
            var siparisler = await _context.SaticiSiparisler
                .Where(s => s.SaticiId == saticiId && 
                           s.SiparisTarihi >= baslangic && 
                           s.SiparisTarihi <= bitis)
                .ToListAsync();
            
            var odemeler = await _context.SaticiOdemeler
                .Where(o => o.SaticiId == saticiId)
                .OrderByDescending(o => o.TalepTarihi)
                .ToListAsync();
            
            var rapor = new SaticiFinansDto
            {
                ToplamSatis = siparisler.Sum(s => s.ToplamTutar),
                ToplamKomisyon = siparisler.Sum(s => s.KomisyonTutari),
                NetKazanc = siparisler.Sum(s => s.SaticiKazanci),
                BekleyenOdeme = odemeler.Where(o => o.Durum == SaticiOdemeDurum.Beklemede).Sum(o => o.NetTutar),
                OdenenTutar = odemeler.Where(o => o.Durum == SaticiOdemeDurum.Odendi).Sum(o => o.NetTutar)
            };
            
            // Aylık rapor
            var aylikGrup = siparisler
                .GroupBy(s => new { s.SiparisTarihi.Year, s.SiparisTarihi.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);
            
            foreach (var grup in aylikGrup)
            {
                rapor.AylikRapor.Add(new AylikFinansDto
                {
                    Yil = grup.Key.Year,
                    Ay = grup.Key.Month,
                    Satis = grup.Sum(s => s.ToplamTutar),
                    Komisyon = grup.Sum(s => s.KomisyonTutari),
                    Net = grup.Sum(s => s.SaticiKazanci),
                    SiparisSayisi = grup.Count()
                });
            }
            
            // Son ödemeler
            rapor.SonOdemeler = odemeler.Take(10).Select(o => new SaticiOdemeDto
            {
                Id = o.Id,
                Tutar = o.Tutar,
                KomisyonTutari = o.KomisyonTutari,
                NetTutar = o.NetTutar,
                Donem = o.Donem,
                OdemeTarihi = o.OdemeTarihi,
                Durum = o.Durum,
                Aciklama = o.Aciklama
            }).ToList();
            
            return rapor;
        }
        
        public async Task<decimal> GetToplamKazancAsync(int saticiId)
        {
            return await _context.SaticiSiparisler
                .Where(s => s.SaticiId == saticiId && s.Durum == SaticiSiparisDurum.TeslimEdildi)
                .SumAsync(s => s.SaticiKazanci);
        }
        
        public async Task<decimal> GetBekleyenOdemeAsync(int saticiId)
        {
            return await _context.SaticiOdemeler
                .Where(o => o.SaticiId == saticiId && o.Durum == SaticiOdemeDurum.Beklemede)
                .SumAsync(o => o.NetTutar);
        }
        
        public async Task<List<SaticiOdemeDto>> GetOdemelerAsync(int saticiId)
        {
            return await _context.SaticiOdemeler
                .Where(o => o.SaticiId == saticiId)
                .OrderByDescending(o => o.TalepTarihi)
                .Select(o => new SaticiOdemeDto
                {
                    Id = o.Id,
                    Tutar = o.Tutar,
                    KomisyonTutari = o.KomisyonTutari,
                    NetTutar = o.NetTutar,
                    Donem = o.Donem,
                    OdemeTarihi = o.OdemeTarihi,
                    Durum = o.Durum,
                    Aciklama = o.Aciklama
                })
                .ToListAsync();
        }
        
        public async Task<bool> OdemeTalebiOlusturAsync(int saticiId, int donem)
        {
            // Dönem için ödeme talebi var mı kontrol et
            var mevcutTalep = await _context.SaticiOdemeler
                .AnyAsync(o => o.SaticiId == saticiId && o.Donem == donem);
            
            if (mevcutTalep) return false;
            
            // Dönem için siparişleri al
            var yil = donem / 100;
            var ay = donem % 100;
            var donemBaslangic = new DateTime(yil, ay, 1);
            var donemBitis = donemBaslangic.AddMonths(1).AddDays(-1);
            
            var siparisler = await _context.SaticiSiparisler
                .Where(s => s.SaticiId == saticiId &&
                           s.Durum == SaticiSiparisDurum.TeslimEdildi &&
                           s.TeslimTarihi >= donemBaslangic &&
                           s.TeslimTarihi <= donemBitis)
                .ToListAsync();
            
            if (!siparisler.Any()) return false;
            
            var tutar = siparisler.Sum(s => s.ToplamTutar);
            var komisyon = siparisler.Sum(s => s.KomisyonTutari);
            var net = siparisler.Sum(s => s.SaticiKazanci);
            
            var odeme = new SaticiOdeme
            {
                SaticiId = saticiId,
                Tutar = tutar,
                KomisyonTutari = komisyon,
                NetTutar = net,
                Donem = donem,
                Durum = SaticiOdemeDurum.Beklemede,
                TalepTarihi = DateTime.Now,
                Aciklama = $"{ay}/{yil} dönemi ödeme talebi"
            };
            
            _context.SaticiOdemeler.Add(odeme);
            await _context.SaveChangesAsync();
            
            return true;
        }
        
        public async Task<bool> OdemeOnaylaAsync(int odemeId, int yoneticiId)
        {
            var odeme = await _context.SaticiOdemeler.FindAsync(odemeId);
            if (odeme == null) return false;
            
            odeme.Durum = SaticiOdemeDurum.Onaylandi;
            odeme.OnaylayanYoneticiId = yoneticiId;
            odeme.OnayTarihi = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> OdemeYapAsync(int odemeId, string referansNo)
        {
            var odeme = await _context.SaticiOdemeler.FindAsync(odemeId);
            if (odeme == null) return false;
            
            odeme.Durum = SaticiOdemeDurum.Odendi;
            odeme.OdemeTarihi = DateTime.Now;
            odeme.ReferansNo = referansNo;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
