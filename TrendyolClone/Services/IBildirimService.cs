using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public interface IBildirimService
    {
        // Bildirim Gönderme
        Task<bool> EmailGonderAsync(int? kullaniciId, string email, string baslik, string icerik, string? sablonKodu = null);
        Task<bool> SmsGonderAsync(int? kullaniciId, string telefonNo, string mesaj, string? sablonKodu = null);
        Task<bool> SablonIleBildirimGonderAsync(string sablonKodu, int kullaniciId, Dictionary<string, string> parametreler);
        
        // Toplu Bildirim
        Task<int> TopluEmailGonderAsync(List<int> kullaniciIdler, string baslik, string icerik);
        Task<int> TumKullanicilariaBildirimGonderAsync(string baslik, string icerik, BildirimTuru tur);
        
        // Bildirim Yönetimi
        Task<List<Bildirim>> BekleyenBildirimleriGetirAsync();
        Task<bool> BildirimGonderAsync(int bildirimId);
        Task<bool> BildirimIptalEtAsync(int bildirimId);
        Task BildirimleriIsleAsync(); // Background job için
        
        // Şablon Yönetimi
        Task<BildirimSablonu?> SablonGetirAsync(string kod);
        Task<List<BildirimSablonu>> TumSablonlariGetirAsync();
        Task<bool> SablonOlusturAsync(BildirimSablonu sablon);
        Task<bool> SablonGuncelleAsync(BildirimSablonu sablon);
        
        // Kullanıcı Tercihleri
        Task<BildirimTercihi?> TercihleriGetirAsync(int kullaniciId);
        Task<bool> TercihleriGuncelleAsync(BildirimTercihi tercih);
        Task<bool> BildirimGonderilsinMiAsync(int kullaniciId, string bildirimTipi);
        
        // Raporlama
        Task<Dictionary<string, int>> BildirimIstatistikleriGetirAsync(DateTime baslangic, DateTime bitis);
        Task<List<Bildirim>> KullaniciBildirimGecmisiAsync(int kullaniciId, int sayfa = 1, int sayfaBoyutu = 20);
    }
}
