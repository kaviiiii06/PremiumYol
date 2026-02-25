using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public class SeoService : ISeoService
    {
        private readonly UygulamaDbContext _context;
        private readonly IConfiguration _configuration;

        public SeoService(UygulamaDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<SeoMetaDto> GetSeoMetaAsync(string sayfaTipi, int? referansId = null)
        {
            var seoAyar = await _context.Set<SeoAyarlari>()
                .FirstOrDefaultAsync(s => s.SayfaTipi == sayfaTipi && s.ReferansId == referansId);

            if (seoAyar != null)
            {
                return new SeoMetaDto
                {
                    Baslik = seoAyar.Baslik,
                    Aciklama = seoAyar.Aciklama,
                    Anahtar = seoAyar.Anahtar,
                    CanonicalUrl = seoAyar.CanonicalUrl,
                    OgBaslik = seoAyar.OgBaslik,
                    OgAciklama = seoAyar.OgAciklama,
                    OgResim = seoAyar.OgResim,
                    Indexlenebilir = seoAyar.Indexlenebilir,
                    TakipEdilebilir = seoAyar.TakipEdilebilir
                };
            }

            // Varsayılan SEO meta oluştur
            return sayfaTipi switch
            {
                "Product" when referansId.HasValue => await GenerateProductSeoAsync(referansId.Value),
                "Category" when referansId.HasValue => await GenerateCategorySeoAsync(referansId.Value),
                _ => GetDefaultSeoMeta()
            };
        }

        public async Task<bool> UpdateSeoMetaAsync(string sayfaTipi, int? referansId, SeoMetaDto seoMeta)
        {
            var seoAyar = await _context.Set<SeoAyarlari>()
                .FirstOrDefaultAsync(s => s.SayfaTipi == sayfaTipi && s.ReferansId == referansId);

            if (seoAyar == null)
            {
                seoAyar = new SeoAyarlari
                {
                    SayfaTipi = sayfaTipi,
                    ReferansId = referansId
                };
                _context.Set<SeoAyarlari>().Add(seoAyar);
            }

            seoAyar.Baslik = seoMeta.Baslik;
            seoAyar.Aciklama = seoMeta.Aciklama;
            seoAyar.Anahtar = seoMeta.Anahtar;
            seoAyar.CanonicalUrl = seoMeta.CanonicalUrl;
            seoAyar.OgBaslik = seoMeta.OgBaslik;
            seoAyar.OgAciklama = seoMeta.OgAciklama;
            seoAyar.OgResim = seoMeta.OgResim;
            seoAyar.Indexlenebilir = seoMeta.Indexlenebilir;
            seoAyar.TakipEdilebilir = seoMeta.TakipEdilebilir;
            seoAyar.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SeoMetaDto> GenerateProductSeoAsync(int urunId)
        {
            var urun = await _context.Urunler
                .Include(u => u.Kategori)
                .FirstOrDefaultAsync(u => u.Id == urunId);

            if (urun == null)
                return GetDefaultSeoMeta();

            var siteAdi = _configuration["SiteSettings:Name"] ?? "E-Ticaret";
            var baseUrl = _configuration["SiteSettings:BaseUrl"] ?? "";

            return new SeoMetaDto
            {
                Baslik = $"{urun.Ad} - {siteAdi}",
                Aciklama = TruncateText(urun.Aciklama, 160),
                Anahtar = $"{urun.Ad}, {urun.Kategori?.Ad}, online alışveriş",
                CanonicalUrl = $"{baseUrl}/Product/Details/{urunId}",
                OgBaslik = urun.Ad,
                OgAciklama = TruncateText(urun.Aciklama, 200),
                OgResim = urun.ResimUrl,
                OgUrl = $"{baseUrl}/Product/Details/{urunId}",
                Indexlenebilir = urun.Aktif,
                TakipEdilebilir = true
            };
        }

        public async Task<SeoMetaDto> GenerateCategorySeoAsync(int kategoriId)
        {
            var kategori = await _context.Kategoriler
                .FirstOrDefaultAsync(k => k.Id == kategoriId);

            if (kategori == null)
                return GetDefaultSeoMeta();

            var siteAdi = _configuration["SiteSettings:Name"] ?? "E-Ticaret";
            var baseUrl = _configuration["SiteSettings:BaseUrl"] ?? "";
            var urunSayisi = await _context.Urunler.CountAsync(u => u.KategoriId == kategoriId && u.Aktif);

            return new SeoMetaDto
            {
                Baslik = $"{kategori.Ad} - {siteAdi}",
                Aciklama = $"{kategori.Ad} kategorisinde {urunSayisi} ürün. En uygun fiyatlarla online alışveriş.",
                Anahtar = $"{kategori.Ad}, {kategori.Ad} ürünleri, online alışveriş",
                CanonicalUrl = $"{baseUrl}/Product?kategoriId={kategoriId}",
                OgBaslik = kategori.Ad,
                OgAciklama = $"{kategori.Ad} kategorisinde {urunSayisi} ürün",
                OgUrl = $"{baseUrl}/Product?kategoriId={kategoriId}",
                Indexlenebilir = true,
                TakipEdilebilir = true
            };
        }

        public string GenerateSeoFriendlyUrl(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Türkçe karakterleri değiştir
            var turkishChars = new Dictionary<char, char>
            {
                {'ı', 'i'}, {'İ', 'I'}, {'ğ', 'g'}, {'Ğ', 'G'},
                {'ü', 'u'}, {'Ü', 'U'}, {'ş', 's'}, {'Ş', 'S'},
                {'ö', 'o'}, {'Ö', 'O'}, {'ç', 'c'}, {'Ç', 'C'}
            };

            var sb = new StringBuilder();
            foreach (var c in text.ToLowerInvariant())
            {
                sb.Append(turkishChars.ContainsKey(c) ? turkishChars[c] : c);
            }

            // Özel karakterleri temizle
            var url = Regex.Replace(sb.ToString(), @"[^a-z0-9\s-]", "");
            url = Regex.Replace(url, @"\s+", " ").Trim();
            url = Regex.Replace(url, @"\s", "-");

            return url;
        }

        private SeoMetaDto GetDefaultSeoMeta()
        {
            var siteAdi = _configuration["SiteSettings:Name"] ?? "E-Ticaret";
            var siteAciklama = _configuration["SiteSettings:Description"] ?? "Online alışveriş platformu";

            return new SeoMetaDto
            {
                Baslik = siteAdi,
                Aciklama = siteAciklama,
                Anahtar = "online alışveriş, e-ticaret",
                Indexlenebilir = true,
                TakipEdilebilir = true
            };
        }

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
        }
    }
}
