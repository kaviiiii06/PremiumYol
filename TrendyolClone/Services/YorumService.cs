using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public class YorumService : IYorumService
    {
        private readonly UygulamaDbContext _context;
        
        public YorumService(UygulamaDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<YorumDto>> GetByUrunAsync(int urunId, YorumFiltre filtre, int? kullaniciId = null)
        {
            var query = _context.UrunYorumlari
                .Where(y => y.UrunId == urunId && y.OnaylandiMi);
            
            // Filtreleme
            if (filtre.Puan.HasValue)
            {
                query = query.Where(y => y.GenelPuan == filtre.Puan.Value);
            }
            
            if (filtre.FotografliMi == true)
            {
                query = query.Where(y => y.Resimler.Any());
            }
            
            if (filtre.OnayliAliciMi == true)
            {
                query = query.Where(y => y.SiparisId != null);
            }
            
            // Sıralama
            query = filtre.Siralama switch
            {
                YorumSiralama.EnYeni => query.OrderByDescending(y => y.Tarih),
                YorumSiralama.EnEski => query.OrderBy(y => y.Tarih),
                YorumSiralama.EnYararli => query.OrderByDescending(y => y.YardimciBuldum),
                YorumSiralama.EnYuksekPuan => query.OrderByDescending(y => y.GenelPuan),
                YorumSiralama.EnDusukPuan => query.OrderBy(y => y.GenelPuan),
                _ => query.OrderByDescending(y => y.Tarih)
            };
            
            // Sayfalama
            var yorumlar = await query
                .Skip((filtre.Sayfa - 1) * filtre.SayfaBoyutu)
                .Take(filtre.SayfaBoyutu)
                .Include(y => y.Kullanici)
                .Include(y => y.Urun)
                .Include(y => y.Resimler)
                .Include(y => y.Videolar)
                .Include(y => y.SaticiYaniti).ThenInclude(sy => sy!.Satici)
                .Include(y => y.Etkilesimler)
                .ToListAsync();
            
            return yorumlar.Select(y => MapToDto(y, kullaniciId)).ToList();
        }
        
        public async Task<YorumDto?> GetByIdAsync(int id, int? kullaniciId = null)
        {
            var yorum = await _context.UrunYorumlari
                .Include(y => y.Kullanici)
                .Include(y => y.Urun)
                .Include(y => y.Resimler)
                .Include(y => y.Videolar)
                .Include(y => y.SaticiYaniti).ThenInclude(sy => sy!.Satici)
                .Include(y => y.Etkilesimler)
                .FirstOrDefaultAsync(y => y.Id == id);
            
            return yorum == null ? null : MapToDto(yorum, kullaniciId);
        }
        
        public async Task<UrunYorum> CreateAsync(YorumOlusturDto dto, int kullaniciId)
        {
            // Kullanıcı daha önce yorum yaptı mı kontrol et
            var mevcutYorum = await _context.UrunYorumlari
                .AnyAsync(y => y.UrunId == dto.UrunId && y.KullaniciId == kullaniciId);
            
            if (mevcutYorum)
            {
                throw new InvalidOperationException("Bu ürün için zaten yorum yaptınız.");
            }
            
            var yorum = new UrunYorum
            {
                UrunId = dto.UrunId,
                KullaniciId = kullaniciId,
                SiparisId = dto.SiparisId,
                Baslik = dto.Baslik,
                YorumMetni = dto.YorumMetni,
                GenelPuan = dto.GenelPuan,
                UrunKalitesiPuan = dto.UrunKalitesiPuan,
                FiyatPerformansPuan = dto.FiyatPerformansPuan,
                KargoHiziPuan = dto.KargoHiziPuan,
                PaketlemePuan = dto.PaketlemePuan,
                Tarih = DateTime.Now,
                OnaylandiMi = false,
                Durum = YorumDurum.Beklemede
            };
            
            // Spam kontrolü
            if (await SpamKontrol(dto.YorumMetni))
            {
                yorum.Durum = YorumDurum.Spam;
            }
            
            _context.UrunYorumlari.Add(yorum);
            await _context.SaveChangesAsync();
            
            // Resimleri ekle
            if (dto.ResimUrls.Any())
            {
                var resimler = dto.ResimUrls.Select((url, index) => new YorumResim
                {
                    YorumId = yorum.Id,
                    ResimUrl = url,
                    Sira = index
                }).ToList();
                
                _context.YorumResimleri.AddRange(resimler);
            }
            
            // Video ekle
            if (!string.IsNullOrEmpty(dto.VideoUrl))
            {
                var video = new YorumVideo
                {
                    YorumId = yorum.Id,
                    VideoUrl = dto.VideoUrl
                };
                
                _context.YorumVideolari.Add(video);
            }
            
            await _context.SaveChangesAsync();
            
            // Ürün ortalama puanını güncelle
            await UrunPuanGuncelleAsync(dto.UrunId);
            
            return yorum;
        }
        
        public async Task UpdateAsync(int id, YorumOlusturDto dto)
        {
            var yorum = await _context.UrunYorumlari
                .Include(y => y.Resimler)
                .Include(y => y.Videolar)
                .FirstOrDefaultAsync(y => y.Id == id);
            
            if (yorum == null) return;
            
            yorum.Baslik = dto.Baslik;
            yorum.YorumMetni = dto.YorumMetni;
            yorum.GenelPuan = dto.GenelPuan;
            yorum.UrunKalitesiPuan = dto.UrunKalitesiPuan;
            yorum.FiyatPerformansPuan = dto.FiyatPerformansPuan;
            yorum.KargoHiziPuan = dto.KargoHiziPuan;
            yorum.PaketlemePuan = dto.PaketlemePuan;
            yorum.GuncellenmeTarihi = DateTime.Now;
            yorum.OnaylandiMi = false;
            yorum.Durum = YorumDurum.Beklemede;
            
            // Mevcut resimleri sil
            _context.YorumResimleri.RemoveRange(yorum.Resimler);
            
            // Yeni resimleri ekle
            if (dto.ResimUrls.Any())
            {
                var resimler = dto.ResimUrls.Select((url, index) => new YorumResim
                {
                    YorumId = yorum.Id,
                    ResimUrl = url,
                    Sira = index
                }).ToList();
                
                _context.YorumResimleri.AddRange(resimler);
            }
            
            await _context.SaveChangesAsync();
            await UrunPuanGuncelleAsync(yorum.UrunId);
        }
        
        public async Task DeleteAsync(int id)
        {
            var yorum = await _context.UrunYorumlari.FindAsync(id);
            if (yorum == null) return;
            
            var urunId = yorum.UrunId;
            
            _context.UrunYorumlari.Remove(yorum);
            await _context.SaveChangesAsync();
            
            await UrunPuanGuncelleAsync(urunId);
        }
        
        public async Task<bool> OnaylaAsync(int id)
        {
            var yorum = await _context.UrunYorumlari.FindAsync(id);
            if (yorum == null) return false;
            
            yorum.OnaylandiMi = true;
            yorum.Durum = YorumDurum.Onaylandi;
            
            await _context.SaveChangesAsync();
            await UrunPuanGuncelleAsync(yorum.UrunId);
            
            return true;
        }
        
        public async Task<bool> ReddetAsync(int id, string sebep)
        {
            var yorum = await _context.UrunYorumlari.FindAsync(id);
            if (yorum == null) return false;
            
            yorum.OnaylandiMi = false;
            yorum.Durum = YorumDurum.Reddedildi;
            yorum.RedSebep = sebep;
            
            await _context.SaveChangesAsync();
            
            return true;
        }
        
        public async Task<YorumIstatistik> GetIstatistikAsync(int urunId)
        {
            var yorumlar = await _context.UrunYorumlari
                .Where(y => y.UrunId == urunId && y.OnaylandiMi)
                .Include(y => y.Resimler)
                .ToListAsync();
            
            var toplam = yorumlar.Count;
            
            var istatistik = new YorumIstatistik
            {
                ToplamYorum = toplam,
                OrtalamaPuan = toplam > 0 ? (decimal)yorumlar.Average(y => y.GenelPuan) : 0,
                BesPuan = yorumlar.Count(y => y.GenelPuan == 5),
                DortPuan = yorumlar.Count(y => y.GenelPuan == 4),
                UcPuan = yorumlar.Count(y => y.GenelPuan == 3),
                IkiPuan = yorumlar.Count(y => y.GenelPuan == 2),
                BirPuan = yorumlar.Count(y => y.GenelPuan == 1),
                FotografliYorum = yorumlar.Count(y => y.Resimler.Any()),
                OnayliAliciYorum = yorumlar.Count(y => y.SiparisId != null)
            };
            
            if (toplam > 0)
            {
                istatistik.BesPuanYuzde = (decimal)istatistik.BesPuan / toplam * 100;
                istatistik.DortPuanYuzde = (decimal)istatistik.DortPuan / toplam * 100;
                istatistik.UcPuanYuzde = (decimal)istatistik.UcPuan / toplam * 100;
                istatistik.IkiPuanYuzde = (decimal)istatistik.IkiPuan / toplam * 100;
                istatistik.BirPuanYuzde = (decimal)istatistik.BirPuan / toplam * 100;
            }
            
            return istatistik;
        }
        
        public async Task<bool> YardimciBuldum(int yorumId, int kullaniciId)
        {
            // Daha önce işlem yaptı mı kontrol et
            var mevcutEtkilesim = await _context.YorumEtkilesimleri
                .FirstOrDefaultAsync(e => e.YorumId == yorumId && 
                                         e.KullaniciId == kullaniciId);
            
            if (mevcutEtkilesim != null)
            {
                if (mevcutEtkilesim.Tip == EtkilesimTip.YardimciBuldum)
                {
                    // Geri al
                    _context.YorumEtkilesimleri.Remove(mevcutEtkilesim);
                    
                    var yorum = await _context.UrunYorumlari.FindAsync(yorumId);
                    if (yorum != null) yorum.YardimciBuldum--;
                }
                else if (mevcutEtkilesim.Tip == EtkilesimTip.YardimciBulmadim)
                {
                    // Değiştir
                    mevcutEtkilesim.Tip = EtkilesimTip.YardimciBuldum;
                    
                    var yorum = await _context.UrunYorumlari.FindAsync(yorumId);
                    if (yorum != null)
                    {
                        yorum.YardimciBulmadim--;
                        yorum.YardimciBuldum++;
                    }
                }
            }
            else
            {
                // Yeni ekle
                var etkilesim = new YorumEtkilesim
                {
                    YorumId = yorumId,
                    KullaniciId = kullaniciId,
                    Tip = EtkilesimTip.YardimciBuldum
                };
                
                _context.YorumEtkilesimleri.Add(etkilesim);
                
                var yorum = await _context.UrunYorumlari.FindAsync(yorumId);
                if (yorum != null) yorum.YardimciBuldum++;
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> YardimciBulmadim(int yorumId, int kullaniciId)
        {
            var mevcutEtkilesim = await _context.YorumEtkilesimleri
                .FirstOrDefaultAsync(e => e.YorumId == yorumId && 
                                         e.KullaniciId == kullaniciId);
            
            if (mevcutEtkilesim != null)
            {
                if (mevcutEtkilesim.Tip == EtkilesimTip.YardimciBulmadim)
                {
                    _context.YorumEtkilesimleri.Remove(mevcutEtkilesim);
                    
                    var yorum = await _context.UrunYorumlari.FindAsync(yorumId);
                    if (yorum != null) yorum.YardimciBulmadim--;
                }
                else if (mevcutEtkilesim.Tip == EtkilesimTip.YardimciBuldum)
                {
                    mevcutEtkilesim.Tip = EtkilesimTip.YardimciBulmadim;
                    
                    var yorum = await _context.UrunYorumlari.FindAsync(yorumId);
                    if (yorum != null)
                    {
                        yorum.YardimciBuldum--;
                        yorum.YardimciBulmadim++;
                    }
                }
            }
            else
            {
                var etkilesim = new YorumEtkilesim
                {
                    YorumId = yorumId,
                    KullaniciId = kullaniciId,
                    Tip = EtkilesimTip.YardimciBulmadim
                };
                
                _context.YorumEtkilesimleri.Add(etkilesim);
                
                var yorum = await _context.UrunYorumlari.FindAsync(yorumId);
                if (yorum != null) yorum.YardimciBulmadim++;
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> SikayetEt(int yorumId, int kullaniciId, string sebep, string? aciklama = null)
        {
            var rapor = new YorumRapor
            {
                YorumId = yorumId,
                RaporEdenKullaniciId = kullaniciId,
                Sebep = sebep,
                Aciklama = aciklama,
                Durum = RaporDurum.Beklemede
            };
            
            _context.YorumRaporlari.Add(rapor);
            
            var yorum = await _context.UrunYorumlari.FindAsync(yorumId);
            if (yorum != null) yorum.SikayetSayisi++;
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> KullaniciYorumYapabilirMi(int kullaniciId, int urunId)
        {
            // Daha önce yorum yaptı mı
            var mevcutYorum = await _context.UrunYorumlari
                .AnyAsync(y => y.UrunId == urunId && y.KullaniciId == kullaniciId);
            
            if (mevcutYorum) return false;
            
            // Ürünü satın aldı mı ve teslim edildi mi
            var siparis = await _context.Siparisler
                .AnyAsync(s => s.KullaniciId == kullaniciId &&
                              s.SiparisKalemleri.Any(sk => sk.UrunId == urunId) &&
                              s.Durum == SiparisDurumu.TeslimEdildi);
            
            return siparis;
        }
        
        public async Task<List<YorumDto>> GetKullaniciYorumlariAsync(int kullaniciId)
        {
            var yorumlar = await _context.UrunYorumlari
                .Where(y => y.KullaniciId == kullaniciId)
                .Include(y => y.Urun)
                .Include(y => y.Kullanici)
                .Include(y => y.Resimler)
                .Include(y => y.Videolar)
                .Include(y => y.SaticiYaniti).ThenInclude(sy => sy!.Satici)
                .OrderByDescending(y => y.Tarih)
                .ToListAsync();
            
            return yorumlar.Select(y => MapToDto(y, kullaniciId)).ToList();
        }
        
        private YorumDto MapToDto(UrunYorum yorum, int? kullaniciId)
        {
            var dto = new YorumDto
            {
                Id = yorum.Id,
                UrunId = yorum.UrunId,
                UrunAdi = yorum.Urun?.Ad ?? "",
                UrunResim = yorum.Urun?.ResimUrl,
                KullaniciId = yorum.KullaniciId,
                KullaniciAdi = yorum.Kullanici?.Ad + " " + yorum.Kullanici?.Soyad?.Substring(0, 1) + ".",
                OnayliAlici = yorum.SiparisId != null,
                Baslik = yorum.Baslik,
                YorumMetni = yorum.YorumMetni,
                GenelPuan = yorum.GenelPuan,
                UrunKalitesiPuan = yorum.UrunKalitesiPuan,
                FiyatPerformansPuan = yorum.FiyatPerformansPuan,
                KargoHiziPuan = yorum.KargoHiziPuan,
                PaketlemePuan = yorum.PaketlemePuan,
                OrtalamaPuan = (yorum.GenelPuan + yorum.UrunKalitesiPuan + yorum.FiyatPerformansPuan + 
                               yorum.KargoHiziPuan + yorum.PaketlemePuan) / 5m,
                OnaylandiMi = yorum.OnaylandiMi,
                Durum = yorum.Durum,
                YardimciBuldum = yorum.YardimciBuldum,
                YardimciBulmadim = yorum.YardimciBulmadim,
                SikayetSayisi = yorum.SikayetSayisi,
                Tarih = yorum.Tarih,
                Resimler = yorum.Resimler.OrderBy(r => r.Sira).Select(r => r.ResimUrl).ToList(),
                Videolar = yorum.Videolar.Select(v => v.VideoUrl).ToList()
            };
            
            if (yorum.SaticiYaniti != null)
            {
                dto.SaticiYaniti = new SaticiYanitDto
                {
                    Id = yorum.SaticiYaniti.Id,
                    MagazaAdi = yorum.SaticiYaniti.Satici?.MagazaAdi ?? "",
                    Yanit = yorum.SaticiYaniti.Yanit,
                    Tarih = yorum.SaticiYaniti.Tarih
                };
            }
            
            if (kullaniciId.HasValue)
            {
                var etkilesim = yorum.Etkilesimler.FirstOrDefault(e => e.KullaniciId == kullaniciId.Value);
                if (etkilesim != null)
                {
                    dto.KullaniciYardimciBuldu = etkilesim.Tip == EtkilesimTip.YardimciBuldum;
                    dto.KullaniciYardimciBulmadi = etkilesim.Tip == EtkilesimTip.YardimciBulmadim;
                }
            }
            
            return dto;
        }
        
        private async Task UrunPuanGuncelleAsync(int urunId)
        {
            // Ürün puanı güncelleme - Urun modelinde Puan property'si eklendiğinde aktif edilecek
            /*
            var urun = await _context.Urunler.FindAsync(urunId);
            if (urun == null) return;
            
            var yorumlar = await _context.UrunYorumlari
                .Where(y => y.UrunId == urunId && y.OnaylandiMi)
                .ToListAsync();
            
            if (yorumlar.Any())
            {
                urun.Puan = (decimal)yorumlar.Average(y => (double)y.GenelPuan);
                urun.YorumSayisi = yorumlar.Count;
            }
            else
            {
                urun.Puan = 0;
                urun.YorumSayisi = 0;
            }
            
            await _context.SaveChangesAsync();
            */
        }
        
        private async Task<bool> SpamKontrol(string yorum)
        {
            // Basit spam kontrolü
            if (yorum.Length < 10) return true;
            if (yorum.Contains("http://") || yorum.Contains("https://")) return true;
            
            // Aynı yorumu daha önce yapan var mı
            var benzerYorum = await _context.UrunYorumlari
                .AnyAsync(y => y.YorumMetni == yorum);
            
            return benzerYorum;
        }

        // Yeni metodlar
        public async Task<List<YorumListeDto>> UrunYorumlariniGetirAsync(int urunId, string siralama, int sayfa, int sayfaBoyutu)
        {
            var query = _context.UrunYorumlari
                .Where(y => y.UrunId == urunId && y.OnaylandiMi)
                .AsQueryable();

            query = siralama switch
            {
                "Puan" => query.OrderByDescending(y => y.GenelPuan),
                "Faydali" => query.OrderByDescending(y => y.YardimciBuldum),
                _ => query.OrderByDescending(y => y.Tarih)
            };

            var yorumlar = await query
                .Skip((sayfa - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .Select(y => new YorumListeDto
                {
                    Id = y.Id,
                    UrunId = y.UrunId,
                    KullaniciAdi = y.Kullanici != null ? y.Kullanici.KullaniciAdi : "Anonim",
                    Yorum = y.YorumMetni,
                    GenelPuan = y.GenelPuan,
                    UrunKalitesiPuan = y.UrunKalitesiPuan,
                    FiyatPerformansPuan = y.FiyatPerformansPuan,
                    OnaylandiMi = y.OnaylandiMi,
                    OlusturmaTarihi = y.Tarih,
                    FaydaliSayisi = y.YardimciBuldum,
                    FaydasizSayisi = y.YardimciBulmadim,
                    Resimler = y.Resimler.Select(r => r.ResimUrl).ToList()
                })
                .ToListAsync();

            return yorumlar;
        }

        public async Task<bool> YorumYapilabilirMiAsync(int urunId, string kullaniciId)
        {
            // String ID'yi int'e çevir
            if (!int.TryParse(kullaniciId, out int kullaniciIdInt))
                return false;

            // Kullanıcının bu ürünü satın alıp almadığını kontrol et
            var siparisVar = await _context.Siparisler
                .AnyAsync(s => s.KullaniciId == kullaniciIdInt && 
                              s.Durum == SiparisDurumu.TeslimEdildi);
            
            if (!siparisVar) return false;

            // Daha önce yorum yapıp yapmadığını kontrol et
            var yorumVar = await _context.UrunYorumlari
                .AnyAsync(y => y.UrunId == urunId && y.KullaniciId == kullaniciIdInt);

            return !yorumVar;
        }

        public async Task<bool> YorumEkleAsync(YorumEkleDto dto)
        {
            if (!int.TryParse(dto.KullaniciId, out int kullaniciIdInt))
                return false;

            var yorum = new UrunYorum
            {
                UrunId = dto.UrunId,
                KullaniciId = kullaniciIdInt,
                YorumMetni = dto.Yorum ?? "",
                Baslik = "Yorum",
                GenelPuan = dto.GenelPuan,
                UrunKalitesiPuan = dto.UrunKalitesiPuan,
                FiyatPerformansPuan = dto.FiyatPerformansPuan,
                OnaylandiMi = false,
                Tarih = DateTime.Now
            };

            _context.UrunYorumlari.Add(yorum);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task YorumFaydaliIsaretle(int yorumId, string kullaniciId, bool faydali)
        {
            var yorum = await _context.UrunYorumlari.FindAsync(yorumId);
            if (yorum == null) return;

            if (faydali)
                yorum.YardimciBuldum++;
            else
                yorum.YardimciBulmadim++;

            await _context.SaveChangesAsync();
        }

        public async Task YorumRaporEtAsync(int yorumId, string kullaniciId, string sebep, string aciklama)
        {
            if (!int.TryParse(kullaniciId, out int kullaniciIdInt))
                return;

            var rapor = new YorumRapor
            {
                YorumId = yorumId,
                RaporEdenKullaniciId = kullaniciIdInt,
                Sebep = sebep,
                Aciklama = aciklama,
                Durum = RaporDurum.Beklemede,
                Tarih = DateTime.Now
            };

            _context.YorumRaporlari.Add(rapor);
            await _context.SaveChangesAsync();
        }

        public async Task<List<YorumListeDto>> KullaniciYorumlariniGetirAsync(string kullaniciId, int sayfa, int sayfaBoyutu)
        {
            if (!int.TryParse(kullaniciId, out int kullaniciIdInt))
                return new List<YorumListeDto>();

            return await _context.UrunYorumlari
                .Where(y => y.KullaniciId == kullaniciIdInt)
                .OrderByDescending(y => y.Tarih)
                .Skip((sayfa - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .Select(y => new YorumListeDto
                {
                    Id = y.Id,
                    UrunId = y.UrunId,
                    UrunAdi = y.Urun != null ? y.Urun.Ad : "",
                    Yorum = y.YorumMetni,
                    GenelPuan = y.GenelPuan,
                    OnaylandiMi = y.OnaylandiMi,
                    OlusturmaTarihi = y.Tarih,
                    FaydaliSayisi = y.YardimciBuldum,
                    FaydasizSayisi = y.YardimciBulmadim
                })
                .ToListAsync();
        }

        public async Task<bool> YorumSilAsync(int yorumId, string kullaniciId)
        {
            if (!int.TryParse(kullaniciId, out int kullaniciIdInt))
                return false;

            var yorum = await _context.UrunYorumlari
                .FirstOrDefaultAsync(y => y.Id == yorumId && y.KullaniciId == kullaniciIdInt);
            
            if (yorum == null) return false;

            _context.UrunYorumlari.Remove(yorum);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<YorumListeDto>> AdminYorumListesiAsync(string durum, int sayfa, int sayfaBoyutu)
        {
            var query = _context.UrunYorumlari.AsQueryable();

            query = durum switch
            {
                "Bekleyen" => query.Where(y => !y.OnaylandiMi),
                "Onaylanan" => query.Where(y => y.OnaylandiMi),
                _ => query
            };

            return await query
                .OrderByDescending(y => y.Tarih)
                .Skip((sayfa - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .Select(y => new YorumListeDto
                {
                    Id = y.Id,
                    UrunId = y.UrunId,
                    UrunAdi = y.Urun != null ? y.Urun.Ad : "",
                    KullaniciAdi = y.Kullanici != null ? y.Kullanici.KullaniciAdi : "",
                    Yorum = y.YorumMetni,
                    GenelPuan = y.GenelPuan,
                    OnaylandiMi = y.OnaylandiMi,
                    OlusturmaTarihi = y.Tarih
                })
                .ToListAsync();
        }

        public async Task YorumOnaylaAsync(int yorumId)
        {
            var yorum = await _context.UrunYorumlari.FindAsync(yorumId);
            if (yorum != null)
            {
                yorum.OnaylandiMi = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task YorumReddetAsync(int yorumId, string sebep)
        {
            var yorum = await _context.UrunYorumlari.FindAsync(yorumId);
            if (yorum != null)
            {
                yorum.OnaylandiMi = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AdminYorumSilAsync(int yorumId)
        {
            var yorum = await _context.UrunYorumlari.FindAsync(yorumId);
            if (yorum != null)
            {
                _context.UrunYorumlari.Remove(yorum);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<YorumRaporDto>> YorumRaporlariniGetirAsync(string durum, int sayfa, int sayfaBoyutu)
        {
            var query = _context.YorumRaporlari.AsQueryable();

            if (durum == "Bekleyen")
                query = query.Where(r => r.Durum == RaporDurum.Beklemede);

            return await query
                .OrderByDescending(r => r.Tarih)
                .Skip((sayfa - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .Select(r => new YorumRaporDto
                {
                    Id = r.Id,
                    YorumId = r.YorumId,
                    Sebep = r.Sebep,
                    Aciklama = r.Aciklama,
                    Durum = r.Durum,
                    Tarih = r.Tarih
                })
                .ToListAsync();
        }

        public async Task RaporIsleAsync(int raporId, string karar, string aciklama)
        {
            var rapor = await _context.YorumRaporlari.FindAsync(raporId);
            if (rapor != null)
            {
                rapor.Durum = karar == "Onayla" ? RaporDurum.Kabul : RaporDurum.Red;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<object> YorumIstatistikleriAsync()
        {
            var toplamYorum = await _context.UrunYorumlari.CountAsync();
            var onayBekleyen = await _context.UrunYorumlari.CountAsync(y => !y.OnaylandiMi);
            var onaylanan = await _context.UrunYorumlari.CountAsync(y => y.OnaylandiMi);

            return new
            {
                ToplamYorum = toplamYorum,
                OnayBekleyen = onayBekleyen,
                Onaylanan = onaylanan,
                OrtalamaPuan = toplamYorum > 0 ? await _context.UrunYorumlari.AverageAsync(y => (double)y.GenelPuan) : 0
            };
        }
    }
}
