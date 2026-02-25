using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;

namespace TrendyolClone
{
    public static class AddUrunVaryasyonHelper
    {
        public static void AddUrunVaryasyonTables(UygulamaDbContext context)
        {
            try
            {
                var connection = context.Database.GetDbConnection();
                connection.Open();

                using var command = connection.CreateCommand();
                
                // UrunVaryasyonlari tablosu
                command.CommandText = @"
                    SELECT COUNT(*) FROM sqlite_master 
                    WHERE type='table' AND name='UrunVaryasyonlari';";
                
                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                {
                    Console.WriteLine("UrunVaryasyonlari tablosu oluşturuluyor...");
                    
                    command.CommandText = @"
                        CREATE TABLE UrunVaryasyonlari (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            UrunId INTEGER NOT NULL,
                            SKU TEXT NOT NULL,
                            Renk TEXT,
                            Beden TEXT,
                            DigerOzellik TEXT,
                            Fiyat REAL NOT NULL,
                            IndirimliFiyat REAL,
                            Stok INTEGER NOT NULL,
                            MinStok INTEGER NOT NULL DEFAULT 5,
                            AnaResim TEXT,
                            Aktif INTEGER NOT NULL DEFAULT 1,
                            Sira INTEGER NOT NULL DEFAULT 0,
                            OlusturmaTarihi TEXT NOT NULL,
                            FOREIGN KEY (UrunId) REFERENCES Urunler(Id) ON DELETE CASCADE
                        );";
                    command.ExecuteNonQuery();
                    
                    command.CommandText = "CREATE UNIQUE INDEX IX_UrunVaryasyonlari_SKU ON UrunVaryasyonlari (SKU);";
                    command.ExecuteNonQuery();
                    
                    Console.WriteLine("✓ UrunVaryasyonlari tablosu oluşturuldu!");
                }
                
                // UrunResimleri tablosu
                command.CommandText = @"
                    SELECT COUNT(*) FROM sqlite_master 
                    WHERE type='table' AND name='UrunResimleri';";
                
                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                {
                    Console.WriteLine("UrunResimleri tablosu oluşturuluyor...");
                    
                    command.CommandText = @"
                        CREATE TABLE UrunResimleri (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            UrunId INTEGER NOT NULL,
                            VaryasyonId INTEGER,
                            ResimUrl TEXT NOT NULL,
                            ThumbnailUrl TEXT,
                            Sira INTEGER NOT NULL,
                            AnaResim INTEGER NOT NULL,
                            YuklemeTarihi TEXT NOT NULL,
                            FOREIGN KEY (UrunId) REFERENCES Urunler(Id) ON DELETE CASCADE,
                            FOREIGN KEY (VaryasyonId) REFERENCES UrunVaryasyonlari(Id) ON DELETE SET NULL
                        );";
                    command.ExecuteNonQuery();
                    
                    Console.WriteLine("✓ UrunResimleri tablosu oluşturuldu!");
                }
                
                // UrunOzellikleri tablosu
                command.CommandText = @"
                    SELECT COUNT(*) FROM sqlite_master 
                    WHERE type='table' AND name='UrunOzellikleri';";
                
                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                {
                    Console.WriteLine("UrunOzellikleri tablosu oluşturuluyor...");
                    
                    command.CommandText = @"
                        CREATE TABLE UrunOzellikleri (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            UrunId INTEGER NOT NULL,
                            OzellikAdi TEXT NOT NULL,
                            OzellikDegeri TEXT NOT NULL,
                            Sira INTEGER NOT NULL,
                            OlusturmaTarihi TEXT NOT NULL,
                            FOREIGN KEY (UrunId) REFERENCES Urunler(Id) ON DELETE CASCADE
                        );";
                    command.ExecuteNonQuery();
                    
                    Console.WriteLine("✓ UrunOzellikleri tablosu oluşturuldu!");
                }
                
                // KargoOlculeri tablosu
                command.CommandText = @"
                    SELECT COUNT(*) FROM sqlite_master 
                    WHERE type='table' AND name='KargoOlculeri';";
                
                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                {
                    Console.WriteLine("KargoOlculeri tablosu oluşturuluyor...");
                    
                    command.CommandText = @"
                        CREATE TABLE KargoOlculeri (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            UrunId INTEGER NOT NULL,
                            En REAL NOT NULL,
                            Boy REAL NOT NULL,
                            Yukseklik REAL NOT NULL,
                            Agirlik REAL NOT NULL,
                            UcretsizKargo INTEGER NOT NULL,
                            KargoUcreti REAL,
                            OlusturmaTarihi TEXT NOT NULL,
                            GuncellenmeTarihi TEXT,
                            FOREIGN KEY (UrunId) REFERENCES Urunler(Id) ON DELETE CASCADE
                        );";
                    command.ExecuteNonQuery();
                    
                    Console.WriteLine("✓ KargoOlculeri tablosu oluşturuldu!");
                }
                
                connection.Close();
                Console.WriteLine("\n✅ Tüm ürün varyasyon tabloları başarıyla oluşturuldu!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Hata: {ex.Message}");
            }
        }
    }
}
