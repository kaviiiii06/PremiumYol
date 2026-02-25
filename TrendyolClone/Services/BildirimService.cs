using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;
using TrendyolClone.Data;
using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public class BildirimService : IBildirimService
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<BildirimService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public BildirimService(
            UygulamaDbContext context,
            ILogger<BildirimService> logger,
            IEmailSender emailSender,
            ISmsSender smsSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }

        public async Task<bool> EmailGonderAsync(int? kullaniciId, string email, string baslik, string icerik, string? sablonKodu = null)
        {
            try
            {
                // Kullanıcı tercihlerini kontrol et
                if (kullaniciId.HasValue)
                {
                    var tercihler = await TercihleriGetirAsync(kullaniciId.Value);
                    if (tercihler != null && !tercihler.EmailBildirimleri)
                    {
                        _logger.LogInformation($"Kullanıcı {kullaniciId} email bildirimlerini kapatmış.");
                        return false;
                    }
                }

                var bildirim = new Bildirim
                {
                    KullaniciId = kullaniciId,
                    Turu = BildirimTuru.Email,
                    Baslik = baslik,
                    Icerik = icerik,
                    Email = email,
                    SablonKodu = sablonKodu,
                    Durum = BildirimDurumu.Beklemede,
                    OlusturmaTarihi = DateTime.Now
                };

                _context.Bildirimler.Add(bildirim);
                await _context.SaveChangesAsync();

                // Hemen göndermeyi dene
                await BildirimGonderAsync(bildirim.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email gönderme hatası");
                return false;
            }
        }

        public async Task<bool> SmsGonderAsync(int? kullaniciId, string telefonNo, string mesaj, string? sablonKodu = null)
        {
            try
            {
                // Kullanıcı tercihlerini kontrol et
                if (kullaniciId.HasValue)
                {
                    var tercihler = await TercihleriGetirAsync(kullaniciId.Value);
                    if (tercihler != null && !tercihler.SmsBildirimleri)
                    {
                        _logger.LogInformation($"Kullanıcı {kullaniciId} SMS bildirimlerini kapatmış.");
                        return false;
                    }
                }

                var bildirim = new Bildirim
                {
                    KullaniciId = kullaniciId,
                    Turu = BildirimTuru.SMS,
                    Baslik = "SMS Bildirimi",
                    Icerik = mesaj,
                    TelefonNo = telefonNo,
                    SablonKodu = sablonKodu,
                    Durum = BildirimDurumu.Beklemede,
                    OlusturmaTarihi = DateTime.Now
                };

                _context.Bildirimler.Add(bildirim);
                await _context.SaveChangesAsync();

                // Hemen göndermeyi dene
                await BildirimGonderAsync(bildirim.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMS gönderme hatası");
                return false;
            }
        }

        public async Task<bool> SablonIleBildirimGonderAsync(string sablonKodu, int kullaniciId, Dictionary<string, string> parametreler)
        {
            try
            {
                var sablon = await SablonGetirAsync(sablonKodu);
                if (sablon == null || !sablon.AktifMi)
                {
                    _logger.LogWarning($"Şablon bulunamadı veya aktif değil: {sablonKodu}");
                    return false;
                }

                var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);
                if (kullanici == null)
                {
                    _logger.LogWarning($"Kullanıcı bulunamadı: {kullaniciId}");
                    return false;
                }

                // Şablon içeriğini parametrelerle doldur
                var baslik = ParametreleriUygula(sablon.Konu, parametreler);
                var icerik = ParametreleriUygula(sablon.Icerik, parametreler);

                var bildirim = new Bildirim
                {
                    KullaniciId = kullaniciId,
                    Turu = sablon.Turu,
                    Baslik = baslik,
                    Icerik = icerik,
                    Email = sablon.Turu == BildirimTuru.Email ? kullanici.Email : null,
                    TelefonNo = sablon.Turu == BildirimTuru.SMS ? kullanici.TelefonNumarasi : null,
                    SablonKodu = sablonKodu,
                    Parametreler = JsonSerializer.Serialize(parametreler),
                    Durum = BildirimDurumu.Beklemede,
                    OlusturmaTarihi = DateTime.Now
                };

                _context.Bildirimler.Add(bildirim);
                await _context.SaveChangesAsync();

                await BildirimGonderAsync(bildirim.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Şablon ile bildirim gönderme hatası: {sablonKodu}");
                return false;
            }
        }

        public async Task<int> TopluEmailGonderAsync(List<int> kullaniciIdler, string baslik, string icerik)
        {
            int basariliSayisi = 0;

            foreach (var kullaniciId in kullaniciIdler)
            {
                var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);
                if (kullanici != null && !string.IsNullOrEmpty(kullanici.Email))
                {
                    if (await EmailGonderAsync(kullaniciId, kullanici.Email, baslik, icerik))
                    {
                        basariliSayisi++;
                    }
                }
            }

            return basariliSayisi;
        }

        public async Task<int> TumKullanicilariaBildirimGonderAsync(string baslik, string icerik, BildirimTuru tur)
        {
            var kullanicilar = await _context.Kullanicilar
                .Where(k => k.Aktif)
                .ToListAsync();

            int basariliSayisi = 0;

            foreach (var kullanici in kullanicilar)
            {
                bool sonuc = false;

                if (tur == BildirimTuru.Email && !string.IsNullOrEmpty(kullanici.Email))
                {
                    sonuc = await EmailGonderAsync(kullanici.Id, kullanici.Email, baslik, icerik);
                }
                else if (tur == BildirimTuru.SMS && !string.IsNullOrEmpty(kullanici.TelefonNumarasi))
                {
                    sonuc = await SmsGonderAsync(kullanici.Id, kullanici.TelefonNumarasi, icerik);
                }

                if (sonuc) basariliSayisi++;
            }

            return basariliSayisi;
        }

        public async Task<List<Bildirim>> BekleyenBildirimleriGetirAsync()
        {
            return await _context.Bildirimler
                .Where(b => b.Durum == BildirimDurumu.Beklemede)
                .OrderBy(b => b.OlusturmaTarihi)
                .Take(100)
                .ToListAsync();
        }

        public async Task<bool> BildirimGonderAsync(int bildirimId)
        {
            var bildirim = await _context.Bildirimler.FindAsync(bildirimId);
            if (bildirim == null || bildirim.Durum != BildirimDurumu.Beklemede)
            {
                return false;
            }

            try
            {
                bildirim.Durum = BildirimDurumu.Gonderiliyor;
                await _context.SaveChangesAsync();

                bool basarili = false;

                if (bildirim.Turu == BildirimTuru.Email && !string.IsNullOrEmpty(bildirim.Email))
                {
                    basarili = await _emailSender.SendEmailAsync(bildirim.Email, bildirim.Baslik, bildirim.Icerik);
                }
                else if (bildirim.Turu == BildirimTuru.SMS && !string.IsNullOrEmpty(bildirim.TelefonNo))
                {
                    basarili = await _smsSender.SendSmsAsync(bildirim.TelefonNo, bildirim.Icerik);
                }

                bildirim.Durum = basarili ? BildirimDurumu.Basarili : BildirimDurumu.Basarisiz;
                bildirim.GonderimTarihi = DateTime.Now;

                if (!basarili)
                {
                    bildirim.HataMesaji = "Gönderim başarısız oldu";
                }

                await _context.SaveChangesAsync();

                return basarili;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Bildirim gönderme hatası: {bildirimId}");
                
                bildirim.Durum = BildirimDurumu.Basarisiz;
                bildirim.HataMesaji = ex.Message;
                await _context.SaveChangesAsync();

                return false;
            }
        }

        public async Task<bool> BildirimIptalEtAsync(int bildirimId)
        {
            var bildirim = await _context.Bildirimler.FindAsync(bildirimId);
            if (bildirim == null)
            {
                return false;
            }

            if (bildirim.Durum == BildirimDurumu.Beklemede || bildirim.Durum == BildirimDurumu.Gonderiliyor)
            {
                bildirim.Durum = BildirimDurumu.IptalEdildi;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task BildirimleriIsleAsync()
        {
            var bekleyenBildirimler = await BekleyenBildirimleriGetirAsync();

            foreach (var bildirim in bekleyenBildirimler)
            {
                await BildirimGonderAsync(bildirim.Id);
                await Task.Delay(100); // Rate limiting
            }
        }

        public async Task<BildirimSablonu?> SablonGetirAsync(string kod)
        {
            return await _context.BildirimSablonlari
                .FirstOrDefaultAsync(s => s.Kod == kod && s.AktifMi);
        }

        public async Task<List<BildirimSablonu>> TumSablonlariGetirAsync()
        {
            return await _context.BildirimSablonlari
                .OrderBy(s => s.Ad)
                .ToListAsync();
        }

        public async Task<bool> SablonOlusturAsync(BildirimSablonu sablon)
        {
            try
            {
                sablon.OlusturmaTarihi = DateTime.Now;
                _context.BildirimSablonlari.Add(sablon);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şablon oluşturma hatası");
                return false;
            }
        }

        public async Task<bool> SablonGuncelleAsync(BildirimSablonu sablon)
        {
            try
            {
                sablon.GuncellenmeTarihi = DateTime.Now;
                _context.BildirimSablonlari.Update(sablon);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şablon güncelleme hatası");
                return false;
            }
        }

        public async Task<BildirimTercihi?> TercihleriGetirAsync(int kullaniciId)
        {
            var tercih = await _context.BildirimTercihleri
                .FirstOrDefaultAsync(t => t.KullaniciId == kullaniciId);

            // Eğer tercih yoksa varsayılan oluştur
            if (tercih == null)
            {
                tercih = new BildirimTercihi
                {
                    KullaniciId = kullaniciId,
                    EmailBildirimleri = true,
                    SmsBildirimleri = false,
                    SiparisBildirimleri = true,
                    KampanyaBildirimleri = true,
                    UrunBildirimleri = false,
                    IadeBildirimleri = true,
                    OlusturmaTarihi = DateTime.Now
                };

                _context.BildirimTercihleri.Add(tercih);
                await _context.SaveChangesAsync();
            }

            return tercih;
        }

        public async Task<bool> TercihleriGuncelleAsync(BildirimTercihi tercih)
        {
            try
            {
                tercih.GuncellenmeTarihi = DateTime.Now;
                _context.BildirimTercihleri.Update(tercih);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tercih güncelleme hatası");
                return false;
            }
        }

        public async Task<bool> BildirimGonderilsinMiAsync(int kullaniciId, string bildirimTipi)
        {
            var tercihler = await TercihleriGetirAsync(kullaniciId);
            if (tercihler == null) return true;

            return bildirimTipi.ToLower() switch
            {
                "siparis" => tercihler.SiparisBildirimleri,
                "kampanya" => tercihler.KampanyaBildirimleri,
                "urun" => tercihler.UrunBildirimleri,
                "iade" => tercihler.IadeBildirimleri,
                _ => true
            };
        }

        public async Task<Dictionary<string, int>> BildirimIstatistikleriGetirAsync(DateTime baslangic, DateTime bitis)
        {
            var bildirimler = await _context.Bildirimler
                .Where(b => b.OlusturmaTarihi >= baslangic && b.OlusturmaTarihi <= bitis)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                { "Toplam", bildirimler.Count },
                { "Beklemede", bildirimler.Count(b => b.Durum == BildirimDurumu.Beklemede) },
                { "Gonderiliyor", bildirimler.Count(b => b.Durum == BildirimDurumu.Gonderiliyor) },
                { "Basarili", bildirimler.Count(b => b.Durum == BildirimDurumu.Basarili) },
                { "Basarisiz", bildirimler.Count(b => b.Durum == BildirimDurumu.Basarisiz) },
                { "IptalEdildi", bildirimler.Count(b => b.Durum == BildirimDurumu.IptalEdildi) },
                { "Email", bildirimler.Count(b => b.Turu == BildirimTuru.Email) },
                { "SMS", bildirimler.Count(b => b.Turu == BildirimTuru.SMS) }
            };
        }

        public async Task<List<Bildirim>> KullaniciBildirimGecmisiAsync(int kullaniciId, int sayfa = 1, int sayfaBoyutu = 20)
        {
            return await _context.Bildirimler
                .Where(b => b.KullaniciId == kullaniciId)
                .OrderByDescending(b => b.OlusturmaTarihi)
                .Skip((sayfa - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .ToListAsync();
        }

        private string ParametreleriUygula(string icerik, Dictionary<string, string> parametreler)
        {
            foreach (var param in parametreler)
            {
                icerik = icerik.Replace($"{{{param.Key}}}", param.Value);
            }
            return icerik;
        }
    }
}
