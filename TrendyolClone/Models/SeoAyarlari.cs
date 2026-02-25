namespace TrendyolClone.Models
{
    public class SeoAyarlari
    {
        public int Id { get; set; }
        public string SayfaTipi { get; set; } = string.Empty; // Home, Product, Category, etc.
        public int? ReferansId { get; set; } // UrunId, KategoriId, etc.
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string Anahtar { get; set; } = string.Empty;
        public string CanonicalUrl { get; set; } = string.Empty;
        public string OgBaslik { get; set; } = string.Empty;
        public string OgAciklama { get; set; } = string.Empty;
        public string OgResim { get; set; } = string.Empty;
        public bool Indexlenebilir { get; set; } = true;
        public bool TakipEdilebilir { get; set; } = true;
        public DateTime GuncellemeTarihi { get; set; } = DateTime.Now;
    }
}
