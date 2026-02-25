namespace TrendyolClone.Services
{
    public class SmsSender : ISmsSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsSender> _logger;

        public SmsSender(IConfiguration configuration, ILogger<SmsSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                // SMS API entegrasyonu burada yapılır
                // Örnek: Netgsm, İleti Merkezi, Twilio vb.
                
                var apiUrl = _configuration["Sms:ApiUrl"];
                var apiKey = _configuration["Sms:ApiKey"];
                var sender = _configuration["Sms:Sender"] ?? "TRENDYOL";

                // Demo amaçlı - gerçek uygulamada SMS API'sine istek atılır
                _logger.LogInformation($"SMS gönderildi: {phoneNumber} - {message}");
                
                // Simüle edilmiş başarılı gönderim
                await Task.Delay(100);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SMS gönderme hatası: {phoneNumber}");
                return false;
            }
        }

        public async Task<bool> SendBulkSmsAsync(List<string> phoneNumbers, string message)
        {
            var tasks = phoneNumbers.Select(phone => SendSmsAsync(phone, message));
            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }
    }
}
