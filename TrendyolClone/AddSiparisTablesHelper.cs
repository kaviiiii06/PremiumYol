using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TrendyolClone.Data;

namespace TrendyolClone
{
    public static class AddSiparisTablesHelper
    {
        public static void AddSiparisTables(UygulamaDbContext context)
        {
            var connectionString = context.Database.GetDbConnection().ConnectionString;

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // SiparisDurumGecmisleri tablosu
                var createSiparisDurumGecmisiTable = @"
                    CREATE TABLE IF NOT EXISTS SiparisDurumGecmisleri (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SiparisId INTEGER NOT NULL,
                        Durum INTEGER NOT NULL,
                        Aciklama TEXT,
                        Tarih TEXT NOT NULL,
                        KullaniciId INTEGER,
                        DegistirenKisi TEXT,
                        FOREIGN KEY (SiparisId) REFERENCES Siparisler(Id) ON DELETE CASCADE,
                        FOREIGN KEY (KullaniciId) REFERENCES Kullanicilar(Id) ON DELETE SET NULL
                    );
                    CREATE INDEX IF NOT EXISTS IX_SiparisDurumGecmisleri_SiparisId ON SiparisDurumGecmisleri(SiparisId);
                    CREATE INDEX IF NOT EXISTS IX_SiparisDurumGecmisleri_Tarih ON SiparisDurumGecmisleri(Tarih);
                ";

                // KargoTakipler tablosu
                var createKargoTakipTable = @"
                    CREATE TABLE IF NOT EXISTS KargoTakipler (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SiparisId INTEGER NOT NULL UNIQUE,
                        KargoFirmasi TEXT NOT NULL,
                        TakipNo TEXT NOT NULL,
                        KargoDurumu TEXT,
                        TahminiTeslimatTarihi TEXT,
                        OlusturmaTarihi TEXT NOT NULL,
                        GuncellenmeTarihi TEXT,
                        FOREIGN KEY (SiparisId) REFERENCES Siparisler(Id) ON DELETE CASCADE
                    );
                    CREATE INDEX IF NOT EXISTS IX_KargoTakipler_SiparisId ON KargoTakipler(SiparisId);
                    CREATE INDEX IF NOT EXISTS IX_KargoTakipler_TakipNo ON KargoTakipler(TakipNo);
                ";

                // KargoHareketler tablosu
                var createKargoHareketTable = @"
                    CREATE TABLE IF NOT EXISTS KargoHareketler (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        KargoTakipId INTEGER NOT NULL,
                        Durum TEXT NOT NULL,
                        Aciklama TEXT,
                        Lokasyon TEXT,
                        Tarih TEXT NOT NULL,
                        FOREIGN KEY (KargoTakipId) REFERENCES KargoTakipler(Id) ON DELETE CASCADE
                    );
                    CREATE INDEX IF NOT EXISTS IX_KargoHareketler_KargoTakipId ON KargoHareketler(KargoTakipId);
                    CREATE INDEX IF NOT EXISTS IX_KargoHareketler_Tarih ON KargoHareketler(Tarih);
                ";

                // Faturalar tablosu
                var createFaturaTable = @"
                    CREATE TABLE IF NOT EXISTS Faturalar (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SiparisId INTEGER NOT NULL UNIQUE,
                        FaturaNo TEXT NOT NULL UNIQUE,
                        FaturaTarihi TEXT NOT NULL,
                        FirmaUnvani TEXT,
                        VergiDairesi TEXT,
                        VergiNo TEXT,
                        Adres TEXT,
                        AraToplam REAL NOT NULL,
                        KDV REAL NOT NULL,
                        KargoUcreti REAL NOT NULL,
                        IndirimTutari REAL NOT NULL,
                        Toplam REAL NOT NULL,
                        PdfUrl TEXT,
                        EFaturaMi INTEGER NOT NULL DEFAULT 0,
                        EFaturaUuid TEXT,
                        FOREIGN KEY (SiparisId) REFERENCES Siparisler(Id) ON DELETE CASCADE
                    );
                    CREATE INDEX IF NOT EXISTS IX_Faturalar_SiparisId ON Faturalar(SiparisId);
                    CREATE INDEX IF NOT EXISTS IX_Faturalar_FaturaNo ON Faturalar(FaturaNo);
                ";

                // Iadeler tablosu
                var createIadeTable = @"
                    CREATE TABLE IF NOT EXISTS Iadeler (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SiparisId INTEGER NOT NULL,
                        KullaniciId INTEGER NOT NULL,
                        Neden INTEGER NOT NULL,
                        Aciklama TEXT,
                        Durum INTEGER NOT NULL DEFAULT 0,
                        IadeKargoTakipNo TEXT,
                        IadeKargoTarihi TEXT,
                        IadeTutari REAL NOT NULL,
                        OdemeYapildiMi INTEGER NOT NULL DEFAULT 0,
                        OdemeTarihi TEXT,
                        RedNedeni TEXT,
                        TalepTarihi TEXT NOT NULL,
                        OnayTarihi TEXT,
                        TamamlanmaTarihi TEXT,
                        FOREIGN KEY (SiparisId) REFERENCES Siparisler(Id) ON DELETE CASCADE,
                        FOREIGN KEY (KullaniciId) REFERENCES Kullanicilar(Id) ON DELETE CASCADE
                    );
                    CREATE INDEX IF NOT EXISTS IX_Iadeler_SiparisId ON Iadeler(SiparisId);
                    CREATE INDEX IF NOT EXISTS IX_Iadeler_KullaniciId ON Iadeler(KullaniciId);
                    CREATE INDEX IF NOT EXISTS IX_Iadeler_Durum ON Iadeler(Durum);
                ";

                // IadeUrunleri tablosu
                var createIadeUrunTable = @"
                    CREATE TABLE IF NOT EXISTS IadeUrunleri (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        IadeId INTEGER NOT NULL,
                        UrunId INTEGER NOT NULL,
                        Adet INTEGER NOT NULL,
                        BirimFiyat REAL NOT NULL,
                        FOREIGN KEY (IadeId) REFERENCES Iadeler(Id) ON DELETE CASCADE,
                        FOREIGN KEY (UrunId) REFERENCES Urunler(Id) ON DELETE CASCADE
                    );
                    CREATE INDEX IF NOT EXISTS IX_IadeUrunleri_IadeId ON IadeUrunleri(IadeId);
                ";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = createSiparisDurumGecmisiTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createKargoTakipTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createKargoHareketTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createFaturaTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createIadeTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createIadeUrunTable;
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("Sipariş tabloları başarıyla oluşturuldu.");
            }
        }
    }
}
