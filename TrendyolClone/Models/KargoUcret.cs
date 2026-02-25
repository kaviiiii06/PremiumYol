using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class KargoUcret
    {
        public int Id { get; set; }
        
        [Required]
        public int KargoFirmaId { get; set; }
        public KargoFirma KargoFirma { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string CikisIli { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string VarisIli { get; set; } = string.Empty;
        
        [Required]
        public decimal Ucret { get; set; }
        
        [Required]
        public int TahminiGun { get; set; }
        
        public DateTime GuncellenmeTarihi { get; set; } = DateTime.Now;
    }
}
