using System;
using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class SaticiUrun
    {
        public int Id { get; set; }
        
        public int SaticiId { get; set; }
        public Satici? Satici { get; set; }
        
        public int UrunId { get; set; }
        public Urun? Urun { get; set; }
        
        public int Stok { get; set; }
        
        [Required]
        public decimal Fiyat { get; set; }
        
        public decimal? IndirimliFiyat { get; set; }
        
        public int KargoSuresi { get; set; } = 3; // Gün
        
        public SaticiUrunDurum Durum { get; set; } = SaticiUrunDurum.Taslak;
        
        public DateTime? OnayTarihi { get; set; }
        
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
        
        public DateTime? GuncellenmeTarihi { get; set; }
        
        [StringLength(500)]
        public string? RedSebep { get; set; }
        
        // İstatistikler
        public int GoruntulemeSayisi { get; set; }
        public int SatisSayisi { get; set; }
        public decimal ToplamSatisTutari { get; set; }
    }
    
    public enum SaticiUrunDurum
    {
        Taslak = 0,
        OnayBekliyor = 1,
        Yayinda = 2,
        Reddedildi = 3,
        StokTukendi = 4,
        Pasif = 5
    }
}
