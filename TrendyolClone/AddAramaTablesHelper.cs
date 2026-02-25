using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TrendyolClone.Data;

namespace TrendyolClone
{
    public static class AddAramaTablesHelper
    {
        public static void AddAramaTables(UygulamaDbContext context)
        {
            var connectionString = context.Database.GetDbConnection().ConnectionString;
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // AramaGecmisleri tablosu
                var createAramaGecmisiTable = @"
                    CREATE TABLE IF NOT EXISTS AramaGecmisleri (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        KullaniciId INTEGER NOT NULL,
                        Terim TEXT NOT NULL,
                        AramaTarihi TEXT NOT NULL,
                        FOREIGN KEY (KullaniciId) REFERENCES Kullanicilar(Id) ON DELETE CASCADE
                    );
                    CREATE INDEX IF NOT EXISTS IX_AramaGecmisleri_KullaniciId ON AramaGecmisleri(KullaniciId);
                    CREATE INDEX IF NOT EXISTS IX_AramaGecmisleri_AramaTarihi ON AramaGecmisleri(AramaTarihi);
                ";

                // PopulerAramalar tablosu
                var createPopulerAramaTable = @"
                    CREATE TABLE IF NOT EXISTS PopulerAramalar (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Terim TEXT NOT NULL UNIQUE,
                        AramaSayisi INTEGER NOT NULL DEFAULT 0,
                        SonAramaTarihi TEXT NOT NULL
                    );
                    CREATE INDEX IF NOT EXISTS IX_PopulerAramalar_AramaSayisi ON PopulerAramalar(AramaSayisi);
                ";

                // AramaTiklamalari tablosu
                var createAramaTiklamaTable = @"
                    CREATE TABLE IF NOT EXISTS AramaTiklamalari (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Terim TEXT NOT NULL,
                        UrunId INTEGER NOT NULL,
                        KullaniciId INTEGER,
                        TiklamaTarihi TEXT NOT NULL,
                        FOREIGN KEY (UrunId) REFERENCES Urunler(Id) ON DELETE CASCADE,
                        FOREIGN KEY (KullaniciId) REFERENCES Kullanicilar(Id) ON DELETE SET NULL
                    );
                    CREATE INDEX IF NOT EXISTS IX_AramaTiklamalari_Terim ON AramaTiklamalari(Terim);
                    CREATE INDEX IF NOT EXISTS IX_AramaTiklamalari_UrunId ON AramaTiklamalari(UrunId);
                ";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = createAramaGecmisiTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createPopulerAramaTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createAramaTiklamaTable;
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("Arama tabloları başarıyla oluşturuldu.");
            }
        }
    }
}
