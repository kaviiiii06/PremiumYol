using TrendyolClone.Models;

namespace TrendyolClone.Models.DTOs
{
    public class SiparisYonetimDto
    {
        public int Id { get; set; }
        public string SiparisNo { get; set; } = string.Empty;
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public decimal ToplamTutar { get; set; }
        public SiparisDurumu Durum { get; set; }
        public string DurumAdi { get; set; } = string.Empty;
        public DateTime SiparisTarihi { get; set; }
        public int UrunSayisi { get; set; }
        public string TeslimatAdresi { get; set; } = string.Empty;
        public string? KargoTakipNo { get; set; }
        public string? KargoFirmasi { get; set; }
    }

    public class SiparisDetayYonetimDto
    {
        public int Id { get; set; }
        public string SiparisNo { get; set; } = string.Empty;
        
        // Kullanıcı Bilgileri
        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        
        // Sipariş Bilgileri
        public decimal AraToplam { get; set; }
        public decimal KargoUcreti { get; set; }
        public decimal IndirimTutari { get; set; }
        public decimal ToplamTutar { get; set; }
        public SiparisDurumu Durum { get; set; }
        public string DurumAdi { get; set; } = string.Empty;
        public DateTime SiparisTarihi { get; set; }
        public string? AdminNotu { get; set; }
        
        // Adres Bilgileri
        public string TeslimatAdresi { get; set; } = string.Empty;
        public string FaturaAdresi { get; set; } = string.Empty;
        
        // Kargo Bilgileri
        public string? KargoTakipNo { get; set; }
        public string? KargoFirmasi { get; set; }
        public DateTime? TahminiTeslimat { get; set; }
        
        // Ürünler
        public List<SiparisKalemDto> Urunler { get; set; } = new();
        
        // Durum Geçmişi
        public List<DurumGecmisiDto> DurumGecmisi { get; set; } = new();
    }

    public class SiparisKalemDto
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string ResimUrl { get; set; } = string.Empty;
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal ToplamFiyat { get; set; }
        public string? Varyasyon { get; set; }
    }

    public class DurumGecmisiDto
    {
        public SiparisDurumu Durum { get; set; }
        public string DurumAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public DateTime Tarih { get; set; }
        public string? DegistirenKisi { get; set; }
    }

    public class SiparisFiltre
    {
        public string? AramaTerimi { get; set; }
        public SiparisDurumu? Durum { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int Sayfa { get; set; } = 1;
        public int SayfaBoyutu { get; set; } = 20;
        public string Siralama { get; set; } = "tarih_desc";
    }

    public class SiparisListeResult
    {
        public List<SiparisYonetimDto> Siparisler { get; set; } = new();
        public int ToplamSayfa { get; set; }
        public int ToplamKayit { get; set; }
        public int MevcutSayfa { get; set; }
    }

    public class SiparisDurumGuncelleDto
    {
        public int SiparisId { get; set; }
        public SiparisDurumu YeniDurum { get; set; }
        public string? Aciklama { get; set; }
        public string? KargoTakipNo { get; set; }
        public string? KargoFirmasi { get; set; }
    }
}
