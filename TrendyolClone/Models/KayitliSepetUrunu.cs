using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class KayitliSepetUrunu
    {
        public int Id { get; set; }
        
        [Required]
        public int KayitliSepetId { get; set; }
        public KayitliSepet KayitliSepet { get; set; } = null!;
        
        [Required]
        public int UrunId { get; set; }
        public Urun Urun { get; set; } = null!;
        
        public int? VaryasyonId { get; set; }
        public UrunVaryasyon? Varyasyon { get; set; }
        
        [Required]
        public int Adet { get; set; }
        
        // Fiyat bilgisi (kayıt anındaki fiyat)
        public decimal BirimFiyat { get; set; }
        
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
        
        // Hesaplanan özellikler
        public decimal ToplamFiyat => BirimFiyat * Adet;
        public bool StokVarMi => Varyasyon != null ? Varyasyon.Stok >= Adet : Urun.Stok >= Adet;
    }
}
