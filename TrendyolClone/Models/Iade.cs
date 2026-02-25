using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Iade
    {
        public int Id { get; set; }

        public int SiparisId { get; set; }
        public virtual Siparis Siparis { get; set; } = null!;

        public int KullaniciId { get; set; }
        public virtual Kullanici Kullanici { get; set; } = null!;

        // İade Bilgileri
        public IadeNedeni Neden { get; set; }

        [StringLength(1000)]
        public string? Aciklama { get; set; }

        public IadeDurumu Durum { get; set; } = IadeDurumu.TalepEdildi;

        // İade Ürünleri
        public virtual List<IadeUrunu> Urunler { get; set; } = new List<IadeUrunu>();

        // Kargo
        [StringLength(50)]
        public string? IadeKargoTakipNo { get; set; }

        public DateTime? IadeKargoTarihi { get; set; }

        // Ödeme
        public decimal IadeTutari { get; set; }
        public bool OdemeYapildiMi { get; set; } = false;
        public DateTime? OdemeTarihi { get; set; }

        [StringLength(500)]
        public string? RedNedeni { get; set; }

        // Tarihler
        public DateTime TalepTarihi { get; set; } = DateTime.Now;
        public DateTime? OnayTarihi { get; set; }
        public DateTime? TamamlanmaTarihi { get; set; }
    }

    public class IadeUrunu
    {
        public int Id { get; set; }

        public int IadeId { get; set; }
        public virtual Iade Iade { get; set; } = null!;

        public int UrunId { get; set; }
        public virtual Urun Urun { get; set; } = null!;

        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal ToplamFiyat => Adet * BirimFiyat;
    }

    public enum IadeNedeni
    {
        UrunHatali = 1,
        YanlisSiparis = 2,
        GecikmeTeslim = 3,
        BeklentiyiKarsilamiyor = 4,
        FikrimiDegistirdim = 5,
        EksikUrun = 6,
        HasarliUrun = 7,
        Diger = 99
    }

    public enum IadeDurumu
    {
        TalepEdildi = 0,
        OnayBekliyor = 1,
        Onaylandi = 2,
        Reddedildi = 3,
        KargoGonderildi = 4,
        KargoTeslimAlindi = 5,
        OdemeYapildi = 6,
        Tamamlandi = 7
    }
}
