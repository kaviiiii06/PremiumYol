using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();
        Task<DashboardIstatistikler> GetIstatistiklerAsync();
        Task<List<GunlukSatis>> GetGunlukSatislarAsync(int gunSayisi = 30);
        Task<List<PopulerUrun>> GetPopulerUrunlerAsync(int adet = 10);
        Task<List<SonSiparis>> GetSonSiparisleriAsync(int adet = 10);
        Task<List<StokUyari>> GetStokUyarilariAsync();
        Task<List<YeniKullanici>> GetYeniKullanicilariAsync(int gunSayisi = 7);
    }
}
