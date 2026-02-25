using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class UrunKampanya
    {
        public int Id { get; set; }
        
        [Required]
        public int UrunId { get; set; }
        public Urun Urun { get; set; } = null!;
        
        [Required]
        public int KampanyaId { get; set; }
        public Kampanya Kampanya { get; set; } = null!;
        
        // Ürüne özel indirim (kampanya genel indirimini override edebilir)
        public decimal? OzelIndirimMiktari { get; set; }
        
        // Stok limiti (kampanya için ayrılan stok)
        public int? StokLimiti { get; set; }
        
        public int KullanilanStok { get; set; } = 0;
        
        // Durum
        public bool Aktif { get; set; } = true;
        
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
        
        // Hesaplanan özellikler
        public bool StokVarMi => !StokLimiti.HasValue || KullanilanStok < StokLimiti.Value;
    }
}
