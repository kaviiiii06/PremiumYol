using System;

namespace TrendyolClone.Models
{
    public class YorumEtkilesim
    {
        public int Id { get; set; }
        
        public int YorumId { get; set; }
        public UrunYorum? Yorum { get; set; }
        
        public int KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }
        
        public EtkilesimTip Tip { get; set; }
        
        public DateTime Tarih { get; set; } = DateTime.Now;
    }
    
    public enum EtkilesimTip
    {
        YardimciBuldum = 0,
        YardimciBulmadim = 1,
        Sikayet = 2
    }
}
