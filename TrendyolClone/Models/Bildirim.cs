using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Bildirim
    {
        public int Id { get; set; }

        public int? KullaniciId { get; set; }
        public virtual Kullanici? Kullanici { get; set; }

        public BildirimTuru Turu { get; set; }

        [Required]
        [StringLength(200)]
        public string Baslik { get; set; } = string.Empty;

        [Required]
        public string Icerik { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? TelefonNo { get; set; }

        public BildirimDurumu Durum { get; set; } = BildirimDurumu.Beklemede;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GonderimTarihi { get; set; }

        [StringLength(500)]
        public string? HataMesaji { get; set; }

        // Ek bilgiler
        [StringLength(50)]
        public string? SablonKodu { get; set; }

        public string? Parametreler { get; set; } // JSON format
    }

    public class BildirimSablonu
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Kod { get; set; } = string.Empty; // SIPARIS_ONAY, KARGO_BILGI, vb.

        [Required]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty;

        public BildirimTuru Turu { get; set; }

        [Required]
        [StringLength(200)]
        public string Konu { get; set; } = string.Empty;

        [Required]
        public string Icerik { get; set; } = string.Empty; // HTML veya Text

        public bool AktifMi { get; set; } = true;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }

        [StringLength(1000)]
        public string? Aciklama { get; set; }
    }

    public class BildirimTercihi
    {
        public int Id { get; set; }

        public int KullaniciId { get; set; }
        public virtual Kullanici Kullanici { get; set; } = null!;

        public bool EmailBildirimleri { get; set; } = true;
        public bool SmsBildirimleri { get; set; } = false;
        public bool SiparisBildirimleri { get; set; } = true;
        public bool KampanyaBildirimleri { get; set; } = true;
        public bool UrunBildirimleri { get; set; } = false;
        public bool IadeBildirimleri { get; set; } = true;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }
    }

    public enum BildirimTuru
    {
        Email = 1,
        SMS = 2,
        Push = 3,
        InApp = 4
    }

    public enum BildirimDurumu
    {
        Beklemede = 0,
        Gonderiliyor = 1,
        Gonderildi = 2,
        Basarili = 3,
        Basarisiz = 4,
        IptalEdildi = 5
    }
}
