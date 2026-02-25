using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;

namespace TrendyolClone
{
    public static class AddKuponKargoHelper
    {
        public static void AddKuponKargoTables(this ModelBuilder modelBuilder)
        {
            // Kupon tablosu
            modelBuilder.Entity<Kupon>(entity =>
            {
                entity.ToTable("Kuponlar");
                entity.HasKey(k => k.Id);
                entity.HasIndex(k => k.Kod).IsUnique();
                entity.Property(k => k.Kod).IsRequired().HasMaxLength(50);
                entity.Property(k => k.Aciklama).IsRequired().HasMaxLength(500);
                entity.Property(k => k.IndirimMiktari).HasColumnType("decimal(18,2)");
                entity.Property(k => k.MaksimumIndirim).HasColumnType("decimal(18,2)");
                entity.Property(k => k.MinimumSepetTutari).HasColumnType("decimal(18,2)");
                
                entity.HasMany(k => k.Kullanimlar)
                    .WithOne(ku => ku.Kupon)
                    .HasForeignKey(ku => ku.KuponId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasMany(k => k.GecerliKategoriler)
                    .WithOne(kk => kk.Kupon)
                    .HasForeignKey(kk => kk.KuponId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // KuponKullanimi tablosu
            modelBuilder.Entity<KuponKullanimi>(entity =>
            {
                entity.ToTable("KuponKullanimlari");
                entity.HasKey(ku => ku.Id);
                entity.Property(ku => ku.IndirimTutari).HasColumnType("decimal(18,2)");
                entity.Property(ku => ku.SepetTutari).HasColumnType("decimal(18,2)");
                entity.Property(ku => ku.IpAdresi).HasMaxLength(50);
                
                entity.HasOne(ku => ku.Kullanici)
                    .WithMany()
                    .HasForeignKey(ku => ku.KullaniciId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(ku => ku.Siparis)
                    .WithMany()
                    .HasForeignKey(ku => ku.SiparisId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            
            // KuponKategori tablosu
            modelBuilder.Entity<KuponKategori>(entity =>
            {
                entity.ToTable("KuponKategoriler");
                entity.HasKey(kk => kk.Id);
                
                entity.HasOne(kk => kk.Kategori)
                    .WithMany()
                    .HasForeignKey(kk => kk.KategoriId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // KargoFirma tablosu
            modelBuilder.Entity<KargoFirma>(entity =>
            {
                entity.ToTable("KargoFirmalari");
                entity.HasKey(kf => kf.Id);
                entity.Property(kf => kf.Ad).IsRequired().HasMaxLength(100);
                entity.Property(kf => kf.Logo).HasMaxLength(500);
                entity.Property(kf => kf.ApiUrl).HasMaxLength(500);
                entity.Property(kf => kf.ApiKey).HasMaxLength(200);
                entity.Property(kf => kf.ApiSecret).HasMaxLength(200);
                entity.Property(kf => kf.TemelUcret).HasColumnType("decimal(18,2)");
                entity.Property(kf => kf.KgBasinaUcret).HasColumnType("decimal(18,2)");
                entity.Property(kf => kf.DesiBasinaUcret).HasColumnType("decimal(18,2)");
                entity.Property(kf => kf.UcretsizKargoLimiti).HasColumnType("decimal(18,2)");
                
                entity.HasMany(kf => kf.Ucretler)
                    .WithOne(ku => ku.KargoFirma)
                    .HasForeignKey(ku => ku.KargoFirmaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // KargoUcret tablosu
            modelBuilder.Entity<KargoUcret>(entity =>
            {
                entity.ToTable("KargoUcretleri");
                entity.HasKey(ku => ku.Id);
                entity.Property(ku => ku.CikisIli).IsRequired().HasMaxLength(50);
                entity.Property(ku => ku.VarisIli).IsRequired().HasMaxLength(50);
                entity.Property(ku => ku.Ucret).HasColumnType("decimal(18,2)");
                
                entity.HasIndex(ku => new { ku.KargoFirmaId, ku.CikisIli, ku.VarisIli });
            });
            
            // KayitliSepet tablosu
            modelBuilder.Entity<KayitliSepet>(entity =>
            {
                entity.ToTable("KayitliSepetler");
                entity.HasKey(ks => ks.Id);
                entity.Property(ks => ks.Ad).IsRequired().HasMaxLength(100);
                entity.Property(ks => ks.Aciklama).HasMaxLength(500);
                
                entity.HasOne(ks => ks.Kullanici)
                    .WithMany()
                    .HasForeignKey(ks => ks.KullaniciId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasMany(ks => ks.Urunler)
                    .WithOne(ksu => ksu.KayitliSepet)
                    .HasForeignKey(ksu => ksu.KayitliSepetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // KayitliSepetUrunu tablosu
            modelBuilder.Entity<KayitliSepetUrunu>(entity =>
            {
                entity.ToTable("KayitliSepetUrunleri");
                entity.HasKey(ksu => ksu.Id);
                entity.Property(ksu => ksu.BirimFiyat).HasColumnType("decimal(18,2)");
                
                entity.HasOne(ksu => ksu.Urun)
                    .WithMany()
                    .HasForeignKey(ksu => ksu.UrunId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(ksu => ksu.Varyasyon)
                    .WithMany()
                    .HasForeignKey(ksu => ksu.VaryasyonId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
