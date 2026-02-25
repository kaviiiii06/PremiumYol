using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface IRaporService
    {
        Task<GenelRaporOzetiDto> GetGenelOzetAsync(RaporFiltre filtre);
        Task<List<SatisRaporuDto>> GetSatisRaporuAsync(RaporFiltre filtre);
        Task<List<UrunSatisRaporuDto>> GetUrunSatisRaporuAsync(RaporFiltre filtre);
        Task<List<KategoriSatisRaporuDto>> GetKategoriSatisRaporuAsync(RaporFiltre filtre);
        Task<List<KullaniciRaporuDto>> GetKullaniciRaporuAsync(RaporFiltre filtre);
        Task<List<FinansalRaporDto>> GetFinansalRaporAsync(RaporFiltre filtre);
    }
}
