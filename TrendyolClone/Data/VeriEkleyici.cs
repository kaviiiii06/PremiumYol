using TrendyolClone.Models;

namespace TrendyolClone.Data
{
    public static class VeriEkleyici
    {
        public static void VeriEkle(UygulamaDbContext context)
        {
            // Rolleri ekle
            if (!context.Roller.Any())
            {
                var roller = new List<Rol>
                {
                    new Rol { Ad = "Müşteri", Aciklama = "Standart müşteri hesabı", Aktif = true },
                    new Rol { Ad = "Premium Müşteri", Aciklama = "Premium üyelik avantajları", Aktif = true },
                    new Rol { Ad = "VIP Müşteri", Aciklama = "VIP müşteri özel ayrıcalıkları", Aktif = true },
                    new Rol { Ad = "Satış Temsilcisi", Aciklama = "Satış işlemleri yetkisi", Aktif = true },
                    new Rol { Ad = "Moderatör", Aciklama = "İçerik moderasyon yetkisi", Aktif = true }
                };

                context.Roller.AddRange(roller);
                context.SaveChanges();
            }

            // Admin hesabı ekle (sadece production için)
            if (!context.Yoneticiler.Any())
            {
                var admin = new Yonetici
                {
                    KullaniciAdi = "baranAdmin2025",
                    Sifre = BCrypt.Net.BCrypt.HashPassword("Baran@2025!Secure"),
                    Email = "admin@premiumyol.com",
                    OlusturmaTarihi = DateTime.Now,
                    Aktif = true
                };

                context.Yoneticiler.Add(admin);
                context.SaveChanges();
            }

            // Site ayarları ekle
            if (!context.SiteAyarlari.Any())
            {
                var siteAyari = new SiteAyarlari
                {
                    SiteAdi = "PremiumYol",
                    SiteAciklamasi = "Premium E-Ticaret Platformu",
                    LogoIcon = "fas fa-crown",
                    IletisimEmail = "baran@onewearr.shop",
                    IletisimTelefon = "0538 969 36 06",
                    AltBilgiMetni = "© 2024 PremiumYol. Tüm hakları saklıdır.",
                    GuncellemeTarihi = DateTime.Now,
                    Aktif = true
                };

                context.SiteAyarlari.Add(siteAyari);
                context.SaveChanges();
            }

            // Kategoriler ekle
            if (!context.Kategoriler.Any())
            {
                var kategoriler = new List<Kategori>
                {
                    new Kategori { Ad = "Elektronik", Aciklama = "Telefon, bilgisayar ve elektronik ürünler" },
                    new Kategori { Ad = "Moda", Aciklama = "Giyim ve aksesuar ürünleri" },
                    new Kategori { Ad = "Ev & Yaşam", Aciklama = "Ev dekorasyonu ve yaşam ürünleri" },
                    new Kategori { Ad = "Spor", Aciklama = "Spor giyim ve ekipmanları" },
                    new Kategori { Ad = "Kitap", Aciklama = "Kitap ve kırtasiye ürünleri" },
                    new Kategori { Ad = "Kozmetik", Aciklama = "Güzellik ve kişisel bakım ürünleri" }
                };

                context.Kategoriler.AddRange(kategoriler);
                context.SaveChanges();
            }
        }
    }
}
