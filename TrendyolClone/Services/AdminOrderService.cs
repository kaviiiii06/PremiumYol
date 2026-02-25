using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly UygulamaDbContext _context;
        private readonly IBildirimService _bildirimService;
        private readonly ILogger<AdminOrderService> _logger;

        public AdminOrderService(
            UygulamaDbContext context,
            IBildirimService bildirimService,
            ILogger<AdminOrderService> logger)
        {
            _context = context;
            _bildirimService = bildirimService;
            _logger = logger;
        }

        public async Task<SiparisListeResult> GetSiparislerAsync(SiparisFiltre filtre)
        {
            var query = _context.Siparisler
                .Include(s => s.Kullanici)
                .Include(s => s.SiparisKalemleri)
                .AsQueryable();

            // Arama
            if (!string.IsNullOrEmpty(filtre.AramaTerimi))
            {
                var arama = filtre.AramaTerimi.ToLower();
                query = query.Where(s =>
                    s.SiparisNo.ToLower().Contains(arama) ||
                    s.Kullanici.Ad.ToLower().Contains(arama) ||
                    s.Kullanici.Soyad.ToLower().Contains(arama) ||
                    s.Kullanici.Email.ToLower().Contains(arama));
            }

            // Durum filtresi
            if (filtre.Durum.HasValue)
            {
                query = query.Where(s => s.Durum == filtre.Durum.Value);
            }

            // Tarih filtresi
            if (filtre.BaslangicTarihi.HasValue)
            {
                query = query.Where(s => s.SiparisTarihi >= filtre.BaslangicTarihi.Value);
            }

            if (filtre.BitisTarihi.HasValue)
            {
                query = query.Where(s => s.SiparisTarihi <= filtre.BitisTarihi.Value);
            }

            // Toplam kayıt sayısı
            var toplamKayit = await query.CountAsync();

            // Sıralama
            query = filtre.Siralama switch
            {
                "tarih_asc" => query.OrderBy(s => s.SiparisTarihi),
                "tarih_desc" => query.OrderByDescending(s => s.SiparisTarihi),
                "tutar_asc" => query.OrderBy(s => s.ToplamTutar),
                "tutar_desc" => query.OrderByDescending(s => s.ToplamTutar),
                _ => query.OrderByDescending(s => s.SiparisTarihi)
            };

            // Sayfalama
            var siparisler = await query
                .Skip((filtre.Sayfa - 1) * filtre.SayfaBoyutu)
                .Take(filtre.SayfaBoyutu)
                .Select(s => new SiparisYonetimDto
                {
                    Id = s.Id,
                    SiparisNo = s.SiparisNo,
                    KullaniciAdi = s.Kullanici.Ad + " " + s.Kullanici.Soyad,
                    Email = s.Kullanici.Email,
                    Telefon = s.Kullanici.TelefonNumarasi ?? "",
                    ToplamTutar = s.ToplamTutar,
                    Durum = s.Durum,
                    DurumAdi = GetDurumAdi(s.Durum),
                    SiparisTarihi = s.SiparisTarihi,
                    UrunSayisi = s.SiparisKalemleri.Sum(sk => sk.Adet),
                    TeslimatAdresi = s.TeslimatAdresi ?? ""
                })
                .ToListAsync();

            return new SiparisListeResult
            {
                Siparisler = siparisler,
                ToplamKayit = toplamKayit,
                ToplamSayfa = (int)Math.Ceiling(toplamKayit / (double)filtre.SayfaBoyutu),
                MevcutSayfa = filtre.Sayfa
            };
        }

        public async Task<SiparisDetayYonetimDto?> GetSiparisDetayAsync(int siparisId)
        {
            var siparis = await _context.Siparisler
                .Include(s => s.Kullanici)
                .Include(s => s.SiparisKalemleri)
                    .ThenInclude(sk => sk.Urun)
                .FirstOrDefaultAsync(s => s.Id == siparisId);

            if (siparis == null) return null;

            // Kargo takip bilgisi
            var kargoTakip = await _context.KargoTakipler
                .FirstOrDefaultAsync(k => k.SiparisId == siparisId);

            // Durum geçmişi
            var durumGecmisi = await _context.SiparisDurumGecmisleri
                .Where(d => d.SiparisId == siparisId)
                .OrderByDescending(d => d.Tarih)
                .Select(d => new DurumGecmisiDto
                {
                    Durum = d.Durum,
                    DurumAdi = GetDurumAdi(d.Durum),
                    Aciklama = d.Aciklama,
                    Tarih = d.Tarih,
                    DegistirenKisi = d.DegistirenKisi
                })
                .ToListAsync();

            return new SiparisDetayYonetimDto
            {
                Id = siparis.Id,
                SiparisNo = siparis.SiparisNo,
                KullaniciId = siparis.KullaniciId,
                KullaniciAdi = siparis.Kullanici.Ad + " " + siparis.Kullanici.Soyad,
                Email = siparis.Kullanici.Email,
                Telefon = siparis.Kullanici.TelefonNumarasi ?? "",
                AraToplam = siparis.SiparisKalemleri.Sum(sk => sk.BirimFiyat * sk.Adet),
                KargoUcreti = siparis.KargoUcreti,
                IndirimTutari = siparis.IndirimTutari,
                ToplamTutar = siparis.ToplamTutar,
                Durum = siparis.Durum,
                DurumAdi = GetDurumAdi(siparis.Durum),
                SiparisTarihi = siparis.SiparisTarihi,
                AdminNotu = siparis.AdminNotu,
                TeslimatAdresi = siparis.TeslimatAdresi ?? "",
                FaturaAdresi = siparis.FaturaAdresi ?? "",
                KargoTakipNo = kargoTakip?.TakipNo,
                KargoFirmasi = kargoTakip?.KargoFirmasi,
                TahminiTeslimat = kargoTakip?.TahminiTeslimatTarihi,
                Urunler = siparis.SiparisKalemleri.Select(sk => new SiparisKalemDto
                {
                    UrunId = sk.UrunId,
                    UrunAdi = sk.Urun.Ad,
                    ResimUrl = sk.Urun.ResimUrl ?? "/images/no-image.jpg",
                    Adet = sk.Adet,
                    BirimFiyat = sk.BirimFiyat,
                    ToplamFiyat = sk.BirimFiyat * sk.Adet
                }).ToList(),
                DurumGecmisi = durumGecmisi
            };
        }

        public async Task<bool> DurumGuncelleAsync(SiparisDurumGuncelleDto dto, int adminId)
        {
            try
            {
                var siparis = await _context.Siparisler
                    .Include(s => s.Kullanici)
                    .FirstOrDefaultAsync(s => s.Id == dto.SiparisId);

                if (siparis == null) return false;

                var eskiDurum = siparis.Durum;
                siparis.Durum = dto.YeniDurum;

                // Durum geçmişi ekle
                var durumGecmisi = new SiparisDurumGecmisi
                {
                    SiparisId = dto.SiparisId,
                    Durum = dto.YeniDurum,
                    Aciklama = dto.Aciklama,
                    Tarih = DateTime.Now,
                    KullaniciId = adminId,
                    DegistirenKisi = "Admin"
                };

                _context.SiparisDurumGecmisleri.Add(durumGecmisi);

                // Kargo bilgisi varsa ekle
                if (!string.IsNullOrEmpty(dto.KargoTakipNo) && !string.IsNullOrEmpty(dto.KargoFirmasi))
                {
                    var kargoTakip = await _context.KargoTakipler
                        .FirstOrDefaultAsync(k => k.SiparisId == dto.SiparisId);

                    if (kargoTakip == null)
                    {
                        kargoTakip = new KargoTakip
                        {
                            SiparisId = dto.SiparisId,
                            KargoFirmasi = dto.KargoFirmasi,
                            TakipNo = dto.KargoTakipNo,
                            OlusturmaTarihi = DateTime.Now
                        };
                        _context.KargoTakipler.Add(kargoTakip);
                    }
                    else
                    {
                        kargoTakip.KargoFirmasi = dto.KargoFirmasi;
                        kargoTakip.TakipNo = dto.KargoTakipNo;
                        kargoTakip.GuncellenmeTarihi = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();

                // Bildirim gönder
                await GonderDurumBildirimi(siparis, dto.YeniDurum);

                _logger.LogInformation($"Sipariş durumu güncellendi: {siparis.SiparisNo} - {eskiDurum} -> {dto.YeniDurum}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Sipariş durumu güncellenirken hata: {dto.SiparisId}");
                return false;
            }
        }

        public async Task<bool> KargoBilgisiEkleAsync(int siparisId, string kargoFirmasi, string takipNo)
        {
            try
            {
                var kargoTakip = await _context.KargoTakipler
                    .FirstOrDefaultAsync(k => k.SiparisId == siparisId);

                if (kargoTakip == null)
                {
                    kargoTakip = new KargoTakip
                    {
                        SiparisId = siparisId,
                        KargoFirmasi = kargoFirmasi,
                        TakipNo = takipNo,
                        OlusturmaTarihi = DateTime.Now
                    };
                    _context.KargoTakipler.Add(kargoTakip);
                }
                else
                {
                    kargoTakip.KargoFirmasi = kargoFirmasi;
                    kargoTakip.TakipNo = takipNo;
                    kargoTakip.GuncellenmeTarihi = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Kargo bilgisi eklenirken hata: {siparisId}");
                return false;
            }
        }

        public async Task<bool> AdminNotuEkleAsync(int siparisId, string not)
        {
            try
            {
                var siparis = await _context.Siparisler.FindAsync(siparisId);
                if (siparis == null) return false;

                siparis.AdminNotu = not;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Admin notu eklenirken hata: {siparisId}");
                return false;
            }
        }

        public async Task<bool> SiparisIptalEtAsync(int siparisId, string iptalNedeni, int adminId)
        {
            try
            {
                var siparis = await _context.Siparisler
                    .Include(s => s.Kullanici)
                    .FirstOrDefaultAsync(s => s.Id == siparisId);

                if (siparis == null) return false;

                siparis.Durum = SiparisDurumu.IptalEdildi;

                var durumGecmisi = new SiparisDurumGecmisi
                {
                    SiparisId = siparisId,
                    Durum = SiparisDurumu.IptalEdildi,
                    Aciklama = iptalNedeni,
                    Tarih = DateTime.Now,
                    KullaniciId = adminId,
                    DegistirenKisi = "Admin"
                };

                _context.SiparisDurumGecmisleri.Add(durumGecmisi);
                await _context.SaveChangesAsync();

                // Bildirim gönder
                await _bildirimService.EmailGonderAsync(
                    siparis.KullaniciId,
                    siparis.Kullanici.Email,
                    "Siparişiniz İptal Edildi",
                    $"<h2>Siparişiniz İptal Edildi</h2><p>Sipariş No: {siparis.SiparisNo}</p><p>İptal Nedeni: {iptalNedeni}</p>"
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Sipariş iptal edilirken hata: {siparisId}");
                return false;
            }
        }

        public async Task<Dictionary<SiparisDurumu, int>> GetDurumIstatistikleriAsync()
        {
            return await _context.Siparisler
                .GroupBy(s => s.Durum)
                .Select(g => new { Durum = g.Key, Sayi = g.Count() })
                .ToDictionaryAsync(x => x.Durum, x => x.Sayi);
        }

        public async Task<List<SiparisYonetimDto>> GetBekleyenSiparislerAsync()
        {
            return await _context.Siparisler
                .Include(s => s.Kullanici)
                .Include(s => s.SiparisKalemleri)
                .Where(s => s.Durum == SiparisDurumu.Onaylandi || s.Durum == SiparisDurumu.Hazirlaniyor)
                .OrderBy(s => s.SiparisTarihi)
                .Take(20)
                .Select(s => new SiparisYonetimDto
                {
                    Id = s.Id,
                    SiparisNo = s.SiparisNo,
                    KullaniciAdi = s.Kullanici.Ad + " " + s.Kullanici.Soyad,
                    Email = s.Kullanici.Email,
                    ToplamTutar = s.ToplamTutar,
                    Durum = s.Durum,
                    DurumAdi = GetDurumAdi(s.Durum),
                    SiparisTarihi = s.SiparisTarihi,
                    UrunSayisi = s.SiparisKalemleri.Sum(sk => sk.Adet)
                })
                .ToListAsync();
        }

        private async Task GonderDurumBildirimi(Siparis siparis, SiparisDurumu yeniDurum)
        {
            var baslik = yeniDurum switch
            {
                SiparisDurumu.Onaylandi => "Siparişiniz Onaylandı",
                SiparisDurumu.Hazirlaniyor => "Siparişiniz Hazırlanıyor",
                SiparisDurumu.KargoyaVerildi => "Siparişiniz Kargoya Verildi",
                SiparisDurumu.TeslimEdildi => "Siparişiniz Teslim Edildi",
                SiparisDurumu.IptalEdildi => "Siparişiniz İptal Edildi",
                _ => "Sipariş Durumu Güncellendi"
            };

            var icerik = $"<h2>{baslik}</h2><p>Sipariş No: {siparis.SiparisNo}</p><p>Yeni Durum: {GetDurumAdi(yeniDurum)}</p>";

            await _bildirimService.EmailGonderAsync(
                siparis.KullaniciId,
                siparis.Kullanici.Email,
                baslik,
                icerik
            );
        }

        private static string GetDurumAdi(SiparisDurumu durum)
        {
            return durum switch
            {
                SiparisDurumu.Beklemede => "Beklemede",
                SiparisDurumu.Onaylandi => "Onaylandı",
                SiparisDurumu.Hazirlaniyor => "Hazırlanıyor",
                SiparisDurumu.KargoyaVerildi => "Kargoya Verildi",
                SiparisDurumu.TeslimEdildi => "Teslim Edildi",
                SiparisDurumu.IptalEdildi => "İptal Edildi",
                _ => durum.ToString()
            };
        }
    }
}
