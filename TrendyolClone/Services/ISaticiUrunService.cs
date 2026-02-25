using System.Collections.Generic;
using System.Threading.Tasks;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface ISaticiUrunService
    {
        Task<List<SaticiUrunDto>> GetBySaticiAsync(int saticiId, SaticiUrunDurum? durum = null);
        Task<SaticiUrun?> GetByIdAsync(int id);
        Task<SaticiUrun> CreateAsync(SaticiUrun urun);
        Task UpdateAsync(SaticiUrun urun);
        Task DeleteAsync(int id);
        Task<bool> StokGuncelleAsync(int id, int stok);
        Task<bool> FiyatGuncelleAsync(int id, decimal fiyat, decimal? indirimliFiyat);
        Task<bool> DurumGuncelleAsync(int id, SaticiUrunDurum durum);
        Task<bool> OnayaGonderAsync(int id);
        Task<bool> YayindanKaldirAsync(int id);
        Task<bool> GoruntulemeSayisiArtirAsync(int id);
        Task<bool> SatisKaydetAsync(int id, int adet, decimal tutar);
    }
}
