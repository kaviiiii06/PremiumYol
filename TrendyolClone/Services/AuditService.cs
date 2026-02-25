using TrendyolClone.Data;
using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public interface IAuditService
    {
        Task LogActionAsync(int? userId, int? adminId, string action, string entityType, int? entityId, string oldValue = null, string newValue = null, string ipAddress = null, string userAgent = null);
        Task<List<IslemKaydi>> GetUserAuditLogsAsync(int userId, int count = 50);
        Task<List<IslemKaydi>> GetEntityAuditLogsAsync(string entityType, int entityId);
    }

    public class AuditService : IAuditService
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<AuditService> _logger;

        public AuditService(UygulamaDbContext context, ILogger<AuditService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogActionAsync(int? userId, int? adminId, string action, string entityType, 
            int? entityId, string oldValue = null, string newValue = null, string ipAddress = null, string userAgent = null)
        {
            try
            {
                var islemKaydi = new IslemKaydi
                {
                    KullaniciId = userId,
                    YoneticiId = adminId,
                    Islem = action,
                    VarlikTipi = entityType,
                    VarlikId = entityId,
                    EskiDeger = oldValue,
                    YeniDeger = newValue,
                    IpAdresi = ipAddress,
                    KullaniciAjanı = userAgent,
                    OlusturmaTarihi = DateTime.Now
                };

                _context.IslemKayitlari.Add(islemKaydi);
                await _context.SaveChangesAsync();

                _logger.LogInformation("İşlem kaydı oluşturuldu: {Action} on {EntityType} {EntityId}", 
                    action, entityType, entityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlem kaydı oluşturulamadı");
            }
        }

        public async Task<List<IslemKaydi>> GetUserAuditLogsAsync(int userId, int count = 50)
        {
            // Geçici olarak boş liste döndür - mapping gerekli
            await Task.CompletedTask;
            return new List<IslemKaydi>();
        }

        public async Task<List<IslemKaydi>> GetEntityAuditLogsAsync(string entityType, int entityId)
        {
            // Geçici olarak boş liste döndür - mapping gerekli
            await Task.CompletedTask;
            return new List<IslemKaydi>();
        }
    }
}
