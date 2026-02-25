namespace TrendyolClone.Models.DTOs
{
    public class SeoMetaDto
    {
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string Anahtar { get; set; } = string.Empty;
        public string CanonicalUrl { get; set; } = string.Empty;
        public string OgBaslik { get; set; } = string.Empty;
        public string OgAciklama { get; set; } = string.Empty;
        public string OgResim { get; set; } = string.Empty;
        public string OgUrl { get; set; } = string.Empty;
        public bool Indexlenebilir { get; set; } = true;
        public bool TakipEdilebilir { get; set; } = true;
    }

    public class SitemapItemDto
    {
        public string Url { get; set; } = string.Empty;
        public DateTime SonDegisiklik { get; set; }
        public string DegisiklikSikligi { get; set; } = "weekly";
        public double Oncelik { get; set; } = 0.5;
    }

    public class AnalyticsEventDto
    {
        public string EventName { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}
