namespace TrendyolClone.Models
{
    public class Siparis
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;
        public DateTime? OdemeTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public SiparisDurumu Durum { get; set; } = SiparisDurumu.Beklemede;
        public string TeslimatAdresi { get; set; }
        public string? FaturaAdresi { get; set; }
        public string OdemeYontemi { get; set; } = "Kredi Kartı";
        public string SiparisNo { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
        public decimal KargoUcreti { get; set; }
        public decimal IndirimTutari { get; set; }
        public string? AdminNotu { get; set; }
        public ICollection<SiparisKalemi> SiparisKalemleri { get; set; }
    }
    
    public enum SiparisDurumu
    {
        Beklemede = 0,
        Onaylandi = 1,
        Hazirlaniyor = 2,
        Kargoda = 3,
        KargoyaVerildi = 3, // Alias
        TeslimEdildi = 4,
        IptalEdildi = 5,
        Iptal = 5 // Alias
    }
    
    public enum OdemeYontemi
    {
        KrediKarti,
        BankaKarti,
        KapiOdeme,
        HavaleEFT
    }
}
