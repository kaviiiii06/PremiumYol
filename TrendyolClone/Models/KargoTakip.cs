using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class KargoTakip
    {
        public int Id { get; set; }

        public int SiparisId { get; set; }
        public virtual Siparis Siparis { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string KargoFirmasi { get; set; } = string.Empty; // Aras, Yurtiçi, MNG, PTT

        [Required]
        [StringLength(50)]
        public string TakipNo { get; set; } = string.Empty;

        [StringLength(100)]
        public string? KargoDurumu { get; set; }

        public DateTime? TahminiTeslimatTarihi { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }

        // Navigation
        public virtual List<KargoHareket> Hareketler { get; set; } = new List<KargoHareket>();
    }

    public class KargoHareket
    {
        public int Id { get; set; }

        public int KargoTakipId { get; set; }
        public virtual KargoTakip KargoTakip { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Durum { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Aciklama { get; set; }

        [StringLength(200)]
        public string? Lokasyon { get; set; }

        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}
