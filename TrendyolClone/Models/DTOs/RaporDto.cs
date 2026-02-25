namespace TrendyolClone.Models.DTOs
{
    public class SatisRaporuDto
    {
        public DateTime Tarih { get; set; }
        public decimal ToplamSatis { get; set; }
        public int SiparisSayisi { get; set; }
        public decimal OrtalamaSeperTutari { get; set; }
        public int UrunSayisi { get; set; }
    }

    public class UrunSatisRaporuDto
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string KategoriAdi { get; set; } = string.Empty;
        public string? ResimUrl { get; set; }
        public int SatisSayisi { get; set; }
        public decimal ToplamGelir { get; set; }
        public int StokMiktari { get; set; }
    }

    public class KategoriSatisRaporuDto
    {
        public int KategoriId { get; set; }
        public string KategoriAdi { get; set; } = string.Empty;
        public int UrunSayisi { get; set; }
        public int SatisSayisi { get; set; }
        public decimal ToplamGelir { get; set; }
        public decimal Yuzde { get; set; }
    }

    public class KullaniciRaporuDto
    {
        public DateTime Tarih { get; set; }
        public int YeniKayit { get; set; }
        public int AktifKullanici { get; set; }
        public int SiparisVerenKullanici { get; set; }
    }

    public class FinansalRaporDto
    {
        public DateTime Tarih { get; set; }
        public decimal BrutSatis { get; set; }
        public decimal KargoGelirleri { get; set; }
        public decimal IndirimTutari { get; set; }
        public decimal IadeTutari { get; set; }
        public decimal NetSatis { get; set; }
    }

    public class RaporFiltre
    {
        public DateTime BaslangicTarihi { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime BitisTarihi { get; set; } = DateTime.Today;
        public string Periyot { get; set; } = "gunluk"; // gunluk, haftalik, aylik
        public int? KategoriId { get; set; }
        public int Limit { get; set; } = 20;
    }

    public class GenelRaporOzetiDto
    {
        public decimal ToplamSatis { get; set; }
        public int ToplamSiparis { get; set; }
        public decimal OrtalamaSeperTutari { get; set; }
        public int ToplamUrunSatisi { get; set; }
        public int YeniKullanici { get; set; }
        public decimal IadeTutari { get; set; }
        public decimal NetKar { get; set; }
        
        // Karşılaştırma
        public decimal SatisArtisi { get; set; }
        public decimal SiparisArtisi { get; set; }
    }
}
