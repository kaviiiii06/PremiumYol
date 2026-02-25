using Microsoft.EntityFrameworkCore;
using TrendyolClone.Models;

namespace TrendyolClone.Data
{
    public static class SaticiModelBuilderExtensions
    {
        public static void AddSaticiTables(this ModelBuilder modelBuilder)
        {
            // Satici - Kullanici ilişkisi
            modelBuilder.Entity<Satici>()
                .HasOne(s => s.Kullanici)
                .WithMany()
                .HasForeignKey(s => s.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Satici - Slug unique
            modelBuilder.Entity<Satici>()
                .HasIndex(s => s.Slug)
                .IsUnique();
            
            // SaticiUrun - Satici ilişkisi
            modelBuilder.Entity<SaticiUrun>()
                .HasOne(su => su.Satici)
                .WithMany(s => s.Urunler)
                .HasForeignKey(su => su.SaticiId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // SaticiUrun - Urun ilişkisi
            modelBuilder.Entity<SaticiUrun>()
                .HasOne(su => su.Urun)
                .WithMany()
                .HasForeignKey(su => su.UrunId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // SaticiSiparis - Satici ilişkisi
            modelBuilder.Entity<SaticiSiparis>()
                .HasOne(ss => ss.Satici)
                .WithMany(s => s.Siparisler)
                .HasForeignKey(ss => ss.SaticiId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // SaticiSiparis - Siparis ilişkisi
            modelBuilder.Entity<SaticiSiparis>()
                .HasOne(ss => ss.Siparis)
                .WithMany()
                .HasForeignKey(ss => ss.SiparisId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // SaticiSiparis - Urun ilişkisi
            modelBuilder.Entity<SaticiSiparis>()
                .HasOne(ss => ss.Urun)
                .WithMany()
                .HasForeignKey(ss => ss.UrunId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // SaticiOdeme - Satici ilişkisi
            modelBuilder.Entity<SaticiOdeme>()
                .HasOne(so => so.Satici)
                .WithMany(s => s.Odemeler)
                .HasForeignKey(so => so.SaticiId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // SaticiDegerlendirme - Satici ilişkisi
            modelBuilder.Entity<SaticiDegerlendirme>()
                .HasOne(sd => sd.Satici)
                .WithMany(s => s.Degerlendirmeler)
                .HasForeignKey(sd => sd.SaticiId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // SaticiDegerlendirme - Kullanici ilişkisi
            modelBuilder.Entity<SaticiDegerlendirme>()
                .HasOne(sd => sd.Kullanici)
                .WithMany()
                .HasForeignKey(sd => sd.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // SaticiDegerlendirme - Siparis ilişkisi
            modelBuilder.Entity<SaticiDegerlendirme>()
                .HasOne(sd => sd.Siparis)
                .WithMany()
                .HasForeignKey(sd => sd.SiparisId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Decimal precision ayarları
            modelBuilder.Entity<Satici>()
                .Property(s => s.KomisyonOrani)
                .HasPrecision(5, 2);
            
            modelBuilder.Entity<Satici>()
                .Property(s => s.ToplamKazanc)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<Satici>()
                .Property(s => s.OrtalamaPuan)
                .HasPrecision(3, 2);
            
            modelBuilder.Entity<SaticiUrun>()
                .Property(su => su.Fiyat)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<SaticiUrun>()
                .Property(su => su.IndirimliFiyat)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<SaticiUrun>()
                .Property(su => su.ToplamSatisTutari)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<SaticiSiparis>()
                .Property(ss => ss.BirimFiyat)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<SaticiSiparis>()
                .Property(ss => ss.ToplamTutar)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<SaticiSiparis>()
                .Property(ss => ss.KomisyonOrani)
                .HasPrecision(5, 2);
            
            modelBuilder.Entity<SaticiSiparis>()
                .Property(ss => ss.KomisyonTutari)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<SaticiSiparis>()
                .Property(ss => ss.SaticiKazanci)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<SaticiOdeme>()
                .Property(so => so.Tutar)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<SaticiOdeme>()
                .Property(so => so.KomisyonTutari)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<SaticiOdeme>()
                .Property(so => so.NetTutar)
                .HasPrecision(18, 2);
        }
    }
}
