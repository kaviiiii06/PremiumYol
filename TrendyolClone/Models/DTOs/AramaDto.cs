namespace TrendyolClone.Models.DTOs
{
    public class AramaDto
    {
        public string? AramaTerimi { get; set; }
        public int? KategoriId { get; set; }
        public List<int>? MarkaIdleri { get; set; }
        public decimal? MinFiyat { get; set; }
        public decimal? MaxFiyat { get; set; }
        public bool? SadeceStoktakiler { get; set; }
        public bool? SadeceIndirimliler { get; set; }
        public double? MinPuan { get; set; }
        public string Siralama { get; set; } = "relevance"; // relevance, price-asc, price-desc, newest, popular
        public int Sayfa { get; set; } = 1;
        public int SayfaBoyutu { get; set; } = 20;
    }

    public class AramaSonucDto
    {
        public string AramaTerimi { get; set; } = string.Empty;
        public int ToplamSonuc { get; set; }
        public int Sayfa { get; set; }
        public int SayfaBoyutu { get; set; }
        public int ToplamSayfa { get; set; }
        public List<Urun> Urunler { get; set; } = new List<Urun>();
        public List<string> Oneriler { get; set; } = new List<string>();
        public AramaFiltreDto Filtreler { get; set; } = new AramaFiltreDto();
        public long AramaSuresi { get; set; } // milliseconds
    }

    public class AramaFiltreDto
    {
        public List<MarkaFiltre> Markalar { get; set; } = new List<MarkaFiltre>();
        public List<KategoriFiltre> Kategoriler { get; set; } = new List<KategoriFiltre>();
        public FiyatAralik FiyatAraligi { get; set; } = new FiyatAralik();
    }

    public class KategoriFiltre
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public int UrunSayisi { get; set; }
    }
}
