using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface IAdminOrderService
    {
        Task<SiparisListeResult> GetSiparislerAsync(SiparisFiltre filtre);
        Task<SiparisDetayYonetimDto?> GetSiparisDetayAsync(int siparisId);
        Task<bool> DurumGuncelleAsync(SiparisDurumGuncelleDto dto, int adminId);
        Task<bool> KargoBilgisiEkleAsync(int siparisId, string kargoFirmasi, string takipNo);
        Task<bool> AdminNotuEkleAsync(int siparisId, string not);
        Task<bool> SiparisIptalEtAsync(int siparisId, string iptalNedeni, int adminId);
        Task<Dictionary<SiparisDurumu, int>> GetDurumIstatistikleriAsync();
        Task<List<SiparisYonetimDto>> GetBekleyenSiparislerAsync();
    }
}
