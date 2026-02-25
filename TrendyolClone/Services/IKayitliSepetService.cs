using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public interface IKayitliSepetService
    {
        // Sepet yönetimi
        Task<KayitliSepet> KaydetAsync(KayitliSepet sepet);
        Task<KayitliSepet?> GetByIdAsync(int id);
        Task<List<KayitliSepet>> GetKullaniciSepetleriAsync(int kullaniciId);
        Task<bool> SilAsync(int id);
        Task<KayitliSepet> GuncelleAsync(KayitliSepet sepet);
        
        // Sepet yükleme
        Task<bool> SepeteYukleAsync(int kayitliSepetId, int kullaniciId);
    }
}
