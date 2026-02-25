using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;

namespace TrendyolClone
{
    public static class AddSepetTableHelper
    {
        public static void AddSepetUrunleriTable(UygulamaDbContext context)
        {
            try
            {
                var connection = context.Database.GetDbConnection();
                connection.Open();

                using var command = connection.CreateCommand();
                
                // Tablo var mı kontrol et
                command.CommandText = @"
                    SELECT COUNT(*) 
                    FROM sqlite_master 
                    WHERE type='table' AND name='SepetUrunleri';";
                
                var exists = Convert.ToInt32(command.ExecuteScalar()) > 0;
                
                if (!exists)
                {
                    Console.WriteLine("SepetUrunleri tablosu oluşturuluyor...");
                    
                    command.CommandText = @"
                        CREATE TABLE SepetUrunleri (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            KullaniciId INTEGER NOT NULL,
                            UrunId INTEGER NOT NULL,
                            Adet INTEGER NOT NULL,
                            EklenmeTarihi TEXT NOT NULL,
                            FOREIGN KEY (KullaniciId) REFERENCES Kullanicilar(Id) ON DELETE CASCADE,
                            FOREIGN KEY (UrunId) REFERENCES Urunler(Id) ON DELETE CASCADE
                        );";
                    command.ExecuteNonQuery();
                    
                    // Unique index ekle
                    command.CommandText = @"
                        CREATE UNIQUE INDEX IX_SepetUrunleri_KullaniciId_UrunId 
                        ON SepetUrunleri (KullaniciId, UrunId);";
                    command.ExecuteNonQuery();
                    
                    // UrunId için index ekle
                    command.CommandText = @"
                        CREATE INDEX IX_SepetUrunleri_UrunId 
                        ON SepetUrunleri (UrunId);";
                    command.ExecuteNonQuery();
                    
                    Console.WriteLine("✓ SepetUrunleri tablosu başarıyla oluşturuldu!");
                }
                else
                {
                    Console.WriteLine("SepetUrunleri tablosu zaten mevcut.");
                }
                
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }
    }
}
