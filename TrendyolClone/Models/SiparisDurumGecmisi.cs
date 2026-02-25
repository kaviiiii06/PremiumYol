using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class SiparisDurumGecmisi
    {
        public int Id { get; set; }

        public int SiparisId { get; set; }
        public virtual Siparis Siparis { get; set; } = null!;

        public SiparisDurumu Durum { get; set; }

        [StringLength(500)]
        public string? Aciklama { get; set; }

        public DateTime Tarih { get; set; } = DateTime.Now;

        // Kim değiştirdi (Admin veya Sistem)
        public int? KullaniciId { get; set; }
        public virtual Kullanici? Kullanici { get; set; }

        [StringLength(100)]
        public string? DegistirenKisi { get; set; } // "Sistem", "Admin: Baran", vb.
    }
}
