using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class OdemeYontemiAyari
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Ad { get; set; } // Kredi Kartı, Havale/EFT, Kapıda Ödeme
        
        [StringLength(500)]
        public string Aciklama { get; set; }
        
        public OdemeYontemiTipi Tip { get; set; } // Online, Offline
        
        // Havale/EFT için IBAN bilgileri
        [StringLength(100)]
        public string? BankaAdi { get; set; }
        
        [StringLength(50)]
        public string? IbanNo { get; set; }
        
        [StringLength(100)]
        public string? HesapSahibi { get; set; }
        
        [StringLength(500)]
        public string? EkBilgi { get; set; }
        
        public bool Aktif { get; set; } = true;
        
        public int Sira { get; set; } // Gösterim sırası
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }
    
    public enum OdemeYontemiTipi
    {
        Online = 1,  // Kredi Kartı, Online Ödeme
        Offline = 2  // Havale/EFT, Kapıda Ödeme
    }
}
#nullable restore
