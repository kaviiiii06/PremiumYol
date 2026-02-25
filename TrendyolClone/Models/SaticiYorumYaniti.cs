using System;
using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class SaticiYorumYaniti
    {
        public int Id { get; set; }
        
        public int YorumId { get; set; }
        public UrunYorum? Yorum { get; set; }
        
        public int SaticiId { get; set; }
        public Satici? Satici { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Yanit { get; set; } = string.Empty;
        
        public DateTime Tarih { get; set; } = DateTime.Now;
        
        public DateTime? GuncellenmeTarihi { get; set; }
    }
}
