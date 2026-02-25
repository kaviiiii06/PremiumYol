using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface IAramaService
    {
        // Arama işlemleri
        Task<List<OtomatikTamamlamaDto>> OtomatikTamamlamaGetirAsync(string terim, int kullaniciId = 0);
        Task<AramaSonucDto> AramaYapAsync(string terim, int kullaniciId = 0, int sayfa = 1, int sayfaBoyutu = 20);
        
        // Arama geçmişi
        Task AramaGecmisiEkleAsync(int kullaniciId, string terim);
        Task<List<string>> AramaGecmisiGetirAsync(int kullaniciId, int adet = 10);
        Task AramaGecmisiTemizleAsync(int kullaniciId);
        
        // Popüler aramalar
        Task<List<string>> PopulerAramalarGetirAsync(int adet = 10);
        Task PopulerAramaGuncelleAsync(string terim);
        
        // Arama tıklama takibi
        Task AramaTiklamaKaydetAsync(string terim, int urunId, int kullaniciId = 0);
        
        // Arama önerileri
        Task<List<string>> AramaOnerileriGetirAsync(int kullaniciId, int adet = 5);
    }
}
