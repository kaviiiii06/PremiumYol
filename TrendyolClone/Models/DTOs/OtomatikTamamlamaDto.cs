namespace TrendyolClone.Models.DTOs
{
    public class OtomatikTamamlamaDto
    {
        public string Terim { get; set; } = string.Empty;
        public string Tip { get; set; } = string.Empty; // Ürün, Kategori, Marka, Geçmiş
        public string? Kategori { get; set; }
    }
}
