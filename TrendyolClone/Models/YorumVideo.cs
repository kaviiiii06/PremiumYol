using System;
using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class YorumVideo
    {
        public int Id { get; set; }
        
        public int YorumId { get; set; }
        public UrunYorum? Yorum { get; set; }
        
        [Required]
        [StringLength(500)]
        public string VideoUrl { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Thumbnail { get; set; }
        
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
    }
}
