using System;
using System.Collections.Generic;

namespace TrendyolClone.Models.DTOs
{
    public class SaticiDto
    {
        public int Id { get; set; }
        public string MagazaAdi { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string? Logo { get; set; }
        public decimal OrtalamaPuan { get; set; }
        public int DegerlendirmeSayisi { get; set; }
        public int ToplamSatis { get; set; }
        public SaticiDurum Durum { get; set; }
    }
    
    public class SaticiDashboardDto
    {
        public decimal BugunSatis { get; set; }
        public decimal BuHaftaSatis { get; set; }
        public decimal BuAySatis { get; set; }
        public int BekleyenSiparis { get; set; }
        public int HazirlaniyorSiparis { get; set; }
        public int KargodaSiparis { get; set; }
        public int DusukStokUrun { get; set; }
        public int TukenStokUrun { get; set; }
        public decimal BekleyenOdeme { get; set; }
        public decimal ToplamKazanc { get; set; }
        public List<GunlukSatisDto> SonYediGun { get; set; } = new();
        public List<PopulerUrunDto> PopulerUrunler { get; set; } = new();
        public List<SaticiSiparisDto> SonSiparisler { get; set; } = new();
    }
    
    public class GunlukSatisDto
    {
        public DateTime Tarih { get; set; }
        public decimal Tutar { get; set; }
        public int SiparisSayisi { get; set; }
    }
    
    public class PopulerUrunDto
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string? Resim { get; set; }
        public int SatisSayisi { get; set; }
        public decimal Tutar { get; set; }
    }
    
    public class SaticiSiparisDto
    {
        public int Id { get; set; }
        public string SiparisNo { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public string? UrunResmi { get; set; }
        public int Adet { get; set; }
        public decimal ToplamTutar { get; set; }
        public decimal SaticiKazanci { get; set; }
        public SaticiSiparisDurum Durum { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public string MusteriAdi { get; set; } = string.Empty;
        public string? KargoTakipNo { get; set; }
    }
    
    public class SaticiFinansDto
    {
        public decimal ToplamSatis { get; set; }
        public decimal ToplamKomisyon { get; set; }
        public decimal NetKazanc { get; set; }
        public decimal BekleyenOdeme { get; set; }
        public decimal OdenenTutar { get; set; }
        public List<AylikFinansDto> AylikRapor { get; set; } = new();
        public List<SaticiOdemeDto> SonOdemeler { get; set; } = new();
    }
    
    public class AylikFinansDto
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public decimal Satis { get; set; }
        public decimal Komisyon { get; set; }
        public decimal Net { get; set; }
        public int SiparisSayisi { get; set; }
    }
    
    public class SaticiOdemeDto
    {
        public int Id { get; set; }
        public decimal Tutar { get; set; }
        public decimal KomisyonTutari { get; set; }
        public decimal NetTutar { get; set; }
        public int Donem { get; set; }
        public DateTime? OdemeTarihi { get; set; }
        public SaticiOdemeDurum Durum { get; set; }
        public string? Aciklama { get; set; }
    }
    
    public class SaticiUrunDto
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string? Resim { get; set; }
        public int Stok { get; set; }
        public decimal Fiyat { get; set; }
        public decimal? IndirimliFiyat { get; set; }
        public SaticiUrunDurum Durum { get; set; }
        public int GoruntulemeSayisi { get; set; }
        public int SatisSayisi { get; set; }
        public DateTime EklenmeTarihi { get; set; }
    }
}
