using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class KuponKullanimi
    {
        public int Id { get; set; }
        
        [Required]
        public int KuponId { get; set; }
        public Kupon Kupon { get; set; } = null!;
        
        [Required]
        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; } = null!;
        
        public int? SiparisId { get; set; }
        public Siparis? Siparis { get; set; }
        
        [Required]
        public decimal IndirimTutari { get; set; }
        
        [Required]
        public DateTime KullanimTarihi { get; set; } = DateTime.Now;
        
        // Ek bilgiler
        public decimal SepetTutari { get; set; }
        public string? IpAdresi { get; set; }
    }
}
