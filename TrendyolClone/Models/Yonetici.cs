using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Yonetici
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; }
        
        [Required]
        public string Sifre { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        
        public bool Aktif { get; set; } = true;
    }
}
