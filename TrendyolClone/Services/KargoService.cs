using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public class KargoService : IKargoService
    {
        private readonly UygulamaDbContext _context;

        public KargoService(UygulamaDbContext context)
        {
            _context = context;
        }

        public async Task<List<KargoSecenegi>> HesaplaKargoUcretleriAsync(string il, decimal agirlik, decimal desi, decimal sepetTutari)
        {
            var kargoFirmalari = await GetAktifKargoFirmalariAsync();
            var secenekler = new List<KargoSecenegi>();

            foreach (var firma in kargoFirmalari)
            {
                var ucretsizKargo = await UcretsizKargoVarMi(firma.Id, sepetTutari);
                var ucret = ucretsizKargo ? 0 : await HesaplaKargoUcretiAsync(firma.Id, agirlik, desi, sepetTutari);
                
                // İl bazlı özel ücret kontrolü
                var ilUcreti = await GetKargoUcretAsync(firma.Id, "İstanbul", il); // Varsayılan çıkış ili İstanbul
                
                secenekler.Add(new KargoSecenegi
                {
                    Id = firma.Id,
                    FirmaAdi = firma.Ad,
                    Logo = firma.Logo,
                    Ucret = ilUcreti?.Ucret ?? ucret,
                    TahminiGun = ilUcreti?.TahminiGun ?? 3,
                    UcretsizKargo = ucretsizKargo,
                    Aciklama = ucretsizKargo ? "Ücretsiz Kargo" : $"{(ilUcreti?.TahminiGun ?? 3)} iş günü"
                });
            }

            return secenekler.OrderBy(s => s.Ucret).ToList();
        }

        public async Task<KargoSecenegi?> GetEnUygunKargoAsync(string il, decimal agirlik, decimal desi, decimal sepetTutari)
        {
            var secenekler = await HesaplaKargoUcretleriAsync(il, agirlik, desi, sepetTutari);
            return secenekler.FirstOrDefault();
        }

        public async Task<decimal> HesaplaKargoUcretiAsync(int kargoFirmaId, decimal agirlik, decimal desi, decimal sepetTutari)
        {
            var firma = await GetKargoFirmaByIdAsync(kargoFirmaId);
            if (firma == null) return 0;

            // Ücretsiz kargo kontrolü
            if (await UcretsizKargoVarMi(kargoFirmaId, sepetTutari))
                return 0;

            // Temel ücret + (Ağırlık * Kg başına ücret) + (Desi * Desi başına ücret)
            var ucret = firma.TemelUcret + 
                       (agirlik * firma.KgBasinaUcret) + 
                       (desi * firma.DesiBasinaUcret);

            return Math.Round(ucret, 2);
        }

        public async Task<List<KargoFirma>> GetAktifKargoFirmalariAsync()
        {
            return await _context.KargoFirmalari
                .Where(kf => kf.Aktif)
                .OrderBy(kf => kf.Sira)
                .ToListAsync();
        }

        public async Task<KargoFirma?> GetKargoFirmaByIdAsync(int id)
        {
            return await _context.KargoFirmalari
                .Include(kf => kf.Ucretler)
                .FirstOrDefaultAsync(kf => kf.Id == id);
        }

        public async Task<KargoFirma> OlusturAsync(KargoFirma kargoFirma)
        {
            kargoFirma.OlusturmaTarihi = DateTime.Now;
            
            _context.KargoFirmalari.Add(kargoFirma);
            await _context.SaveChangesAsync();
            return kargoFirma;
        }

        public async Task<KargoFirma> GuncelleAsync(KargoFirma kargoFirma)
        {
            kargoFirma.GuncellenmeTarihi = DateTime.Now;
            
            _context.KargoFirmalari.Update(kargoFirma);
            await _context.SaveChangesAsync();
            return kargoFirma;
        }

        public async Task<bool> SilAsync(int id)
        {
            var firma = await _context.KargoFirmalari.FindAsync(id);
            if (firma == null) return false;
            
            _context.KargoFirmalari.Remove(firma);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<KargoUcret?> GetKargoUcretAsync(int kargoFirmaId, string cikisIli, string varisIli)
        {
            return await _context.KargoUcretleri
                .FirstOrDefaultAsync(ku => 
                    ku.KargoFirmaId == kargoFirmaId && 
                    ku.CikisIli == cikisIli && 
                    ku.VarisIli == varisIli);
        }

        public async Task<KargoUcret> KargoUcretOlusturAsync(KargoUcret kargoUcret)
        {
            kargoUcret.GuncellenmeTarihi = DateTime.Now;
            
            _context.KargoUcretleri.Add(kargoUcret);
            await _context.SaveChangesAsync();
            return kargoUcret;
        }

        public async Task<KargoUcret> KargoUcretGuncelleAsync(KargoUcret kargoUcret)
        {
            kargoUcret.GuncellenmeTarihi = DateTime.Now;
            
            _context.KargoUcretleri.Update(kargoUcret);
            await _context.SaveChangesAsync();
            return kargoUcret;
        }

        public async Task<List<KargoUcret>> GetKargoFirmaUcretleriAsync(int kargoFirmaId)
        {
            return await _context.KargoUcretleri
                .Where(ku => ku.KargoFirmaId == kargoFirmaId)
                .OrderBy(ku => ku.VarisIli)
                .ToListAsync();
        }

        public async Task<bool> UcretsizKargoVarMi(int kargoFirmaId, decimal sepetTutari)
        {
            var firma = await GetKargoFirmaByIdAsync(kargoFirmaId);
            if (firma == null) return false;

            return firma.UcretsizKargoLimiti.HasValue && 
                   sepetTutari >= firma.UcretsizKargoLimiti.Value;
        }
    }
}
