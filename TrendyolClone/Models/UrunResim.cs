using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class UrunResim
    {
        public int Id { get; set; }
        
        [Required]
        public int UrunId { get; set; }
        public Urun Urun { get; set; }
        
        public int? VaryasyonId { get; set; }
        public UrunVaryasyon? Varyasyon { get; set; }
        
        [Required]
        [StringLength(500)]
        public string ResimUrl { get; set; }
        
        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }
        
        public int Sira { get; set; }
        
        public bool AnaResim { get; set; }
        
        public DateTime YuklemeTarihi { get; set; } = DateTime.Now;
    }
}
