using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Favori
    {
        public int Id { get; set; }
        
        [Required]
        public int KullaniciId { get; set; }
        public virtual Kullanici Kullanici { get; set; }
        
        [Required]
        public int UrunId { get; set; }
        public virtual Urun Urun { get; set; }
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }
}
