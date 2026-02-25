using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Marka
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Marka adı zorunludur")]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Slug { get; set; }
        
        [StringLength(500)]
        public string? Aciklama { get; set; }
        
        [StringLength(500)]
        public string? LogoUrl { get; set; }
        
        // SEO
        [StringLength(200)]
        public string? MetaTitle { get; set; }
        
        [StringLength(500)]
        public string? MetaDescription { get; set; }
        
        // Durum
        public bool Aktif { get; set; } = true;
        
        public int Sira { get; set; } = 0;
        
        // İstatistikler
        public int UrunSayisi { get; set; } = 0;
        
        // Tarihler
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }
        
        // İlişkiler
        public List<Urun> Urunler { get; set; } = new List<Urun>();
    }
}
