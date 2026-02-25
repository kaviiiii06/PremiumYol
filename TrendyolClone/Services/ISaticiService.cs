using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface ISaticiService
    {
        Task<Satici?> GetByIdAsync(int id);
        Task<Satici?> GetByKullaniciIdAsync(int kullaniciId);
        Task<Satici?> GetBySlugAsync(string slug);
        Task<List<SaticiDto>> GetAllAsync();
        Task<List<SaticiDto>> GetOnaylananlarAsync();
        Task<Satici> CreateAsync(Satici satici);
        Task UpdateAsync(Satici satici);
        Task<bool> OnaylaAsync(int id);
        Task<bool> ReddetAsync(int id, string sebep);
        Task<bool> AskiyaAlAsync(int id, string sebep);
        Task<SaticiDashboardDto> GetDashboardAsync(int saticiId);
        Task<bool> PuanGuncelleAsync(int saticiId);
    }
}
