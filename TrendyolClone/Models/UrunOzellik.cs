using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class UrunOzellik
    {
        public int Id { get; set; }
        
        [Required]
        public int UrunId { get; set; }
        public Urun Urun { get; set; }
        
        [Required]
        [StringLength(100)]
        public string OzellikAdi { get; set; } // Marka, Malzeme, Garanti, vb.
        
        [Required]
        [StringLength(500)]
        public string OzellikDegeri { get; set; }
        
        public int Sira { get; set; }
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }
}
