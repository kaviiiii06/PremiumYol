using TrendyolClone.Models;

namespace TrendyolClone.Models.ViewModels
{
    public class UrunDetayViewModel
    {
        public Urun Urun { get; set; } = null!;
        public List<UrunVaryasyon> Varyasyonlar { get; set; } = new List<UrunVaryasyon>();
        public List<UrunResim> Resimler { get; set; } = new List<UrunResim>();
        public List<UrunOzellik> Ozellikler { get; set; } = new List<UrunOzellik>();
        public KargoOlculeri? KargoOlculeri { get; set; }
        public UrunVaryasyon? SecilenVaryasyon { get; set; }
        
        // Hesaplanan özellikler
        public bool VaryasyonVarMi => Varyasyonlar.Any();
        public bool ResimVarMi => Resimler.Any();
        public bool OzellikVarMi => Ozellikler.Any();
        public decimal GecerliFiyat => SecilenVaryasyon?.GecerliFiyat ?? Urun.IndirimliFiyat ?? Urun.Fiyat;
        public bool StokVarMi => (SecilenVaryasyon?.Stok ?? Urun.Stok) > 0;
        public int MevcutStok => SecilenVaryasyon?.Stok ?? Urun.Stok;
        
        // Renk ve beden seçenekleri
        public List<string> RenkSecenekleri => Varyasyonlar
            .Where(v => !string.IsNullOrEmpty(v.Renk))
            .Select(v => v.Renk!)
            .Distinct()
            .ToList();
            
        public List<string> BedenSecenekleri => Varyasyonlar
            .Where(v => !string.IsNullOrEmpty(v.Beden))
            .Select(v => v.Beden!)
            .Distinct()
            .ToList();
    }
}
