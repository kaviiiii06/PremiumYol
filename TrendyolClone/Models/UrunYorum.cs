using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class UrunYorum
    {
        public int Id { get; set; }
        
        public int UrunId { get; set; }
        public Urun? Urun { get; set; }
        
        public int KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }
        
        public int? SiparisId { get; set; }
        public Siparis? Siparis { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Baslik { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string YorumMetni { get; set; } = string.Empty;
        
        [Range(1, 5)]
        public int GenelPuan { get; set; }
        
        [Range(1, 5)]
        public int UrunKalitesiPuan { get; set; }
        
        [Range(1, 5)]
        public int FiyatPerformansPuan { get; set; }
        
        [Range(1, 5)]
        public int KargoHiziPuan { get; set; }
        
        [Range(1, 5)]
        public int PaketlemePuan { get; set; }
        
        public bool OnaylandiMi { get; set; } = false;
        
        public YorumDurum Durum { get; set; } = YorumDurum.Beklemede;
        
        public int YardimciBuldum { get; set; } = 0;
        
        public int YardimciBulmadim { get; set; } = 0;
        
        public int SikayetSayisi { get; set; } = 0;
        
        public DateTime Tarih { get; set; } = DateTime.Now;
        
        public DateTime? GuncellenmeTarihi { get; set; }
        
        [StringLength(500)]
        public string? RedSebep { get; set; }
        
        // Navigation Properties
        public List<YorumResim> Resimler { get; set; } = new();
        public List<YorumVideo> Videolar { get; set; } = new();
        public List<YorumEtkilesim> Etkilesimler { get; set; } = new();
        public SaticiYorumYaniti? SaticiYaniti { get; set; }
        public List<YorumRapor> Raporlar { get; set; } = new();
    }
    
    public enum YorumDurum
    {
        Beklemede = 0,
        Onaylandi = 1,
        Reddedildi = 2,
        Spam = 3
    }
}
