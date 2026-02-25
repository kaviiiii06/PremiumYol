using System.Text.Json;

namespace TrendyolClone.Services
{
    public interface ILoggingService
    {
        Task LogUserActionAsync(int userId, string action, string details = null);
        Task LogErrorAsync(string source, string message, Exception exception = null);
        Task LogPaymentAsync(int orderId, string status, string details);
        Task LogApiCallAsync(string apiName, string endpoint, bool success, string response = null);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly string _logDirectory;

        public LoggingService(ILogger<LoggingService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _logDirectory = Path.Combine(env.ContentRootPath, "Logs");
            
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }

        public async Task LogUserActionAsync(int userId, string action, string details = null)
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                Type = "UserAction",
                UserId = userId,
                Action = action,
                Details = details
            };

            _logger.LogInformation("User {UserId} performed action: {Action}", userId, action);
            await WriteLogToFileAsync("user_actions", logEntry);
        }

        public async Task LogErrorAsync(string source, string message, Exception exception = null)
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                Type = "Error",
                Source = source,
                Message = message,
                Exception = exception?.ToString()
            };

            _logger.LogError(exception, "Error in {Source}: {Message}", source, message);
            await WriteLogToFileAsync("errors", logEntry);
        }

        public async Task LogPaymentAsync(int orderId, string status, string details)
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                Type = "Payment",
                OrderId = orderId,
                Status = status,
                Details = details
            };

            _logger.LogInformation("Payment for Order {OrderId}: {Status}", orderId, status);
            await WriteLogToFileAsync("payments", logEntry);
        }

        public async Task LogApiCallAsync(string apiName, string endpoint, bool success, string response = null)
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                Type = "ApiCall",
                ApiName = apiName,
                Endpoint = endpoint,
                Success = success,
                Response = response
            };

            _logger.LogInformation("API Call to {ApiName} - {Endpoint}: {Success}", 
                apiName, endpoint, success ? "Success" : "Failed");
            
            await WriteLogToFileAsync("api_calls", logEntry);
        }

        private async Task WriteLogToFileAsync(string category, object logEntry)
        {
            try
            {
                var fileName = $"{category}_{DateTime.Now:yyyyMMdd}.json";
                var filePath = Path.Combine(_logDirectory, fileName);
                
                var json = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                
                await File.AppendAllTextAsync(filePath, json + Environment.NewLine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write log to file");
            }
        }
    }
}
