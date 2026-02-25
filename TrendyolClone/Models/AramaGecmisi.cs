using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class AramaGecmisi
    {
        public int Id { get; set; }

        // Kullanıcı bilgisi (opsiyonel - misafir kullanıcılar için)
        public int? KullaniciId { get; set; }
        public virtual Kullanici? Kullanici { get; set; }

        [StringLength(100)]
        public string? SessionId { get; set; }

        [Required]
        [StringLength(200)]
        public string AramaTerimi { get; set; } = string.Empty;

        public int SonucSayisi { get; set; }

        // Kategori filtresi (varsa)
        public int? KategoriId { get; set; }
        public virtual Kategori? Kategori { get; set; }

        public DateTime AramaTarihi { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? IpAdresi { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        // Navigation
        public virtual List<AramaTiklama> Tiklamalar { get; set; } = new List<AramaTiklama>();
    }
}
