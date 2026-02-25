using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Models.DTOs;

namespace TrendyolClone.Services
{
    public class GelismisSiparisService : IGelismisSiparisService
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<GelismisSiparisService> _logger;

        public GelismisSiparisService(UygulamaDbContext context, ILogger<GelismisSiparisService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SiparisDetayDto?> SiparisDetayGetirAsync(int siparisId, int kullaniciId)
        {
            var siparis = await _context.Siparisler
                .Include(s => s.SiparisKalemleri)
                    .ThenInclude(sk => sk.Urun)
                .FirstOrDefaultAsync(s => s.Id == siparisId && s.KullaniciId == kullaniciId);

            if (siparis == null)
                return null;

            var kargo = await _context.KargoTakipler
                .FirstOrDefaultAsync(k => k.SiparisId == siparisId);

            var fatura = await _context.Faturalar
                .FirstOrDefaultAsync(f => f.SiparisId == siparisId);

            var iade = await _context.Iadeler
                .FirstOrDefaultAsync(i => i.SiparisId == siparisId);

            var durumGecmisi = await _context.SiparisDurumGecmisleri
                .Where(d => d.SiparisId == siparisId)
                .OrderBy(d => d.Tarih)
                .Select(d => new SiparisDurumDto
                {
                    Durum = d.Durum,
                    Aciklama = d.Aciklama,
                    Tarih = d.Tarih,
                    DegistirenKisi = d.DegistirenKisi
                })
                .ToListAsync();

            return new SiparisDetayDto
            {
                Id = siparis.Id,
                SiparisNo = $"SIP-{siparis.Id:D6}",
                SiparisTarihi = siparis.SiparisTarihi,
                Durum = siparis.Durum,
                ToplamTutar = siparis.ToplamTutar,
                TeslimatAdresi = siparis.TeslimatAdresi,
                OdemeYontemi = siparis.OdemeYontemi,
                KargoFirmasi = kargo?.KargoFirmasi,
                KargoTakipNo = kargo?.TakipNo,
                KargoDurumu = kargo?.KargoDurumu,
                TahminiTeslimatTarihi = kargo?.TahminiTeslimatTarihi,
                FaturaNo = fatura?.FaturaNo,
                FaturaPdfUrl = fatura?.PdfUrl,
                Urunler = siparis.SiparisKalemleri.Select(sk => new SiparisUrunDto
                {
                    UrunId = sk.UrunId,
                    UrunAdi = sk.Urun.Ad,
                    ResimUrl = sk.Urun.ResimUrl,
                    Adet = sk.Miktar,
                    BirimFiyat = sk.BirimFiyat,
                    ToplamFiyat = sk.Miktar * sk.BirimFiyat
                }).ToList(),
                DurumGecmisi = durumGecmisi,
                IadeTalebiVarMi = iade != null,
                IadeDurumu = iade?.Durum
            };
        }

        public async Task<List<SiparisDetayDto>> KullaniciSiparisleriniGetirAsync(int kullaniciId)
        {
            var siparisler = await _context.Siparisler
                .Where(s => s.KullaniciId == kullaniciId)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            var sonuclar = new List<SiparisDetayDto>();

            foreach (var siparis in siparisler)
            {
                var detay = await SiparisDetayGetirAsync(siparis.Id, kullaniciId);
                if (detay != null)
                {
                    sonuclar.Add(detay);
                }
            }

            return sonuclar;
        }

        public async Task SiparisDurumuGuncelleAsync(int siparisId, SiparisDurumu yeniDurum, string? aciklama = null, string? degistirenKisi = null)
        {
            var siparis = await _context.Siparisler.FindAsync(siparisId);
            if (siparis == null)
                throw new Exception("Sipariş bulunamadı");

            siparis.Durum = yeniDurum;

            var gecmis = new SiparisDurumGecmisi
            {
                SiparisId = siparisId,
                Durum = yeniDurum,
                Aciklama = aciklama,
                Tarih = DateTime.Now,
                DegistirenKisi = degistirenKisi ?? "Sistem"
            };

            _context.SiparisDurumGecmisleri.Add(gecmis);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Sipariş {siparisId} durumu {yeniDurum} olarak güncellendi");
        }

        public async Task<List<SiparisDurumDto>> DurumGecmisiGetirAsync(int siparisId)
        {
            return await _context.SiparisDurumGecmisleri
                .Where(d => d.SiparisId == siparisId)
                .OrderBy(d => d.Tarih)
                .Select(d => new SiparisDurumDto
                {
                    Durum = d.Durum,
                    Aciklama = d.Aciklama,
                    Tarih = d.Tarih,
                    DegistirenKisi = d.DegistirenKisi
                })
                .ToListAsync();
        }

        public async Task KargoTakipEkleAsync(int siparisId, string kargoFirmasi, string takipNo)
        {
            var mevcutKargo = await _context.KargoTakipler
                .FirstOrDefaultAsync(k => k.SiparisId == siparisId);

            if (mevcutKargo != null)
            {
                mevcutKargo.KargoFirmasi = kargoFirmasi;
                mevcutKargo.TakipNo = takipNo;
                mevcutKargo.GuncellenmeTarihi = DateTime.Now;
            }
            else
            {
                var kargo = new KargoTakip
                {
                    SiparisId = siparisId,
                    KargoFirmasi = kargoFirmasi,
                    TakipNo = takipNo,
                    OlusturmaTarihi = DateTime.Now
                };

                _context.KargoTakipler.Add(kargo);
            }

            await _context.SaveChangesAsync();

            // Sipariş durumunu güncelle
            await SiparisDurumuGuncelleAsync(siparisId, SiparisDurumu.Kargoda, $"Kargo takip no: {takipNo}");
        }

        public async Task<KargoTakipDto?> KargoTakipGetirAsync(int siparisId)
        {
            var kargo = await _context.KargoTakipler
                .Include(k => k.Hareketler.OrderBy(h => h.Tarih))
                .FirstOrDefaultAsync(k => k.SiparisId == siparisId);

            if (kargo == null)
                return null;

            return new KargoTakipDto
            {
                KargoFirmasi = kargo.KargoFirmasi,
                TakipNo = kargo.TakipNo,
                KargoDurumu = kargo.KargoDurumu,
                TahminiTeslimatTarihi = kargo.TahminiTeslimatTarihi,
                Hareketler = kargo.Hareketler.Select(h => new KargoHareketDto
                {
                    Durum = h.Durum,
                    Aciklama = h.Aciklama,
                    Lokasyon = h.Lokasyon,
                    Tarih = h.Tarih
                }).ToList()
            };
        }

        public async Task KargoHareketEkleAsync(int siparisId, string durum, string? aciklama = null, string? lokasyon = null)
        {
            var kargo = await _context.KargoTakipler
                .FirstOrDefaultAsync(k => k.SiparisId == siparisId);

            if (kargo == null)
                throw new Exception("Kargo takip bilgisi bulunamadı");

            var hareket = new KargoHareket
            {
                KargoTakipId = kargo.Id,
                Durum = durum,
                Aciklama = aciklama,
                Lokasyon = lokasyon,
                Tarih = DateTime.Now
            };

            kargo.KargoDurumu = durum;
            kargo.GuncellenmeTarihi = DateTime.Now;

            _context.KargoHareketler.Add(hareket);
            await _context.SaveChangesAsync();
        }

        public async Task<Fatura> FaturaOlusturAsync(int siparisId)
        {
            var siparis = await _context.Siparisler
                .Include(s => s.SiparisKalemleri)
                .FirstOrDefaultAsync(s => s.Id == siparisId);

            if (siparis == null)
                throw new Exception("Sipariş bulunamadı");

            var mevcutFatura = await _context.Faturalar
                .FirstOrDefaultAsync(f => f.SiparisId == siparisId);

            if (mevcutFatura != null)
                return mevcutFatura;

            // Fatura numarası oluştur
            var yil = DateTime.Now.Year;
            var sonFatura = await _context.Faturalar
                .Where(f => f.FaturaNo.StartsWith($"FTR-{yil}"))
                .OrderByDescending(f => f.Id)
                .FirstOrDefaultAsync();

            int sira = 1;
            if (sonFatura != null)
            {
                var parts = sonFatura.FaturaNo.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int sonSira))
                {
                    sira = sonSira + 1;
                }
            }

            var faturaNo = $"FTR-{yil}-{sira:D5}";

            // Tutar hesaplamaları
            var araToplam = siparis.SiparisKalemleri.Sum(sk => sk.Miktar * sk.BirimFiyat);
            var kdv = araToplam * 0.18m; // %18 KDV
            var kargoUcreti = 0m; // Session'dan alınabilir
            var indirimTutari = 0m; // Kupon indirimi

            var fatura = new Fatura
            {
                SiparisId = siparisId,
                FaturaNo = faturaNo,
                FaturaTarihi = DateTime.Now,
                AraToplam = araToplam,
                KDV = kdv,
                KargoUcreti = kargoUcreti,
                IndirimTutari = indirimTutari,
                Toplam = siparis.ToplamTutar
            };

            _context.Faturalar.Add(fatura);
            await _context.SaveChangesAsync();

            return fatura;
        }

        public async Task<Fatura?> FaturaGetirAsync(int siparisId)
        {
            return await _context.Faturalar
                .FirstOrDefaultAsync(f => f.SiparisId == siparisId);
        }

        public async Task<Iade> IadeTalebiOlusturAsync(IadeTalebiDto dto, int kullaniciId)
        {
            var siparis = await _context.Siparisler
                .Include(s => s.SiparisKalemleri)
                .FirstOrDefaultAsync(s => s.Id == dto.SiparisId && s.KullaniciId == kullaniciId);

            if (siparis == null)
                throw new Exception("Sipariş bulunamadı");

            if (siparis.Durum != SiparisDurumu.TeslimEdildi)
                throw new Exception("Sadece teslim edilmiş siparişler için iade talebi oluşturulabilir");

            // İade tutarını hesapla
            decimal iadeTutari = 0;
            var iadeUrunleri = new List<IadeUrunu>();

            foreach (var urunDto in dto.Urunler)
            {
                var siparisKalemi = siparis.SiparisKalemleri
                    .FirstOrDefault(sk => sk.UrunId == urunDto.UrunId);

                if (siparisKalemi != null)
                {
                    var iadeUrunu = new IadeUrunu
                    {
                        UrunId = urunDto.UrunId,
                        Adet = urunDto.Adet,
                        BirimFiyat = siparisKalemi.BirimFiyat
                    };

                    iadeUrunleri.Add(iadeUrunu);
                    iadeTutari += iadeUrunu.ToplamFiyat;
                }
            }

            var iade = new Iade
            {
                SiparisId = dto.SiparisId,
                KullaniciId = kullaniciId,
                Neden = dto.Neden,
                Aciklama = dto.Aciklama,
                Durum = IadeDurumu.TalepEdildi,
                IadeTutari = iadeTutari,
                TalepTarihi = DateTime.Now,
                Urunler = iadeUrunleri
            };

            _context.Iadeler.Add(iade);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"İade talebi oluşturuldu. Sipariş: {dto.SiparisId}, Tutar: {iadeTutari}");

            return iade;
        }

        public async Task IadeTalebiOnaylaAsync(int iadeId, string? aciklama = null)
        {
            var iade = await _context.Iadeler.FindAsync(iadeId);
            if (iade == null)
                throw new Exception("İade talebi bulunamadı");

            iade.Durum = IadeDurumu.Onaylandi;
            iade.OnayTarihi = DateTime.Now;

            if (!string.IsNullOrEmpty(aciklama))
            {
                iade.Aciklama = (iade.Aciklama ?? "") + "\n\nOnay Notu: " + aciklama;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"İade talebi onaylandı. İade ID: {iadeId}");
        }

        public async Task IadeTalebiReddetAsync(int iadeId, string redNedeni)
        {
            var iade = await _context.Iadeler.FindAsync(iadeId);
            if (iade == null)
                throw new Exception("İade talebi bulunamadı");

            iade.Durum = IadeDurumu.Reddedildi;
            iade.RedNedeni = redNedeni;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"İade talebi reddedildi. İade ID: {iadeId}");
        }

        public async Task<Iade?> IadeGetirAsync(int iadeId)
        {
            return await _context.Iadeler
                .Include(i => i.Siparis)
                .Include(i => i.Urunler)
                    .ThenInclude(iu => iu.Urun)
                .FirstOrDefaultAsync(i => i.Id == iadeId);
        }

        public async Task<List<Iade>> KullaniciIadeleriniGetirAsync(int kullaniciId)
        {
            return await _context.Iadeler
                .Include(i => i.Siparis)
                .Include(i => i.Urunler)
                    .ThenInclude(iu => iu.Urun)
                .Where(i => i.KullaniciId == kullaniciId)
                .OrderByDescending(i => i.TalepTarihi)
                .ToListAsync();
        }

        public async Task SiparisIptalEtAsync(int siparisId, string neden)
        {
            var siparis = await _context.Siparisler.FindAsync(siparisId);
            if (siparis == null)
                throw new Exception("Sipariş bulunamadı");

            if (siparis.Durum == SiparisDurumu.TeslimEdildi)
                throw new Exception("Teslim edilmiş sipariş iptal edilemez");

            await SiparisDurumuGuncelleAsync(siparisId, SiparisDurumu.IptalEdildi, neden);

            _logger.LogInformation($"Sipariş iptal edildi. Sipariş ID: {siparisId}, Neden: {neden}");
        }
    }
}
