using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class KayitliSepet
    {
        public int Id { get; set; }
        
        [Required]
        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty; // "Düğün Alışverişi", "Yılbaşı Hediyeleri"
        
        [StringLength(500)]
        public string? Aciklama { get; set; }
        
        public List<KayitliSepetUrunu> Urunler { get; set; } = new List<KayitliSepetUrunu>();
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }
        
        // Hesaplanan özellikler
        public int ToplamUrunSayisi => Urunler.Sum(u => u.Adet);
        public decimal ToplamTutar => Urunler.Sum(u => u.ToplamFiyat);
    }
}
