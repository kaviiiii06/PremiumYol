using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface ISaticiFinansService
    {
        Task<SaticiFinansDto> GetFinansRaporuAsync(int saticiId, DateTime? baslangic = null, DateTime? bitis = null);
        Task<decimal> GetToplamKazancAsync(int saticiId);
        Task<decimal> GetBekleyenOdemeAsync(int saticiId);
        Task<List<SaticiOdemeDto>> GetOdemelerAsync(int saticiId);
        Task<bool> OdemeTalebiOlusturAsync(int saticiId, int donem);
        Task<bool> OdemeOnaylaAsync(int odemeId, int yoneticiId);
        Task<bool> OdemeYapAsync(int odemeId, string referansNo);
    }
}
