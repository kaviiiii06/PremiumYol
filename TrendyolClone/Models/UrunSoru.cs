using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class UrunSoru
    {
        public int Id { get; set; }
        
        public int UrunId { get; set; }
        public virtual Urun Urun { get; set; }
        
        public int KullaniciId { get; set; }
        public virtual Kullanici Kullanici { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Soru { get; set; }
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        
        public bool Cevaplandi { get; set; } = false;
        
        public virtual ICollection<UrunCevap> Cevaplar { get; set; }
    }
    
    public class UrunCevap
    {
        public int Id { get; set; }
        
        public int SoruId { get; set; }
        public virtual UrunSoru Soru { get; set; }
        
        public int? KullaniciId { get; set; }
        public virtual Kullanici Kullanici { get; set; }
        
        public int? YoneticiId { get; set; }
        public virtual Yonetici Yonetici { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Cevap { get; set; }
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        
        public bool Resmi { get; set; } = false; // Admin/Satıcı cevabı mı?
    }
}
