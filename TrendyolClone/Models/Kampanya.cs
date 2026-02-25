using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Kampanya
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Kampanya adı zorunludur")]
        [StringLength(200)]
        public string Ad { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Aciklama { get; set; }
        
        [StringLength(200)]
        public string? Slug { get; set; }
        
        // Kampanya türü
        [Required]
        public KampanyaTuru Turu { get; set; }
        
        // İndirim bilgileri
        [Required]
        public decimal IndirimMiktari { get; set; }
        
        public decimal? MaksimumIndirim { get; set; }
        
        // Koşullar
        public decimal? MinimumSepetTutari { get; set; }
        
        public int? MinimumUrunAdedi { get; set; }
        
        // Tarih
        [Required]
        public DateTime BaslangicTarihi { get; set; }
        
        [Required]
        public DateTime BitisTarihi { get; set; }
        
        // Durum
        public bool Aktif { get; set; } = true;
        
        // Görsel
        [StringLength(500)]
        public string? BannerUrl { get; set; }
        
        // Öncelik (birden fazla kampanya varsa)
        public int Oncelik { get; set; } = 0;
        
        // İstatistikler
        public int KullanimSayisi { get; set; } = 0;
        
        public decimal ToplamIndirimTutari { get; set; } = 0;
        
        // Tarihler
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }
        
        // İlişkiler
        public List<UrunKampanya> UrunKampanyalari { get; set; } = new List<UrunKampanya>();
        
        // Hesaplanan özellikler
        public bool GecerliMi => Aktif && 
                                 DateTime.Now >= BaslangicTarihi && 
                                 DateTime.Now <= BitisTarihi;
    }
    
    public enum KampanyaTuru
    {
        Yuzde = 1,          // %20 indirim
        Tutar = 2,          // 50 TL indirim
        AlBirAl = 3,        // 2 al 1 öde
        UcretsizKargo = 4,  // Ücretsiz kargo
        Hediye = 5          // Hediye ürün
    }
}
