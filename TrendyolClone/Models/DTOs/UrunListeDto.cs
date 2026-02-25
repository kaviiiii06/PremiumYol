namespace TrendyolClone.Models.DTOs
{
    public class UrunListeDto
    {
        public List<Urun> Urunler { get; set; } = new List<Urun>();
        public int ToplamSayfa { get; set; }
        public int MevcutSayfa { get; set; }
        public int ToplamUrun { get; set; }
        public bool SonrakiSayfaVarMi { get; set; }
        public bool OncekiSayfaVarMi { get; set; }
        public List<MarkaFiltre> MarkaFiltreleri { get; set; } = new List<MarkaFiltre>();
        public FiyatAralik FiyatAraligi { get; set; } = new FiyatAralik();
        public int ToplamStokluUrun { get; set; }
        public int ToplamIndirimliUrun { get; set; }
    }
    
    public class MarkaFiltre
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public int UrunSayisi { get; set; }
        public bool Secili { get; set; }
    }
    
    public class FiyatAralik
    {
        public decimal MinFiyat { get; set; }
        public decimal MaxFiyat { get; set; }
    }
}
