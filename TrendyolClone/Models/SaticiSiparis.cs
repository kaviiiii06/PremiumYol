using System;
using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class SaticiSiparis
    {
        public int Id { get; set; }
        
        public int SaticiId { get; set; }
        public Satici? Satici { get; set; }
        
        public int SiparisId { get; set; }
        public Siparis? Siparis { get; set; }
        
        public int UrunId { get; set; }
        public Urun? Urun { get; set; }
        
        public int Adet { get; set; }
        
        public decimal BirimFiyat { get; set; }
        
        public decimal ToplamTutar { get; set; }
        
        public decimal KomisyonOrani { get; set; }
        
        public decimal KomisyonTutari { get; set; }
        
        public decimal SaticiKazanci { get; set; }
        
        public SaticiSiparisDurum Durum { get; set; } = SaticiSiparisDurum.YeniSiparis;
        
        [StringLength(50)]
        public string? KargoTakipNo { get; set; }
        
        public DateTime? KargoTarihi { get; set; }
        
        public DateTime? TeslimTarihi { get; set; }
        
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;
        
        [StringLength(500)]
        public string? Notlar { get; set; }
    }
    
    public enum SaticiSiparisDurum
    {
        YeniSiparis = 0,
        Hazirlaniyor = 1,
        KargoyaVerildi = 2,
        TeslimEdildi = 3,
        IptalEdildi = 4,
        IadeEdildi = 5
    }
}
