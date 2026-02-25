using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Adres
    {
        public int Id { get; set; }
        
        [Required]
        public int KullaniciId { get; set; }
        
        public Kullanici Kullanici { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Baslik { get; set; } // Ev, İş, vb.
        
        [Required]
        [StringLength(100)]
        public string TamAd { get; set; }
        
        [Required]
        public string AdresSatiri { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Sehir { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Ilce { get; set; }
        
        [StringLength(10)]
        public string PostaKodu { get; set; }
        
        [Required]
        [Phone]
        public string TelefonNumarasi { get; set; }
        
        public bool Varsayilan { get; set; } = false;
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }
}
