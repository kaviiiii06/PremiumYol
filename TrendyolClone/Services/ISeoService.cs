using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface ISeoService
    {
        Task<SeoMetaDto> GetSeoMetaAsync(string sayfaTipi, int? referansId = null);
        Task<bool> UpdateSeoMetaAsync(string sayfaTipi, int? referansId, SeoMetaDto seoMeta);
        Task<SeoMetaDto> GenerateProductSeoAsync(int urunId);
        Task<SeoMetaDto> GenerateCategorySeoAsync(int kategoriId);
        string GenerateSeoFriendlyUrl(string text);
    }
}
