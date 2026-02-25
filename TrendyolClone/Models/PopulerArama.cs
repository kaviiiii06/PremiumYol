using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class PopulerArama
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string AramaTerimi { get; set; } = string.Empty;

        public int AramaSayisi { get; set; } = 0;

        public DateTime SonAramaTarihi { get; set; } = DateTime.Now;

        // Kategori bazlı popüler aramalar (opsiyonel)
        public int? KategoriId { get; set; }
        public virtual Kategori? Kategori { get; set; }

        // Trend hesaplama için
        public int GunlukAramaSayisi { get; set; } = 0;
        public int HaftalikAramaSayisi { get; set; } = 0;
        public int AylikAramaSayisi { get; set; } = 0;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime GuncellenmeTarihi { get; set; } = DateTime.Now;
    }
}
