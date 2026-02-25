using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface IGelismisSiparisService
    {
        // Sipariş Detay
        Task<SiparisDetayDto?> SiparisDetayGetirAsync(int siparisId, int kullaniciId);
        Task<List<SiparisDetayDto>> KullaniciSiparisleriniGetirAsync(int kullaniciId);
        
        // Durum Yönetimi
        Task SiparisDurumuGuncelleAsync(int siparisId, SiparisDurumu yeniDurum, string? aciklama = null, string? degistirenKisi = null);
        Task<List<SiparisDurumDto>> DurumGecmisiGetirAsync(int siparisId);
        
        // Kargo Takip
        Task KargoTakipEkleAsync(int siparisId, string kargoFirmasi, string takipNo);
        Task<KargoTakipDto?> KargoTakipGetirAsync(int siparisId);
        Task KargoHareketEkleAsync(int siparisId, string durum, string? aciklama = null, string? lokasyon = null);
        
        // Fatura
        Task<Fatura> FaturaOlusturAsync(int siparisId);
        Task<Fatura?> FaturaGetirAsync(int siparisId);
        
        // İade
        Task<Iade> IadeTalebiOlusturAsync(IadeTalebiDto dto, int kullaniciId);
        Task IadeTalebiOnaylaAsync(int iadeId, string? aciklama = null);
        Task IadeTalebiReddetAsync(int iadeId, string redNedeni);
        Task<Iade?> IadeGetirAsync(int iadeId);
        Task<List<Iade>> KullaniciIadeleriniGetirAsync(int kullaniciId);
        
        // İptal
        Task SiparisIptalEtAsync(int siparisId, string neden);
    }
}
