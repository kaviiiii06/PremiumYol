namespace TrendyolClone.Models
{
    public class SepetKalemi
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; }
        public decimal Fiyat { get; set; }
        public int Miktar { get; set; }
        public string ResimUrl { get; set; }
        public decimal ToplamFiyat => Fiyat * Miktar;
    }
}
