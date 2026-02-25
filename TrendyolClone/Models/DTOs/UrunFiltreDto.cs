namespace TrendyolClone.Models.DTOs
{
    public class UrunFiltreDto
    {
        public string? AramaTerimi { get; set; }
        public int? KategoriId { get; set; }
        public List<int>? MarkaIdleri { get; set; }
        public decimal? MinFiyat { get; set; }
        public decimal? MaxFiyat { get; set; }
        public bool? SadeceStoktakiler { get; set; }
        public bool? SadeceIndirimliler { get; set; }
        public double? MinPuan { get; set; }
        public UrunSiralamaTuru Siralama { get; set; } = UrunSiralamaTuru.Varsayilan;
        public int Sayfa { get; set; } = 1;
        public int SayfaBoyutu { get; set; } = 20;
    }
    
    public enum UrunSiralamaTuru
    {
        Varsayilan = 0,
        FiyatArtan = 1,
        FiyatAzalan = 2,
        YeniEklenenler = 3,
        PopulerOlanlar = 4,
        EnCokSatan = 5,
        EnYuksekPuan = 6,
        IndirimOrani = 7
    }
}
