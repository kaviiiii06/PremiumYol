using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class UrunVaryasyon
    {
        public int Id { get; set; }
        
        [Required]
        public int UrunId { get; set; }
        public Urun Urun { get; set; }
        
        // Varyasyon özellikleri
        [Required]
        [StringLength(50)]
        public string SKU { get; set; } // Stok kodu (benzersiz)
        
        [StringLength(50)]
        public string? Renk { get; set; }
        
        [StringLength(20)]
        public string? Beden { get; set; }
        
        [StringLength(100)]
        public string? DigerOzellik { get; set; }
        
        // Fiyat ve stok
        [Required]
        public decimal Fiyat { get; set; }
        
        public decimal? IndirimliFiyat { get; set; }
        
        [Required]
        public int Stok { get; set; }
        
        [Required]
        public int MinStok { get; set; } = 5; // Uyarı için minimum stok
        
        // Görseller
        [StringLength(500)]
        public string? AnaResim { get; set; }
        
        public List<UrunResim> Resimler { get; set; } = new List<UrunResim>();
        
        // Durum
        public bool Aktif { get; set; } = true;
        
        public int Sira { get; set; } // Gösterim sırası
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        
        // Hesaplanan özellikler
        public bool StokDusukMu => Stok <= MinStok;
        public bool IndirimdeMi => IndirimliFiyat.HasValue && IndirimliFiyat < Fiyat;
        public decimal GecerliFiyat => IndirimliFiyat ?? Fiyat;
    }
}
