using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class SiteAyarlari
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string SiteAdi { get; set; } = "PremiumYol";
        
        [StringLength(200)]
        public string SiteAciklamasi { get; set; } = "Premium E-Ticaret Platformu";
        
        [StringLength(500)]
        public string LogoUrl { get; set; } = "";
        
        [StringLength(50)]
        public string LogoIcon { get; set; } = "fas fa-crown";
        
        [StringLength(100)]
        public string IletisimEmail { get; set; } = "info@onewearr.shop";
        
        [StringLength(20)]
        public string IletisimTelefon { get; set; } = "0538 969 36 06";
        
        [StringLength(200)]
        public string AltBilgiMetni { get; set; } = "Tüm hakları saklıdır.";
        
        public DateTime GuncellemeTarihi { get; set; } = DateTime.Now;
        
        public bool Aktif { get; set; } = true;
    }
}
