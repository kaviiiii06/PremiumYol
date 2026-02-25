using Microsoft.EntityFrameworkCore;
using TrendyolClone.Models;

namespace TrendyolClone.Data
{
    public class UygulamaDbContext : DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options)
        {
        }
        
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisKalemi> SiparisKalemleri { get; set; }
        public DbSet<Adres> Adresler { get; set; }
        public DbSet<Yonetici> Yoneticiler { get; set; }
        public DbSet<Yorum> Yorumlar { get; set; }
        public DbSet<Rol> Roller { get; set; }
        public DbSet<Tedarikci> Tedarikciler { get; set; }
        public DbSet<SiteAyarlari> SiteAyarlari { get; set; }
        public DbSet<Favori> Favoriler { get; set; }
        public DbSet<UrunSoru> UrunSorulari { get; set; }
        public DbSet<UrunCevap> UrunCevaplari { get; set; }
        public DbSet<IslemKaydi> IslemKayitlari { get; set; }
        public DbSet<OdemeIslemi> OdemeIslemleri { get; set; }
        public DbSet<OdemeYontemiAyari> OdemeYontemleri { get; set; }
        public DbSet<SepetUrunu> SepetUrunleri { get; set; }
        
        // Yeni: Ürün Varyasyon Sistemi
        public DbSet<UrunVaryasyon> UrunVaryasyonlari { get; set; }
        public DbSet<UrunResim> UrunResimleri { get; set; }
        public DbSet<UrunOzellik> UrunOzellikleri { get; set; }
        public DbSet<KargoOlculeri> KargoOlculeri { get; set; }
        
        // Yeni: Kupon ve Kargo Sistemi
        public DbSet<Kupon> Kuponlar { get; set; }
        public DbSet<KuponKullanimi> KuponKullanimlari { get; set; }
        public DbSet<KuponKategori> KuponKategoriler { get; set; }
        public DbSet<KargoFirma> KargoFirmalari { get; set; }
        public DbSet<KargoUcret> KargoUcretleri { get; set; }
        public DbSet<KayitliSepet> KayitliSepetler { get; set; }
        public DbSet<KayitliSepetUrunu> KayitliSepetUrunleri { get; set; }
        
        // Yeni: Marka ve Kampanya Sistemi
        public DbSet<Marka> Markalar { get; set; }
        public DbSet<Kampanya> Kampanyalar { get; set; }
        public DbSet<UrunKampanya> UrunKampanyalari { get; set; }
        
        // Yeni: Arama Motoru
        public DbSet<AramaGecmisi> AramaGecmisleri { get; set; }
        public DbSet<PopulerArama> PopulerAramalar { get; set; }
        public DbSet<AramaTiklama> AramaTiklamalari { get; set; }
        
        // Yeni: Gelişmiş Sipariş Sistemi
        public DbSet<SiparisDurumGecmisi> SiparisDurumGecmisleri { get; set; }
        public DbSet<KargoTakip> KargoTakipler { get; set; }
        public DbSet<KargoHareket> KargoHareketler { get; set; }
        public DbSet<Fatura> Faturalar { get; set; }
        public DbSet<Iade> Iadeler { get; set; }
        public DbSet<IadeUrunu> IadeUrunleri { get; set; }
        
        // Yeni: Bildirim Sistemi
        public DbSet<Bildirim> Bildirimler { get; set; }
        public DbSet<BildirimSablonu> BildirimSablonlari { get; set; }
        public DbSet<BildirimTercihi> BildirimTercihleri { get; set; }
        
        // Yeni: Satıcı Paneli
        public DbSet<Satici> Saticilar { get; set; }
        public DbSet<SaticiUrun> SaticiUrunler { get; set; }
        public DbSet<SaticiSiparis> SaticiSiparisler { get; set; }
        public DbSet<SaticiOdeme> SaticiOdemeler { get; set; }
        public DbSet<SaticiDegerlendirme> SaticiDegerlendirmeler { get; set; }
        
        // Yeni: Yorum & Değerlendirme Sistemi
        public DbSet<UrunYorum> UrunYorumlari { get; set; }
        public DbSet<YorumResim> YorumResimleri { get; set; }
        public DbSet<YorumVideo> YorumVideolari { get; set; }
        public DbSet<YorumEtkilesim> YorumEtkilesimleri { get; set; }
        public DbSet<SaticiYorumYaniti> SaticiYorumYanitlari { get; set; }
        public DbSet<YorumRapor> YorumRaporlari { get; set; }
        
        // Yeni: SEO & Marketing
        public DbSet<SeoAyarlari> SeoAyarlari { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Urun - Kategori ilişkisi
            modelBuilder.Entity<Urun>()
                .HasOne(p => p.Kategori)
                .WithMany(c => c.Urunler)
                .HasForeignKey(p => p.KategoriId);
                
            // Siparis - Kullanici ilişkisi
            modelBuilder.Entity<Siparis>()
                .HasOne(o => o.Kullanici)
                .WithMany(u => u.Siparisler)
                .HasForeignKey(o => o.KullaniciId);
                
            // SiparisKalemi - Siparis ilişkisi
            modelBuilder.Entity<SiparisKalemi>()
                .HasOne(oi => oi.Siparis)
                .WithMany(o => o.SiparisKalemleri)
                .HasForeignKey(oi => oi.SiparisId);
                
            // SiparisKalemi - Urun ilişkisi
            modelBuilder.Entity<SiparisKalemi>()
                .HasOne(oi => oi.Urun)
                .WithMany()
                .HasForeignKey(oi => oi.UrunId);
                
            // Yorum - Urun ilişkisi
            modelBuilder.Entity<Yorum>()
                .HasOne(r => r.Urun)
                .WithMany()
                .HasForeignKey(r => r.UrunId);
                
            // Yorum - Kullanici ilişkisi
            modelBuilder.Entity<Yorum>()
                .HasOne(r => r.Kullanici)
                .WithMany()
                .HasForeignKey(r => r.KullaniciId);
                
            // Kullanici - Rol ilişkisi
            modelBuilder.Entity<Kullanici>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Kullanicilar)
                .HasForeignKey(u => u.RolId);
                
            // Urun - Tedarikci ilişkisi
            modelBuilder.Entity<Urun>()
                .HasOne(p => p.Tedarikci)
                .WithMany(s => s.Urunler)
                .HasForeignKey(p => p.TedarikciId);
                
            // Favori - Kullanici ilişkisi
            modelBuilder.Entity<Favori>()
                .HasOne(f => f.Kullanici)
                .WithMany()
                .HasForeignKey(f => f.KullaniciId);
                
            // Favori - Urun ilişkisi
            modelBuilder.Entity<Favori>()
                .HasOne(f => f.Urun)
                .WithMany()
                .HasForeignKey(f => f.UrunId);
                
            // Favori - Unique constraint
            modelBuilder.Entity<Favori>()
                .HasIndex(f => new { f.KullaniciId, f.UrunId })
                .IsUnique();
                
            // UrunSoru - Urun ilişkisi
            modelBuilder.Entity<UrunSoru>()
                .HasOne(q => q.Urun)
                .WithMany()
                .HasForeignKey(q => q.UrunId);
                
            // UrunSoru - Kullanici ilişkisi
            modelBuilder.Entity<UrunSoru>()
                .HasOne(q => q.Kullanici)
                .WithMany()
                .HasForeignKey(q => q.KullaniciId);
                
            // UrunCevap - Soru ilişkisi
            modelBuilder.Entity<UrunCevap>()
                .HasOne(a => a.Soru)
                .WithMany(q => q.Cevaplar)
                .HasForeignKey(a => a.SoruId);
                
            // UrunCevap - Kullanici ilişkisi (opsiyonel)
            modelBuilder.Entity<UrunCevap>()
                .HasOne(a => a.Kullanici)
                .WithMany()
                .HasForeignKey(a => a.KullaniciId)
                .IsRequired(false);
                
            // UrunCevap - Yonetici ilişkisi (opsiyonel)
            modelBuilder.Entity<UrunCevap>()
                .HasOne(a => a.Yonetici)
                .WithMany()
                .HasForeignKey(a => a.YoneticiId)
                .IsRequired(false);
                
            // Decimal precision ayarları
            modelBuilder.Entity<Urun>()
                .Property(p => p.Fiyat)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<Urun>()
                .Property(p => p.IndirimliFiyat)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<Siparis>()
                .Property(o => o.ToplamTutar)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<SiparisKalemi>()
                .Property(oi => oi.BirimFiyat)
                .HasPrecision(18, 2);
                
            // OdemeIslemi - Siparis ilişkisi
            modelBuilder.Entity<OdemeIslemi>()
                .HasOne(pt => pt.Siparis)
                .WithMany()
                .HasForeignKey(pt => pt.SiparisId);
                
            modelBuilder.Entity<OdemeIslemi>()
                .Property(pt => pt.Tutar)
                .HasPrecision(18, 2);
                
            // Adres - Kullanici ilişkisi
            modelBuilder.Entity<Adres>()
                .HasOne(a => a.Kullanici)
                .WithMany()
                .HasForeignKey(a => a.KullaniciId);
                
            // SepetUrunu - Kullanici ilişkisi
            modelBuilder.Entity<SepetUrunu>()
                .HasOne(c => c.Kullanici)
                .WithMany()
                .HasForeignKey(c => c.KullaniciId);
                
            // SepetUrunu - Urun ilişkisi
            modelBuilder.Entity<SepetUrunu>()
                .HasOne(c => c.Urun)
                .WithMany()
                .HasForeignKey(c => c.UrunId);
                
            // SepetUrunu - Unique constraint (bir kullanıcı aynı ürünü birden fazla kez ekleyemez)
            modelBuilder.Entity<SepetUrunu>()
                .HasIndex(c => new { c.KullaniciId, c.UrunId })
                .IsUnique();
                
            // ===== YENİ: Ürün Varyasyon Sistemi İlişkileri =====
            
            // UrunVaryasyon - Urun ilişkisi
            modelBuilder.Entity<UrunVaryasyon>()
                .HasOne(v => v.Urun)
                .WithMany()
                .HasForeignKey(v => v.UrunId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // UrunVaryasyon - SKU unique
            modelBuilder.Entity<UrunVaryasyon>()
                .HasIndex(v => v.SKU)
                .IsUnique();
                
            // UrunResim - Urun ilişkisi
            modelBuilder.Entity<UrunResim>()
                .HasOne(r => r.Urun)
                .WithMany()
                .HasForeignKey(r => r.UrunId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // UrunResim - Varyasyon ilişkisi (opsiyonel)
            modelBuilder.Entity<UrunResim>()
                .HasOne(r => r.Varyasyon)
                .WithMany(v => v.Resimler)
                .HasForeignKey(r => r.VaryasyonId)
                .OnDelete(DeleteBehavior.SetNull);
                
            // UrunOzellik - Urun ilişkisi
            modelBuilder.Entity<UrunOzellik>()
                .HasOne(o => o.Urun)
                .WithMany()
                .HasForeignKey(o => o.UrunId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // KargoOlculeri - Urun ilişkisi (1-1)
            modelBuilder.Entity<KargoOlculeri>()
                .HasOne(k => k.Urun)
                .WithOne()
                .HasForeignKey<KargoOlculeri>(k => k.UrunId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // ===== YENİ: Kupon ve Kargo Sistemi İlişkileri =====
            modelBuilder.AddKuponKargoTables();
            
            // ===== YENİ: Marka ve Kampanya Sistemi İlişkileri =====
            modelBuilder.AddMarkaKampanyaTables();
            
            // ===== YENİ: Bildirim Sistemi İlişkileri =====
            modelBuilder.AddBildirimTables();
            
            // ===== YENİ: Satıcı Paneli İlişkileri =====
            modelBuilder.AddSaticiTables();
        }
    }
}
