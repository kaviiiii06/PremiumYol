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
    public class SaticiService : ISaticiService
    {
        private readonly UygulamaDbContext _context;
        
        public SaticiService(UygulamaDbContext context)
        {
            _context = context;
        }
        
        public async Task<Satici?> GetByIdAsync(int id)
        {
            return await _context.Saticilar
                .Include(s => s.Kullanici)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        
        public async Task<Satici?> GetByKullaniciIdAsync(int kullaniciId)
        {
            return await _context.Saticilar
                .Include(s => s.Kullanici)
                .FirstOrDefaultAsync(s => s.KullaniciId == kullaniciId);
        }
        
        public async Task<Satici?> GetBySlugAsync(string slug)
        {
            return await _context.Saticilar
                .Include(s => s.Kullanici)
                .FirstOrDefaultAsync(s => s.Slug == slug);
        }
        
        public async Task<List<SaticiDto>> GetAllAsync()
        {
            return await _context.Saticilar
                .Select(s => new SaticiDto
                {
                    Id = s.Id,
                    MagazaAdi = s.MagazaAdi,
                    Slug = s.Slug,
                    Aciklama = s.Aciklama,
                    Logo = s.Logo,
                    OrtalamaPuan = s.OrtalamaPuan,
                    DegerlendirmeSayisi = s.DegerlendirmeSayisi,
                    ToplamSatis = s.ToplamSatis,
                    Durum = s.Durum
                })
                .ToListAsync();
        }
        
        public async Task<List<SaticiDto>> GetOnaylananlarAsync()
        {
            return await _context.Saticilar
                .Where(s => s.Durum == SaticiDurum.Onaylandi && s.AktifMi)
                .Select(s => new SaticiDto
                {
                    Id = s.Id,
                    MagazaAdi = s.MagazaAdi,
                    Slug = s.Slug,
                    Aciklama = s.Aciklama,
                    Logo = s.Logo,
                    OrtalamaPuan = s.OrtalamaPuan,
                    DegerlendirmeSayisi = s.DegerlendirmeSayisi,
                    ToplamSatis = s.ToplamSatis,
                    Durum = s.Durum
                })
                .ToListAsync();
        }
        
        public async Task<Satici> CreateAsync(Satici satici)
        {
            satici.Slug = GenerateSlug(satici.MagazaAdi);
            satici.KayitTarihi = DateTime.Now;
            satici.Durum = SaticiDurum.Beklemede;
            
            _context.Saticilar.Add(satici);
            await _context.SaveChangesAsync();
            
            return satici;
        }
        
        public async Task UpdateAsync(Satici satici)
        {
            _context.Saticilar.Update(satici);
            await _context.SaveChangesAsync();
        }
        
        public async Task<bool> OnaylaAsync(int id)
        {
            var satici = await GetByIdAsync(id);
            if (satici == null) return false;
            
            satici.Durum = SaticiDurum.Onaylandi;
            satici.OnayTarihi = DateTime.Now;
            satici.AktifMi = true;
            
            await UpdateAsync(satici);
            return true;
        }
        
        public async Task<bool> ReddetAsync(int id, string sebep)
        {
            var satici = await GetByIdAsync(id);
            if (satici == null) return false;
            
            satici.Durum = SaticiDurum.Reddedildi;
            satici.AktifMi = false;
            
            await UpdateAsync(satici);
            return true;
        }
        
        public async Task<bool> AskiyaAlAsync(int id, string sebep)
        {
            var satici = await GetByIdAsync(id);
            if (satici == null) return false;
            
            satici.Durum = SaticiDurum.AskiyaAlindi;
            satici.AktifMi = false;
            
            await UpdateAsync(satici);
            return true;
        }
        
        public async Task<SaticiDashboardDto> GetDashboardAsync(int saticiId)
        {
            var bugun = DateTime.Today;
            var haftaBasi = bugun.AddDays(-(int)bugun.DayOfWeek);
            var ayBasi = new DateTime(bugun.Year, bugun.Month, 1);
            
            var siparisler = await _context.SaticiSiparisler
                .Where(s => s.SaticiId == saticiId)
                .ToListAsync();
            
            var dashboard = new SaticiDashboardDto
            {
                BugunSatis = siparisler.Where(s => s.SiparisTarihi.Date == bugun).Sum(s => s.SaticiKazanci),
                BuHaftaSatis = siparisler.Where(s => s.SiparisTarihi >= haftaBasi).Sum(s => s.SaticiKazanci),
                BuAySatis = siparisler.Where(s => s.SiparisTarihi >= ayBasi).Sum(s => s.SaticiKazanci),
                BekleyenSiparis = siparisler.Count(s => s.Durum == SaticiSiparisDurum.YeniSiparis),
                HazirlaniyorSiparis = siparisler.Count(s => s.Durum == SaticiSiparisDurum.Hazirlaniyor),
                KargodaSiparis = siparisler.Count(s => s.Durum == SaticiSiparisDurum.KargoyaVerildi),
                ToplamKazanc = siparisler.Where(s => s.Durum == SaticiSiparisDurum.TeslimEdildi).Sum(s => s.SaticiKazanci)
            };
            
            // Düşük stok uyarıları
            dashboard.DusukStokUrun = await _context.SaticiUrunler
                .Where(s => s.SaticiId == saticiId && s.Stok > 0 && s.Stok <= 10)
                .CountAsync();
            
            dashboard.TukenStokUrun = await _context.SaticiUrunler
                .Where(s => s.SaticiId == saticiId && s.Stok == 0)
                .CountAsync();
            
            // Bekleyen ödeme
            dashboard.BekleyenOdeme = await _context.SaticiOdemeler
                .Where(o => o.SaticiId == saticiId && o.Durum == SaticiOdemeDurum.Beklemede)
                .SumAsync(o => o.NetTutar);
            
            // Son 7 gün satışları
            for (int i = 6; i >= 0; i--)
            {
                var tarih = bugun.AddDays(-i);
                var gunlukSatis = siparisler
                    .Where(s => s.SiparisTarihi.Date == tarih)
                    .Sum(s => s.SaticiKazanci);
                
                dashboard.SonYediGun.Add(new GunlukSatisDto
                {
                    Tarih = tarih,
                    Tutar = gunlukSatis,
                    SiparisSayisi = siparisler.Count(s => s.SiparisTarihi.Date == tarih)
                });
            }
            
            // Popüler ürünler
            dashboard.PopulerUrunler = await _context.SaticiUrunler
                .Where(s => s.SaticiId == saticiId)
                .OrderByDescending(s => s.SatisSayisi)
                .Take(5)
                .Select(s => new PopulerUrunDto
                {
                    UrunId = s.UrunId,
                    UrunAdi = s.Urun!.Ad,
                    Resim = s.Urun.ResimUrl,
                    SatisSayisi = s.SatisSayisi,
                    Tutar = s.ToplamSatisTutari
                })
                .ToListAsync();
            
            // Son siparişler
            dashboard.SonSiparisler = await _context.SaticiSiparisler
                .Where(s => s.SaticiId == saticiId)
                .OrderByDescending(s => s.SiparisTarihi)
                .Take(10)
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
            
            return dashboard;
        }
        
        public async Task<bool> PuanGuncelleAsync(int saticiId)
        {
            var satici = await GetByIdAsync(saticiId);
            if (satici == null) return false;
            
            var degerlendirmeler = await _context.SaticiDegerlendirmeler
                .Where(d => d.SaticiId == saticiId && d.OnaylandiMi)
                .ToListAsync();
            
            if (degerlendirmeler.Any())
            {
                satici.OrtalamaPuan = (decimal)degerlendirmeler.Average(d => d.Puan);
                satici.DegerlendirmeSayisi = degerlendirmeler.Count;
            }
            
            await UpdateAsync(satici);
            return true;
        }
        
        private string GenerateSlug(string magazaAdi)
        {
            var slug = magazaAdi.ToLowerInvariant()
                .Replace("ı", "i")
                .Replace("ğ", "g")
                .Replace("ü", "u")
                .Replace("ş", "s")
                .Replace("ö", "o")
                .Replace("ç", "c")
                .Replace(" ", "-");
            
            return new string(slug.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());
        }
    }
}
