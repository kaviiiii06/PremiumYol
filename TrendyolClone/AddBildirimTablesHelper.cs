using Microsoft.EntityFrameworkCore;
using TrendyolClone.Models;

namespace TrendyolClone.Data
{
    public static class BildirimTablesExtensions
    {
        public static void AddBildirimTables(this ModelBuilder modelBuilder)
        {
            // Bildirim - Kullanici ilişkisi
            modelBuilder.Entity<Bildirim>()
                .HasOne(b => b.Kullanici)
                .WithMany()
                .HasForeignKey(b => b.KullaniciId)
                .OnDelete(DeleteBehavior.SetNull);

            // BildirimTercihi - Kullanici ilişkisi
            modelBuilder.Entity<BildirimTercihi>()
                .HasOne(bt => bt.Kullanici)
                .WithMany()
                .HasForeignKey(bt => bt.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            // BildirimTercihi - Unique constraint
            modelBuilder.Entity<BildirimTercihi>()
                .HasIndex(bt => bt.KullaniciId)
                .IsUnique();

            // BildirimSablonu - Kod unique
            modelBuilder.Entity<BildirimSablonu>()
                .HasIndex(bs => bs.Kod)
                .IsUnique();

            // Bildirim indeksleri
            modelBuilder.Entity<Bildirim>()
                .HasIndex(b => b.Durum);

            modelBuilder.Entity<Bildirim>()
                .HasIndex(b => b.OlusturmaTarihi);

            modelBuilder.Entity<Bildirim>()
                .HasIndex(b => new { b.KullaniciId, b.OlusturmaTarihi });
        }
    }
}
