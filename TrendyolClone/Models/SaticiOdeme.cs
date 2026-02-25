using System;
using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class SaticiOdeme
    {
        public int Id { get; set; }
        
        public int SaticiId { get; set; }
        public Satici? Satici { get; set; }
        
        public decimal Tutar { get; set; }
        
        public decimal KomisyonTutari { get; set; }
        
        public decimal NetTutar { get; set; }
        
        public int Donem { get; set; } // YYYYMM formatında
        
        public DateTime? OdemeTarihi { get; set; }
        
        public OdemeTipi OdemeTipi { get; set; } = OdemeTipi.Havale;
        
        public SaticiOdemeDurum Durum { get; set; } = SaticiOdemeDurum.Beklemede;
        
        [StringLength(500)]
        public string? Aciklama { get; set; }
        
        [StringLength(100)]
        public string? ReferansNo { get; set; }
        
        public DateTime TalepTarihi { get; set; } = DateTime.Now;
        
        public int? OnaylayanYoneticiId { get; set; }
        
        public DateTime? OnayTarihi { get; set; }
    }
    
    public enum OdemeTipi
    {
        Havale = 0,
        EFT = 1,
        Cek = 2
    }
    
    public enum SaticiOdemeDurum
    {
        Beklemede = 0,
        Onaylandi = 1,
        Odendi = 2,
        Reddedildi = 3
    }
}
