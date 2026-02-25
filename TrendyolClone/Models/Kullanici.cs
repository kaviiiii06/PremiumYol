#nullable enable
using System.ComponentModel.DataAnnotations;

namespace TrendyolClone.Models
{
    public class Kullanici
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Ad gerekli")]
        [StringLength(50)]
        public string Ad { get; set; }
        
        [Required(ErrorMessage = "Soyad gerekli")]
        [StringLength(50)]
        public string Soyad { get; set; }
        
        [Required(ErrorMessage = "Kullanıcı adı gerekli")]
        [StringLength(50, MinimumLength = 3)]
        public string KullaniciAdi { get; set; }
        
        [Required(ErrorMessage = "Email gerekli")]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Şifre gerekli")]
        [StringLength(100, MinimumLength = 6)]
        public string Sifre { get; set; }
        
        [Phone]
        public string? TelefonNumarasi { get; set; }
        
        [StringLength(500)]
        public string? ProfilFotoUrl { get; set; }
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public bool Aktif { get; set; } = true;
        
        public int? RolId { get; set; }
        public virtual Rol? Rol { get; set; }
        public ICollection<Siparis>? Siparisler { get; set; }
    }
}
#nullable restore
