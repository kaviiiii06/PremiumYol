using System.Collections.Generic;
using System.Threading.Tasks;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface ISaticiSiparisService
    {
        Task<List<SaticiSiparisDto>> GetBySaticiAsync(int saticiId, SaticiSiparisDurum? durum = null);
        Task<SaticiSiparis?> GetByIdAsync(int id);
        Task<List<SaticiSiparisDto>> GetBekleyenlerAsync(int saticiId);
        Task<bool> KargoHazirlaAsync(int id, string kargoTakipNo);
        Task<bool> TeslimEdildiAsync(int id);
        Task<bool> IptalEtAsync(int id, string sebep);
        Task<SaticiSiparis> CreateAsync(int saticiId, int siparisId, int urunId, int adet, decimal birimFiyat, decimal komisyonOrani);
    }
}
