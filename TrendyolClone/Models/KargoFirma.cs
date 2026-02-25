using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class KargoFirma
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty; // Aras, Yurtiçi, MNG
        
        [StringLength(500)]
        public string? Logo { get; set; }
        
        // API bilgileri
        [StringLength(500)]
        public string? ApiUrl { get; set; }
        
        [StringLength(200)]
        public string? ApiKey { get; set; }
        
        [StringLength(200)]
        public string? ApiSecret { get; set; }
        
        // Fiyatlandırma
        [Required]
        public decimal TemelUcret { get; set; }
        
        [Required]
        public decimal KgBasinaUcret { get; set; }
        
        [Required]
        public decimal DesiBasinaUcret { get; set; }
        
        // Ücretsiz kargo limiti
        public decimal? UcretsizKargoLimiti { get; set; }
        
        public bool Aktif { get; set; } = true;
        
        public int Sira { get; set; } = 0;
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }
        
        // İlişkiler
        public List<KargoUcret> Ucretler { get; set; } = new List<KargoUcret>();
    }
}
