using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class SiparisKalemi
    {
        public int Id { get; set; }
        
        public int SiparisId { get; set; }
        public virtual Siparis Siparis { get; set; }
        
        public int UrunId { get; set; }
        public virtual Urun Urun { get; set; }
        
        [Required]
        public int Miktar { get; set; }
        
        // Alias for Miktar
        public int Adet 
        { 
            get => Miktar; 
            set => Miktar = value; 
        }
        
        [Required]
        public decimal BirimFiyat { get; set; }
        
        public decimal ToplamFiyat => Miktar * BirimFiyat;
    }
}
