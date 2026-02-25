using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Kategori
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Ad { get; set; }
        
        public string Aciklama { get; set; }
        
        public string ResimUrl { get; set; }
        
        public bool Aktif { get; set; } = true;
        
        public ICollection<Urun> Urunler { get; set; } = new List<Urun>();
    }
}
