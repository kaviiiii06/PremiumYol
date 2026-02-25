using System;
using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class SaticiDegerlendirme
    {
        public int Id { get; set; }
        
        public int SaticiId { get; set; }
        public Satici? Satici { get; set; }
        
        public int KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }
        
        public int SiparisId { get; set; }
        public Siparis? Siparis { get; set; }
        
        [Range(1, 5)]
        public int Puan { get; set; }
        
        [StringLength(1000)]
        public string? Yorum { get; set; }
        
        // Detaylı puanlama
        public int UrunKalitesiPuan { get; set; }
        public int KargoPuan { get; set; }
        public int IletisimPuan { get; set; }
        
        public DateTime Tarih { get; set; } = DateTime.Now;
        
        public bool OnaylandiMi { get; set; } = false;
        
        // Satıcı yanıtı
        [StringLength(1000)]
        public string? SaticiYaniti { get; set; }
        
        public DateTime? YanitTarihi { get; set; }
    }
}
