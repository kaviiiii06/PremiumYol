using System;
using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class YorumRapor
    {
        public int Id { get; set; }
        
        public int YorumId { get; set; }
        public UrunYorum? Yorum { get; set; }
        
        public int RaporEdenKullaniciId { get; set; }
        public Kullanici? RaporEdenKullanici { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Sebep { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Aciklama { get; set; }
        
        public RaporDurum Durum { get; set; } = RaporDurum.Beklemede;
        
        public DateTime Tarih { get; set; } = DateTime.Now;
        
        public int? InceleyenYoneticiId { get; set; }
        
        public DateTime? IncelemeTarihi { get; set; }
        
        [StringLength(500)]
        public string? YoneticiNotu { get; set; }
    }
    
    public enum RaporDurum
    {
        Beklemede = 0,
        Incelendi = 1,
        Kabul = 2,
        Red = 3
    }
}
