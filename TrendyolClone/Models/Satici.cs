using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Satici
    {
        public int Id { get; set; }
        
        public int KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }
        
        [Required]
        [StringLength(100)]
        public string MagazaAdi { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Slug { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Aciklama { get; set; }
        
        [StringLength(500)]
        public string? Logo { get; set; }
        
        [StringLength(500)]
        public string? KapakResmi { get; set; }
        
        [StringLength(20)]
        public string? VergiNo { get; set; }
        
        [StringLength(100)]
        public string? VergiDairesi { get; set; }
        
        [StringLength(200)]
        public string? TicariUnvan { get; set; }
        
        [StringLength(20)]
        public string? Telefon { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(500)]
        public string? Adres { get; set; }
        
        public decimal KomisyonOrani { get; set; } = 15; // %15 varsayılan
        
        public SaticiDurum Durum { get; set; } = SaticiDurum.Beklemede;
        
        public DateTime? OnayTarihi { get; set; }
        
        public DateTime KayitTarihi { get; set; } = DateTime.Now;
        
        public bool AktifMi { get; set; } = true;
        
        // İstatistikler
        public int ToplamSatis { get; set; }
        public decimal ToplamKazanc { get; set; }
        public decimal OrtalamaPuan { get; set; }
        public int DegerlendirmeSayisi { get; set; }
        
        // Navigation Properties
        public List<SaticiUrun> Urunler { get; set; } = new();
        public List<SaticiSiparis> Siparisler { get; set; } = new();
        public List<SaticiOdeme> Odemeler { get; set; } = new();
        public List<SaticiDegerlendirme> Degerlendirmeler { get; set; } = new();
    }
    
    public enum SaticiDurum
    {
        Beklemede = 0,
        Onaylandi = 1,
        Reddedildi = 2,
        AskiyaAlindi = 3
    }
}
