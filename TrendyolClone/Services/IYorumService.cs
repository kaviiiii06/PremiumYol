using System.Collections.Generic;
using System.Threading.Tasks;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public interface IYorumService
    {
        Task<List<YorumDto>> GetByUrunAsync(int urunId, YorumFiltre filtre, int? kullaniciId = null);
        Task<YorumDto?> GetByIdAsync(int id, int? kullaniciId = null);
        Task<UrunYorum> CreateAsync(YorumOlusturDto dto, int kullaniciId);
        Task UpdateAsync(int id, YorumOlusturDto dto);
        Task DeleteAsync(int id);
        Task<bool> OnaylaAsync(int id);
        Task<bool> ReddetAsync(int id, string sebep);
        Task<YorumIstatistik> GetIstatistikAsync(int urunId);
        Task<bool> YardimciBuldum(int yorumId, int kullaniciId);
        Task<bool> YardimciBulmadim(int yorumId, int kullaniciId);
        Task<bool> SikayetEt(int yorumId, int kullaniciId, string sebep, string? aciklama = null);
        Task<bool> KullaniciYorumYapabilirMi(int kullaniciId, int urunId);
        Task<List<YorumDto>> GetKullaniciYorumlariAsync(int kullaniciId);
        
        // Yeni metodlar
        Task<List<YorumListeDto>> UrunYorumlariniGetirAsync(int urunId, string siralama, int sayfa, int sayfaBoyutu);
        Task<bool> YorumYapilabilirMiAsync(int urunId, string kullaniciId);
        Task<bool> YorumEkleAsync(YorumEkleDto dto);
        Task YorumFaydaliIsaretle(int yorumId, string kullaniciId, bool faydali);
        Task YorumRaporEtAsync(int yorumId, string kullaniciId, string sebep, string aciklama);
        Task<List<YorumListeDto>> KullaniciYorumlariniGetirAsync(string kullaniciId, int sayfa, int sayfaBoyutu);
        Task<bool> YorumSilAsync(int yorumId, string kullaniciId);
        Task<List<YorumListeDto>> AdminYorumListesiAsync(string durum, int sayfa, int sayfaBoyutu);
        Task YorumOnaylaAsync(int yorumId);
        Task YorumReddetAsync(int yorumId, string sebep);
        Task AdminYorumSilAsync(int yorumId);
        Task<List<YorumRaporDto>> YorumRaporlariniGetirAsync(string durum, int sayfa, int sayfaBoyutu);
        Task RaporIsleAsync(int raporId, string karar, string aciklama);
        Task<object> YorumIstatistikleriAsync();
    }
}
