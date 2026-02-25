using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface IKargoService
    {
        // Kargo ücreti hesaplama
        Task<List<KargoSecenegi>> HesaplaKargoUcretleriAsync(string il, decimal agirlik, decimal desi, decimal sepetTutari);
        Task<KargoSecenegi?> GetEnUygunKargoAsync(string il, decimal agirlik, decimal desi, decimal sepetTutari);
        Task<decimal> HesaplaKargoUcretiAsync(int kargoFirmaId, decimal agirlik, decimal desi, decimal sepetTutari);
        
        // Kargo firma yönetimi
        Task<List<KargoFirma>> GetAktifKargoFirmalariAsync();
        Task<KargoFirma?> GetKargoFirmaByIdAsync(int id);
        Task<KargoFirma> OlusturAsync(KargoFirma kargoFirma);
        Task<KargoFirma> GuncelleAsync(KargoFirma kargoFirma);
        Task<bool> SilAsync(int id);
        
        // Kargo ücret yönetimi
        Task<KargoUcret?> GetKargoUcretAsync(int kargoFirmaId, string cikisIli, string varisIli);
        Task<KargoUcret> KargoUcretOlusturAsync(KargoUcret kargoUcret);
        Task<KargoUcret> KargoUcretGuncelleAsync(KargoUcret kargoUcret);
        Task<List<KargoUcret>> GetKargoFirmaUcretleriAsync(int kargoFirmaId);
        
        // Ücretsiz kargo kontrolü
        Task<bool> UcretsizKargoVarMi(int kargoFirmaId, decimal sepetTutari);
    }
}
