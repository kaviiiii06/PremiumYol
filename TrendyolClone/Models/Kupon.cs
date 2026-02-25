using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Kupon
    {
        public int Id { get; set; }
        
        // Kupon bilgileri
        [Required]
        [StringLength(50)]
        public string Kod { get; set; } = string.Empty; // YILBASI2024
        
        [Required]
        [StringLength(500)]
        public string Aciklama { get; set; } = string.Empty;
        
        // İndirim türü
        [Required]
        public IndirimTuru IndirimTuru { get; set; }
        
        [Required]
        public decimal IndirimMiktari { get; set; }
        
        public decimal? MaksimumIndirim { get; set; } // %20 max 100TL gibi
        
        // Kullanım koşulları
        public decimal? MinimumSepetTutari { get; set; }
        
        public int? MaksimumKullanimSayisi { get; set; }
        
        public int KullanimSayisi { get; set; } = 0;
        
        public int? KullaniciBasinaKullanimSayisi { get; set; }
        
        // Geçerlilik
        [Required]
        public DateTime BaslangicTarihi { get; set; }
        
        [Required]
        public DateTime BitisTarihi { get; set; }
        
        public bool Aktif { get; set; } = true;
        
        // İlişkiler
        public List<KuponKullanimi> Kullanimlar { get; set; } = new List<KuponKullanimi>();
        public List<KuponKategori> GecerliKategoriler { get; set; } = new List<KuponKategori>();
        
        // Hesaplanan özellikler
        public bool GecerliMi => Aktif && 
                                 DateTime.Now >= BaslangicTarihi && 
                                 DateTime.Now <= BitisTarihi &&
                                 (!MaksimumKullanimSayisi.HasValue || KullanimSayisi < MaksimumKullanimSayisi.Value);
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }
    }
}
