using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Yorum
    {
        public int Id { get; set; }
        
        [Required]
        public int UrunId { get; set; }
        public Urun Urun { get; set; }
        
        [Required]
        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Puan { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Icerik { get; set; }
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        
        public bool Onaylandi { get; set; } = true;
        
        public bool DogrulanmisSatin { get; set; } = false;
    }
}
