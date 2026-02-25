namespace TrendyolClone.Models.DTOs
{
    public class SiparisDetayDto
    {
        public int Id { get; set; }
        public string SiparisNo { get; set; } = string.Empty;
        public DateTime SiparisTarihi { get; set; }
        public SiparisDurumu Durum { get; set; }
        public decimal ToplamTutar { get; set; }
        public string TeslimatAdresi { get; set; } = string.Empty;
        public string OdemeYontemi { get; set; } = string.Empty;

        // Kargo
        public string? KargoFirmasi { get; set; }
        public string? KargoTakipNo { get; set; }
        public string? KargoDurumu { get; set; }
        public DateTime? TahminiTeslimatTarihi { get; set; }

        // Fatura
        public string? FaturaNo { get; set; }
        public string? FaturaPdfUrl { get; set; }

        // Ürünler
        public List<SiparisUrunDto> Urunler { get; set; } = new List<SiparisUrunDto>();

        // Durum Geçmişi
        public List<SiparisDurumDto> DurumGecmisi { get; set; } = new List<SiparisDurumDto>();

        // İade
        public bool IadeTalebiVarMi { get; set; }
        public IadeDurumu? IadeDurumu { get; set; }
    }

    public class SiparisUrunDto
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string? ResimUrl { get; set; }
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal ToplamFiyat { get; set; }
    }

    public class SiparisDurumDto
    {
        public SiparisDurumu Durum { get; set; }
        public string? Aciklama { get; set; }
        public DateTime Tarih { get; set; }
        public string? DegistirenKisi { get; set; }
    }

    public class IadeTalebiDto
    {
        public int SiparisId { get; set; }
        public IadeNedeni Neden { get; set; }
        public string? Aciklama { get; set; }
        public List<IadeUrunDto> Urunler { get; set; } = new List<IadeUrunDto>();
    }

    public class IadeUrunDto
    {
        public int UrunId { get; set; }
        public int Adet { get; set; }
    }

    public class SiparisDurumGuncellemeDto
    {
        public int SiparisId { get; set; }
        public SiparisDurumu YeniDurum { get; set; }
        public string? Aciklama { get; set; }
    }

    public class KargoTakipDto
    {
        public string KargoFirmasi { get; set; } = string.Empty;
        public string TakipNo { get; set; } = string.Empty;
        public string? KargoDurumu { get; set; }
        public DateTime? TahminiTeslimatTarihi { get; set; }
        public List<KargoHareketDto> Hareketler { get; set; } = new List<KargoHareketDto>();
    }

    public class KargoHareketDto
    {
        public string Durum { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string? Lokasyon { get; set; }
        public DateTime Tarih { get; set; }
    }
}
