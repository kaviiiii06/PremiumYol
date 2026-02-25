namespace TrendyolClone.Models.DTOs
{
    public class DashboardDto
    {
        public DashboardIstatistikler Istatistikler { get; set; } = new();
        public List<GunlukSatis> GunlukSatislar { get; set; } = new();
        public List<PopulerUrun> PopulerUrunler { get; set; } = new();
        public List<SonSiparis> SonSiparisler { get; set; } = new();
        public List<StokUyari> StokUyarilari { get; set; } = new();
        public List<YeniKullanici> YeniKullanicilar { get; set; } = new();
    }

    public class DashboardIstatistikler
    {
        public decimal BugunkuSatis { get; set; }
        public int BugunkuSiparisSayisi { get; set; }
        public decimal AylikSatis { get; set; }
        public int AylikSiparisSayisi { get; set; }
        public int ToplamKullanici { get; set; }
        public int AktifKullanici { get; set; }
        public int BekleyenSiparis { get; set; }
        public int BekleyenIade { get; set; }
        public int DusukStokUrun { get; set; }
        public int TukenenUrun { get; set; }
        
        // Karşılaştırma
        public decimal SatisArtisi { get; set; } // Yüzde
        public decimal SiparisArtisi { get; set; } // Yüzde
    }

    public class GunlukSatis
    {
        public DateTime Tarih { get; set; }
        public decimal Tutar { get; set; }
        public int SiparisSayisi { get; set; }
    }

    public class PopulerUrun
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string ResimUrl { get; set; } = string.Empty;
        public int SatisSayisi { get; set; }
        public decimal ToplamGelir { get; set; }
    }

    public class SonSiparis
    {
        public int SiparisId { get; set; }
        public string SiparisNo { get; set; } = string.Empty;
        public string KullaniciAdi { get; set; } = string.Empty;
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
    }

    public class StokUyari
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string ResimUrl { get; set; } = string.Empty;
        public int MevcutStok { get; set; }
        public int MinimumStok { get; set; }
        public string UyariSeviyesi { get; set; } = string.Empty; // Kritik, Düşük
    }

    public class YeniKullanici
    {
        public int KullaniciId { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime KayitTarihi { get; set; }
        public int SiparisSayisi { get; set; }
    }
}
