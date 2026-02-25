using TrendyolClone.Data;
using TrendyolClone.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace TrendyolClone.Services
{
    public class OrderService
    {
        private readonly UygulamaDbContext _context;
        private readonly ApiService _apiService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            UygulamaDbContext context, 
            ApiService apiService,
            ILogger<OrderService> logger)
        {
            _context = context;
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// Sipariş verildiğinde tedarikçiye otomatik sipariş gönderir
        /// </summary>
        public async Task<string> CreateSupplierOrderAsync(Siparis siparis)
        {
            try
            {
                _logger.LogInformation($"Creating supplier order for Order ID: {siparis.Id}");

                var siparisKalemleri = await _context.SiparisKalemleri
                    .Include(sk => sk.Urun)
                    .ThenInclude(u => u.Tedarikci)
                    .Where(sk => sk.SiparisId == siparis.Id)
                    .ToListAsync();

                var supplierOrders = new List<SupplierOrderResult>();

                // Tedarikçi bazında grupla
                var supplierGroups = siparisKalemleri.GroupBy(sk => sk.Urun.TedarikciId);

                foreach (var supplierGroup in supplierGroups)
                {
                    var tedarikci = supplierGroup.First().Urun.Tedarikci;
                    if (tedarikci == null) continue;

                    var result = await CreateOrderForSupplier(tedarikci, supplierGroup.ToList(), siparis);
                    supplierOrders.Add(result);
                }

                // Sonuçları JSON olarak döndür
                return JsonSerializer.Serialize(supplierOrders, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating supplier order for Order ID: {siparis.Id}");
                throw;
            }
        }

        private async Task<SupplierOrderResult> CreateOrderForSupplier(
            Tedarikci tedarikci, 
            List<SiparisKalemi> kalemler, 
            Siparis siparis)
        {
            try
            {
                switch (tedarikci.Tip.ToLower())
                {
                    case "cj dropshipping":
                        return await CreateCJOrder(tedarikci, kalemler, siparis);
                    case "aliexpress":
                        return await CreateAliExpressOrder(tedarikci, kalemler, siparis);
                    default:
                        return await CreateGenericOrder(tedarikci, kalemler, siparis);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating order for supplier {tedarikci.Ad}");
                return new SupplierOrderResult
                {
                    SupplierName = tedarikci.Ad,
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<SupplierOrderResult> CreateCJOrder(
            Tedarikci tedarikci, 
            List<SiparisKalemi> kalemler, 
            Siparis siparis)
        {
            // CJ Dropshipping sipariş oluşturma
            var orderData = new
            {
                orderNumber = siparis.Id.ToString(),
                shippingAddress = new
                {
                    name = $"{siparis.Kullanici.Ad} {siparis.Kullanici.Soyad}",
                    phone = siparis.Kullanici.TelefonNumarasi ?? "05551234567",
                    address = "Türkiye", // Gerçek adres bilgisi eklenecek
                    zipCode = "06000",
                    country = "TR"
                },
                products = kalemler.Select(kalem => new
                {
                    productId = kalem.Urun.TedarikciUrunId,
                    quantity = kalem.Miktar,
                    unitPrice = kalem.BirimFiyat
                }).ToArray()
            };

            var headers = new Dictionary<string, string>
            {
                ["CJ-Access-Token"] = await GetCJAccessToken(tedarikci)
            };

            var url = $"{tedarikci.ApiUrl}/shopping/order/createOrder";
            var response = await _apiService.PostAsync(url, orderData, headers);

            return new SupplierOrderResult
            {
                SupplierName = tedarikci.Ad,
                Success = true,
                OrderId = ExtractOrderIdFromResponse(response),
                TrackingNumber = ExtractTrackingFromResponse(response),
                Response = response
            };
        }

        private async Task<SupplierOrderResult> CreateAliExpressOrder(
            Tedarikci tedarikci, 
            List<SiparisKalemi> kalemler, 
            Siparis siparis)
        {
            // AliExpress sipariş oluşturma
            var orderData = new
            {
                logistics_address = new
                {
                    contact_person = $"{siparis.Kullanici.Ad} {siparis.Kullanici.Soyad}",
                    phone = siparis.Kullanici.TelefonNumarasi ?? "05551234567",
                    address = "Türkiye",
                    zip = "06000",
                    country = "TR"
                },
                product_items = kalemler.Select(kalem => new
                {
                    product_id = kalem.Urun.TedarikciUrunId,
                    quantity = kalem.Miktar,
                    unit_price = kalem.BirimFiyat
                }).ToArray()
            };

            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {tedarikci.ApiAnahtari}"
            };

            var url = $"{tedarikci.ApiUrl}/orders/create";
            var response = await _apiService.PostAsync(url, orderData, headers);

            return new SupplierOrderResult
            {
                SupplierName = tedarikci.Ad,
                Success = true,
                OrderId = ExtractOrderIdFromResponse(response),
                Response = response
            };
        }

        private async Task<SupplierOrderResult> CreateGenericOrder(
            Tedarikci tedarikci, 
            List<SiparisKalemi> kalemler, 
            Siparis siparis)
        {
            // Genel tedarikçi için demo sipariş
            await Task.Delay(1000); // Simüle edilmiş API çağrısı

            return new SupplierOrderResult
            {
                SupplierName = tedarikci.Ad,
                Success = true,
                OrderId = $"DEMO-{DateTime.Now:yyyyMMddHHmmss}",
                TrackingNumber = $"TRK{new Random().Next(100000, 999999)}",
                Response = JsonSerializer.Serialize(new
                {
                    status = "success",
                    message = "Demo sipariş oluşturuldu",
                    estimated_delivery = DateTime.Now.AddDays(tedarikci.TeslimatGunu).ToString("yyyy-MM-dd")
                })
            };
        }

        private Task<string> GetCJAccessToken(Tedarikci tedarikci)
        {
            return Task.FromResult("demo-token");
        }

        private string ExtractOrderIdFromResponse(string response)
        {
            try
            {
                var json = JsonSerializer.Deserialize<JsonElement>(response);
                if (json.TryGetProperty("orderId", out var orderIdElement))
                {
                    return orderIdElement.GetString();
                }
                if (json.TryGetProperty("order_id", out var orderIdElement2))
                {
                    return orderIdElement2.GetString();
                }
            }
            catch
            {
                // JSON parse hatası
            }
            
            return $"ORDER-{DateTime.Now:yyyyMMddHHmmss}";
        }

        private string ExtractTrackingFromResponse(string response)
        {
            try
            {
                var json = JsonSerializer.Deserialize<JsonElement>(response);
                if (json.TryGetProperty("trackingNumber", out var trackingElement))
                {
                    return trackingElement.GetString();
                }
            }
            catch
            {
                // JSON parse hatası
            }
            
            return null;
        }

        /// <summary>
        /// Sipariş durumunu günceller
        /// </summary>
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string supplierOrderId)
        {
            try
            {
                var siparis = await _context.Siparisler.FindAsync(orderId);
                if (siparis == null) return false;

                // Tedarikçiden sipariş durumunu çek
                // Bu kısım tedarikçiye göre özelleştirilecek

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order status for Order ID: {orderId}");
                return false;
            }
        }
    }

    public class SupplierOrderResult
    {
        public string SupplierName { get; set; }
        public bool Success { get; set; }
        public string OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public string ErrorMessage { get; set; }
        public string Response { get; set; }
    }
}