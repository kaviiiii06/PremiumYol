using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace TrendyolClone.Api
{
    public class Index
    {
        public static async Task Handler(HttpContext context)
        {
            var builder = WebApplication.CreateBuilder();
            
            // Program.cs'deki yapılandırmayı kullan
            var startup = new Program();
            var app = builder.Build();
            
            await app.RunAsync();
        }
    }
}
