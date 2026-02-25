using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Tedarikci
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Ad { get; set; }
        
        [StringLength(200)]
        public string Aciklama { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Tip { get; set; } // AliExpress, CJ Dropshipping, DHgate, etc.
        
        [StringLength(500)]
        public string ApiUrl { get; set; }
        
        [StringLength(200)]
        public string ApiAnahtari { get; set; }
        
        [StringLength(200)]
        public string ApiGizliAnahtari { get; set; }
        
        [StringLength(100)]
        public string IletisimEmail { get; set; }
        
        [StringLength(20)]
        public string IletisimTelefon { get; set; }
        
        public decimal KomisyonOrani { get; set; } // Komisyon oranı %
        
        public int TeslimatGunu { get; set; } // Teslimat süresi (gün)
        
        public bool Aktif { get; set; } = true;
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        
        public virtual ICollection<Urun> Urunler { get; set; } = new List<Urun>();
    }
}
