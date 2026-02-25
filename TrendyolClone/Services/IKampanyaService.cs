using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public interface IKampanyaService
    {
        Task<List<Kampanya>> GetAllAsync();
        Task<List<Kampanya>> GetAktifKampanyalarAsync();
        Task<Kampanya?> GetByIdAsync(int id);
        Task<Kampanya?> GetBySlugAsync(string slug);
        Task<List<Kampanya>> GetUrunKampanyalariAsync(int urunId);
        Task<Kampanya?> GetEnIyiKampanyaAsync(int urunId);
        Task<decimal> HesaplaIndirimAsync(int urunId, decimal urunFiyati);
        Task<Kampanya> OlusturAsync(Kampanya kampanya);
        Task<Kampanya> GuncelleAsync(Kampanya kampanya);
        Task<bool> SilAsync(int id);
        Task<bool> AktiflikDurumunuDegistirAsync(int id, bool aktif);
        Task<bool> UrunKampanyayaEkleAsync(int kampanyaId, int urunId, decimal? ozelIndirim = null, int? stokLimiti = null);
        Task<bool> UrunKampanyandanCikarAsync(int kampanyaId, int urunId);
        Task<List<Urun>> GetKampanyaUrunleriAsync(int kampanyaId);
        Task<string> GenerateSlugAsync(string kampanyaAdi, int? excludeId = null);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
        Task UpdateKampanyaIstatistikleriAsync(int kampanyaId, decimal indirimTutari);
        Task<List<Kampanya>> GetPopulerKampanyalarAsync(int limit = 5);
        Task<bool> KampanyaGecerliMi(int kampanyaId);
        Task<string?> KampanyaHataMesaji(int kampanyaId, decimal sepetTutari, int urunAdedi);
    }
}
