using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Fatura
    {
        public int Id { get; set; }

        public int SiparisId { get; set; }
        public virtual Siparis Siparis { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string FaturaNo { get; set; } = string.Empty; // FTR-2024-00001

        public DateTime FaturaTarihi { get; set; } = DateTime.Now;

        // Fatura Bilgileri
        [StringLength(200)]
        public string? FirmaUnvani { get; set; }

        [StringLength(100)]
        public string? VergiDairesi { get; set; }

        [StringLength(20)]
        public string? VergiNo { get; set; }

        [StringLength(500)]
        public string? Adres { get; set; }

        // Tutar Bilgileri
        public decimal AraToplam { get; set; }
        public decimal KDV { get; set; }
        public decimal KargoUcreti { get; set; }
        public decimal IndirimTutari { get; set; }
        public decimal Toplam { get; set; }

        // PDF
        [StringLength(500)]
        public string? PdfUrl { get; set; }

        public bool EFaturaMi { get; set; } = false;

        [StringLength(100)]
        public string? EFaturaUuid { get; set; }
    }
}
