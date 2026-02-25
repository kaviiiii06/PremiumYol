using System.Collections.Concurrent;

namespace TrendyolClone.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = new();
        private readonly int _permitLimit;
        private readonly int _windowSeconds;

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _permitLimit = configuration.GetValue<int>("RateLimiting:PermitLimit", 100);
            _windowSeconds = configuration.GetValue<int>("RateLimiting:Window", 60);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<SkipRateLimitAttribute>() != null)
            {
                await _next(context);
                return;
            }

            var clientId = GetClientIdentifier(context);
            var clientInfo = _clients.GetOrAdd(clientId, _ => new ClientRequestInfo());

            lock (clientInfo)
            {
                var now = DateTime.UtcNow;
                
                if ((now - clientInfo.WindowStart).TotalSeconds > _windowSeconds)
                {
                    clientInfo.RequestCount = 0;
                    clientInfo.WindowStart = now;
                }

                if (clientInfo.RequestCount >= _permitLimit)
                {
                    context.Response.StatusCode = 429;
                    _logger.LogWarning("Rate limit exceeded for client: {ClientId}", clientId);
                    return;
                }

                clientInfo.RequestCount++;
            }

            await _next(context);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userId = context.Session.GetString("UserId") ?? "anonymous";
            return $"{ip}_{userId}";
        }

        private class ClientRequestInfo
        {
            public int RequestCount { get; set; }
            public DateTime WindowStart { get; set; } = DateTime.UtcNow;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SkipRateLimitAttribute : Attribute { }
}
