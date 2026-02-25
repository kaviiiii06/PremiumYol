using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml;
using TrendyolClone.Data;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public class SitemapService
    {
        private readonly UygulamaDbContext _context;
        private readonly IConfiguration _configuration;

        public SitemapService(UygulamaDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> GenerateSitemapXmlAsync()
        {
            var baseUrl = _configuration["SiteSettings:BaseUrl"] ?? "https://example.com";
            var items = new List<SitemapItemDto>();

            // Ana sayfa
            items.Add(new SitemapItemDto
            {
                Url = baseUrl,
                SonDegisiklik = DateTime.Now,
                DegisiklikSikligi = "daily",
                Oncelik = 1.0
            });

            // Ürünler
            var urunler = await _context.Urunler
                .Where(u => u.Aktif)
                .Select(u => new { u.Id, u.OlusturmaTarihi })
                .ToListAsync();

            foreach (var urun in urunler)
            {
                items.Add(new SitemapItemDto
                {
                    Url = $"{baseUrl}/Product/Details/{urun.Id}",
                    SonDegisiklik = urun.OlusturmaTarihi,
                    DegisiklikSikligi = "weekly",
                    Oncelik = 0.8
                });
            }

            // Kategoriler
            var kategoriler = await _context.Kategoriler
                .Select(k => k.Id)
                .ToListAsync();

            foreach (var kategoriId in kategoriler)
            {
                items.Add(new SitemapItemDto
                {
                    Url = $"{baseUrl}/Product?kategoriId={kategoriId}",
                    SonDegisiklik = DateTime.Now,
                    DegisiklikSikligi = "daily",
                    Oncelik = 0.7
                });
            }

            // Statik sayfalar
            var staticPages = new[]
            {
                new { Url = "/Account/Login", Priority = 0.5 },
                new { Url = "/Account/Register", Priority = 0.5 },
                new { Url = "/Cart", Priority = 0.6 }
            };

            foreach (var page in staticPages)
            {
                items.Add(new SitemapItemDto
                {
                    Url = $"{baseUrl}{page.Url}",
                    SonDegisiklik = DateTime.Now,
                    DegisiklikSikligi = "monthly",
                    Oncelik = page.Priority
                });
            }

            return GenerateXml(items);
        }

        private string GenerateXml(List<SitemapItemDto> items)
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using (var writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                foreach (var item in items)
                {
                    writer.WriteStartElement("url");
                    
                    writer.WriteElementString("loc", item.Url);
                    writer.WriteElementString("lastmod", item.SonDegisiklik.ToString("yyyy-MM-dd"));
                    writer.WriteElementString("changefreq", item.DegisiklikSikligi);
                    writer.WriteElementString("priority", item.Oncelik.ToString("0.0"));
                    
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            return sb.ToString();
        }
    }
}
