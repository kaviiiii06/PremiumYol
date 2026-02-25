using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public class AnalyticsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(IConfiguration configuration, ILogger<AnalyticsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GetGoogleAnalyticsId()
        {
            return _configuration["Analytics:GoogleAnalyticsId"] ?? "";
        }

        public string GetGoogleTagManagerId()
        {
            return _configuration["Analytics:GoogleTagManagerId"] ?? "";
        }

        public string GetFacebookPixelId()
        {
            return _configuration["Analytics:FacebookPixelId"] ?? "";
        }

        public string GenerateProductViewEvent(int urunId, string urunAdi, decimal fiyat, string kategori)
        {
            var eventData = new
            {
                event_name = "view_item",
                items = new[]
                {
                    new
                    {
                        item_id = urunId.ToString(),
                        item_name = urunAdi,
                        price = fiyat,
                        item_category = kategori
                    }
                }
            };

            return System.Text.Json.JsonSerializer.Serialize(eventData);
        }

        public string GenerateAddToCartEvent(int urunId, string urunAdi, decimal fiyat, int adet)
        {
            var eventData = new
            {
                event_name = "add_to_cart",
                items = new[]
                {
                    new
                    {
                        item_id = urunId.ToString(),
                        item_name = urunAdi,
                        price = fiyat,
                        quantity = adet
                    }
                }
            };

            return System.Text.Json.JsonSerializer.Serialize(eventData);
        }

        public string GeneratePurchaseEvent(string siparisNo, decimal toplam, List<dynamic> urunler)
        {
            var eventData = new
            {
                event_name = "purchase",
                transaction_id = siparisNo,
                value = toplam,
                currency = "TRY",
                items = urunler
            };

            return System.Text.Json.JsonSerializer.Serialize(eventData);
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters)
        {
            _logger.LogInformation($"Analytics Event: {eventName}, Parameters: {System.Text.Json.JsonSerializer.Serialize(parameters)}");
        }
    }
}
