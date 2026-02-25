using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class SepetUrunu
    {
        public int Id { get; set; }
        
        [Required]
        public int KullaniciId { get; set; }
        
        public Kullanici Kullanici { get; set; }
        
        [Required]
        public int UrunId { get; set; }
        
        public Urun Urun { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Adet { get; set; }
        
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
    }
}
