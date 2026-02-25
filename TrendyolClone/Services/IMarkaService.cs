using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public interface IMarkaService
    {
        Task<List<Marka>> GetAllAsync();
        Task<List<Marka>> GetAktifMarkalerAsync();
        Task<Marka?> GetByIdAsync(int id);
        Task<Marka?> GetBySlugAsync(string slug);
        Task<Marka> OlusturAsync(Marka marka);
        Task<Marka> GuncelleAsync(Marka marka);
        Task<bool> SilAsync(int id);
        Task<bool> AktiflikDurumunuDegistirAsync(int id, bool aktif);
        Task<string> GenerateSlugAsync(string markaAdi, int? excludeId = null);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
        Task<int> GetUrunSayisiAsync(int markaId);
        Task UpdateUrunSayisiAsync(int markaId);
        Task<List<Marka>> SearchAsync(string searchTerm);
        Task<List<Marka>> GetPopulerMarkalarAsync(int limit = 10);
    }
}
