using System;
using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class YorumResim
    {
        public int Id { get; set; }
        
        public int YorumId { get; set; }
        public UrunYorum? Yorum { get; set; }
        
        [Required]
        [StringLength(500)]
        public string ResimUrl { get; set; } = string.Empty;
        
        public int Sira { get; set; } = 0;
        
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
    }
}
