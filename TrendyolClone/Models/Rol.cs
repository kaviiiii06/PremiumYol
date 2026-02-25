using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Rol
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Ad { get; set; }
        
        [StringLength(200)]
        public string Aciklama { get; set; }
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        
        public bool Aktif { get; set; } = true;
        
        public virtual ICollection<Kullanici> Kullanicilar { get; set; } = new List<Kullanici>();
    }
}
