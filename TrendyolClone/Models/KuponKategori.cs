using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class KuponKategori
    {
        public int Id { get; set; }
        
        [Required]
        public int KuponId { get; set; }
        public Kupon Kupon { get; set; } = null!;
        
        [Required]
        public int KategoriId { get; set; }
        public Kategori Kategori { get; set; } = null!;
    }
}
