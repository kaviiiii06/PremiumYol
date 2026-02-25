using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public interface IKuponService
    {
        // Kupon sorgulama
        Task<Kupon?> GetByKodAsync(string kod);
        Task<Kupon?> GetByIdAsync(int id);
        Task<List<Kupon>> GetAktifKuponlarAsync();
        Task<List<Kupon>> GetKullaniciKuponlariAsync(int kullaniciId);
        
        // Kupon doğrulama
        Task<bool> KuponGecerliMi(string kod, int kullaniciId, decimal sepetTutari, List<int>? kategoriIdleri = null);
        Task<string?> KuponHataMesaji(string kod, int kullaniciId, decimal sepetTutari, List<int>? kategoriIdleri = null);
        
        // İndirim hesaplama
        Task<decimal> HesaplaIndirimiAsync(string kod, decimal sepetTutari);
        Task<decimal> HesaplaIndirimiAsync(Kupon kupon, decimal sepetTutari);
        
        // Kupon kullanımı
        Task<KuponKullanimi> KuponKullaniminiKaydetAsync(int kuponId, int kullaniciId, int? siparisId, decimal indirimTutari, decimal sepetTutari, string? ipAdresi = null);
        Task<int> GetKullaniciKullanimSayisiAsync(int kuponId, int kullaniciId);
        
        // Admin işlemleri
        Task<Kupon> OlusturAsync(Kupon kupon);
        Task<Kupon> GuncelleAsync(Kupon kupon);
        Task<bool> SilAsync(int id);
        Task<bool> AktiflikDurumunuDegistirAsync(int id, bool aktif);
        
        // İstatistikler
        Task<int> ToplamKullanimSayisiAsync(int kuponId);
        Task<decimal> ToplamIndirimTutariAsync(int kuponId);
        Task<List<KuponKullanimi>> GetKullanimGecmisiAsync(int kuponId, int? limit = null);
    }
}
