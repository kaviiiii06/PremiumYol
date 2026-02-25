using System;
using System.Collections.Generic;

namespace TrendyolClone.Models.DTOs
{
    public class YorumDto
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string? UrunResim { get; set; }
        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public bool OnayliAlici { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string YorumMetni { get; set; } = string.Empty;
        public int GenelPuan { get; set; }
        public int UrunKalitesiPuan { get; set; }
        public int FiyatPerformansPuan { get; set; }
        public int KargoHiziPuan { get; set; }
        public int PaketlemePuan { get; set; }
        public decimal OrtalamaPuan { get; set; }
        public bool OnaylandiMi { get; set; }
        public YorumDurum Durum { get; set; }
        public int YardimciBuldum { get; set; }
        public int YardimciBulmadim { get; set; }
        public int SikayetSayisi { get; set; }
        public DateTime Tarih { get; set; }
        public List<string> Resimler { get; set; } = new();
        public List<string> Videolar { get; set; } = new();
        public SaticiYanitDto? SaticiYaniti { get; set; }
        public bool KullaniciYardimciBuldu { get; set; }
        public bool KullaniciYardimciBulmadi { get; set; }
    }
    
    public class SaticiYanitDto
    {
        public int Id { get; set; }
        public string MagazaAdi { get; set; } = string.Empty;
        public string Yanit { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
    }
    
    public class YorumIstatistik
    {
        public int ToplamYorum { get; set; }
        public decimal OrtalamaPuan { get; set; }
        public int BesPuan { get; set; }
        public int DortPuan { get; set; }
        public int UcPuan { get; set; }
        public int IkiPuan { get; set; }
        public int BirPuan { get; set; }
        public int FotografliYorum { get; set; }
        public int OnayliAliciYorum { get; set; }
        public decimal BesPuanYuzde { get; set; }
        public decimal DortPuanYuzde { get; set; }
        public decimal UcPuanYuzde { get; set; }
        public decimal IkiPuanYuzde { get; set; }
        public decimal BirPuanYuzde { get; set; }
    }
    
    public class YorumFiltre
    {
        public int? Puan { get; set; }
        public bool? FotografliMi { get; set; }
        public bool? OnayliAliciMi { get; set; }
        public YorumSiralama Siralama { get; set; } = YorumSiralama.EnYeni;
        public int Sayfa { get; set; } = 1;
        public int SayfaBoyutu { get; set; } = 10;
    }
    
    public enum YorumSiralama
    {
        EnYeni = 0,
        EnEski = 1,
        EnYararli = 2,
        EnYuksekPuan = 3,
        EnDusukPuan = 4
    }
    
    public class YorumOlusturDto
    {
        public int UrunId { get; set; }
        public int SiparisId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string YorumMetni { get; set; } = string.Empty;
        public int GenelPuan { get; set; }
        public int UrunKalitesiPuan { get; set; }
        public int FiyatPerformansPuan { get; set; }
        public int KargoHiziPuan { get; set; }
        public int PaketlemePuan { get; set; }
        public List<string> ResimUrls { get; set; } = new();
        public string? VideoUrl { get; set; }
    }
    
    public class YorumRaporDto
    {
        public int Id { get; set; }
        public int YorumId { get; set; }
        public string YorumBaslik { get; set; } = string.Empty;
        public string YorumMetni { get; set; } = string.Empty;
        public string RaporEdenKullanici { get; set; } = string.Empty;
        public string Sebep { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public RaporDurum Durum { get; set; }
        public DateTime Tarih { get; set; }
    }

    // Yorum listesi için DTO
    public class YorumListeDto
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public string? UrunAdi { get; set; }
        public string? KullaniciAdi { get; set; }
        public string? Yorum { get; set; }
        public int GenelPuan { get; set; }
        public int UrunKalitesiPuan { get; set; }
        public int FiyatPerformansPuan { get; set; }
        public bool OnaylandiMi { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public int FaydaliSayisi { get; set; }
        public int FaydasizSayisi { get; set; }
        public List<string>? Resimler { get; set; }
        public string? SaticiYaniti { get; set; }
        public DateTime? SaticiYanitTarihi { get; set; }
    }

    // Yorum ekleme için DTO
    public class YorumEkleDto
    {
        public int UrunId { get; set; }
        public int SiparisId { get; set; }
        public string? KullaniciId { get; set; }
        public string? Yorum { get; set; }
        public int GenelPuan { get; set; }
        public int UrunKalitesiPuan { get; set; }
        public int FiyatPerformansPuan { get; set; }
        public bool TavsiyeEderMi { get; set; }
        public string? VideoUrl { get; set; }
    }
}
