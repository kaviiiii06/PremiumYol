using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public interface IDropshippingService
    {
        Task<List<Urun>> SearchProductsAsync(Tedarikci tedarikci, string searchTerm, int page = 1, int pageSize = 20);
        Task<Urun> GetProductDetailsAsync(Tedarikci tedarikci, string productId);
        Task<bool> UpdateStockAsync(Tedarikci tedarikci, string productId);
        Task<decimal> GetCurrentPriceAsync(Tedarikci tedarikci, string productId);
    }
}