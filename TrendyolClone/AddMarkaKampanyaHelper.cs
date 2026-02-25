using Microsoft.EntityFrameworkCore;
using TrendyolClone.Models;

namespace TrendyolClone
{
    public static class AddMarkaKampanyaHelper
    {
        public static void AddMarkaKampanyaTables(this ModelBuilder modelBuilder)
        {
            // Marka tablosu
            modelBuilder.Entity<Marka>(entity =>
            {
                entity.ToTable("Markalar");
                entity.HasKey(m => m.Id);
                entity.HasIndex(m => m.Slug).IsUnique();
                entity.Property(m => m.Ad).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Slug).HasMaxLength(200);
                entity.Property(m => m.Aciklama).HasMaxLength(500);
                entity.Property(m => m.LogoUrl).HasMaxLength(500);
                entity.Property(m => m.MetaTitle).HasMaxLength(200);
                entity.Property(m => m.MetaDescription).HasMaxLength(500);
                
                entity.HasMany(m => m.Urunler)
                    .WithOne(u => u.Marka)
                    .HasForeignKey(u => u.MarkaId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            
            // Kampanya tablosu
            modelBuilder.Entity<Kampanya>(entity =>
            {
                entity.ToTable("Kampanyalar");
                entity.HasKey(k => k.Id);
                entity.HasIndex(k => k.Slug).IsUnique();
                entity.Property(k => k.Ad).IsRequired().HasMaxLength(200);
                entity.Property(k => k.Slug).HasMaxLength(200);
                entity.Property(k => k.Aciklama).HasMaxLength(500);
                entity.Property(k => k.BannerUrl).HasMaxLength(500);
                entity.Property(k => k.IndirimMiktari).HasColumnType("decimal(18,2)");
                entity.Property(k => k.MaksimumIndirim).HasColumnType("decimal(18,2)");
                entity.Property(k => k.MinimumSepetTutari).HasColumnType("decimal(18,2)");
                entity.Property(k => k.ToplamIndirimTutari).HasColumnType("decimal(18,2)");
                
                entity.HasMany(k => k.UrunKampanyalari)
                    .WithOne(uk => uk.Kampanya)
                    .HasForeignKey(uk => uk.KampanyaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // UrunKampanya ilişki tablosu
            modelBuilder.Entity<UrunKampanya>(entity =>
            {
                entity.ToTable("UrunKampanyalari");
                entity.HasKey(uk => uk.Id);
                entity.Property(uk => uk.OzelIndirimMiktari).HasColumnType("decimal(18,2)");
                
                entity.HasOne(uk => uk.Urun)
                    .WithMany(u => u.Kampanyalar)
                    .HasForeignKey(uk => uk.UrunId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                // Bir ürün aynı kampanyaya birden fazla eklenemez
                entity.HasIndex(uk => new { uk.UrunId, uk.KampanyaId }).IsUnique();
            });
            
            // Urun tablosu güncellemeleri
            modelBuilder.Entity<Urun>(entity =>
            {
                entity.Property(u => u.Slug).HasMaxLength(200);
                entity.Property(u => u.MetaTitle).HasMaxLength(200);
                entity.Property(u => u.MetaDescription).HasMaxLength(500);
                entity.Property(u => u.CanonicalUrl).HasMaxLength(500);
                entity.Property(u => u.MarkaAdi).HasMaxLength(100);
                entity.Property(u => u.PopuleriteSkor).HasColumnType("decimal(18,2)");
                
                entity.HasIndex(u => u.Slug).IsUnique();
            });
        }
    }
}
