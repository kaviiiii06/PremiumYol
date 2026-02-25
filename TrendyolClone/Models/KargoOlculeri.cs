using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class KargoOlculeri
    {
        public int Id { get; set; }
        
        [Required]
        public int UrunId { get; set; }
        public Urun Urun { get; set; }
        
        // Ölçüler (cm)
        [Required]
        public decimal En { get; set; }
        
        [Required]
        public decimal Boy { get; set; }
        
        [Required]
        public decimal Yukseklik { get; set; }
        
        // Desi otomatik hesaplanır: (En x Boy x Yükseklik) / 3000
        public decimal Desi => (En * Boy * Yukseklik) / 3000;
        
        // Ağırlık (kg)
        [Required]
        public decimal Agirlik { get; set; }
        
        // Kargo bilgileri
        public bool UcretsizKargo { get; set; }
        
        public decimal? KargoUcreti { get; set; }
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }
    }
}
