using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public interface IUrunVaryasyonService
    {
        // Varyasyon CRUD
        Task<List<UrunVaryasyon>> GetVaryasyonlarAsync(int urunId);
        Task<UrunVaryasyon?> GetVaryasyonByIdAsync(int id);
        Task<UrunVaryasyon?> GetVaryasyonBySKUAsync(string sku);
        Task<UrunVaryasyon> AddVaryasyonAsync(UrunVaryasyon varyasyon);
        Task UpdateVaryasyonAsync(UrunVaryasyon varyasyon);
        Task DeleteVaryasyonAsync(int id);
        
        // Stok yönetimi
        Task<bool> UpdateStokAsync(int varyasyonId, int miktar);
        Task<bool> StokKontrolAsync(int varyasyonId, int miktar);
        Task<List<UrunVaryasyon>> GetDusukStokVaryasyonlarAsync();
        
        // Resim yönetimi
        Task<List<UrunResim>> GetResimlerAsync(int urunId, int? varyasyonId = null);
        Task<UrunResim> AddResimAsync(UrunResim resim);
        Task DeleteResimAsync(int resimId);
        Task SetAnaResimAsync(int resimId);
        
        // Özellik yönetimi
        Task<List<UrunOzellik>> GetOzelliklerAsync(int urunId);
        Task<UrunOzellik> AddOzellikAsync(UrunOzellik ozellik);
        Task UpdateOzellikAsync(UrunOzellik ozellik);
        Task DeleteOzellikAsync(int ozellikId);
        
        // Kargo
        Task<KargoOlculeri?> GetKargoOlculeriAsync(int urunId);
        Task<KargoOlculeri> AddOrUpdateKargoOlculeriAsync(KargoOlculeri kargo);
        Task<decimal> HesaplaKargoUcretiAsync(int urunId, string il);
    }
}
