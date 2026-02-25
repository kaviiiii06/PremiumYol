using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Urun
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Ad { get; set; }
        
        [Required]
        public string Aciklama { get; set; }
        
        [Required]
        [Range(0.01, 999999)]
        public decimal Fiyat { get; set; }
        
        public decimal? IndirimliFiyat { get; set; }
        
        [Required]
        public string ResimUrl { get; set; }
        
        public int KategoriId { get; set; }
        public Kategori Kategori { get; set; }
        
        public int Stok { get; set; }
        public bool Aktif { get; set; } = true;
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        
        // Marka ilişkisi (string yerine FK)
        public int? MarkaId { get; set; }
        public Marka? Marka { get; set; }
        
        [StringLength(100)]
        public string? MarkaAdi { get; set; } // Geriye dönük uyumluluk için
        
        // Değerlendirme
        public double Puan { get; set; }
        public int YorumSayisi { get; set; }
        
        // SEO Alanları
        [StringLength(200)]
        public string? Slug { get; set; }
        
        [StringLength(200)]
        public string? MetaTitle { get; set; }
        
        [StringLength(500)]
        public string? MetaDescription { get; set; }
        
        [StringLength(500)]
        public string? CanonicalUrl { get; set; }
        
        // İstatistikler
        public int GoruntulenmeSayisi { get; set; } = 0;
        
        public int SatinAlmaSayisi { get; set; } = 0;
        
        public int FavoriSayisi { get; set; } = 0;
        
        // Popülerlik skoru (otomatik hesaplanabilir)
        public decimal PopuleriteSkor { get; set; } = 0;
        
        // Tedarikçi
        public int? TedarikciId { get; set; }
        public virtual Tedarikci? Tedarikci { get; set; }
        
        [StringLength(100)]
        public string? TedarikciUrunId { get; set; }
        
        [StringLength(500)]
        public string? TedarikciUrl { get; set; }
        
        // Kampanya ilişkisi
        public List<UrunKampanya> Kampanyalar { get; set; } = new List<UrunKampanya>();
        
        // Hesaplanan özellikler
        public decimal GecerliFiyat => IndirimliFiyat ?? Fiyat;
        
        public bool IndirimdeMi => IndirimliFiyat.HasValue && IndirimliFiyat < Fiyat;
        
        public decimal IndirimOrani => IndirimdeMi ? ((Fiyat - IndirimliFiyat!.Value) / Fiyat * 100) : 0;
    }
}
