using Microsoft.Data.Sqlite;

namespace TrendyolClone;

public static class AddColumnHelper
{
    public static void AddProfilFotoUrlColumn()
    {
        var connectionString = "Data Source=PremiumYol.db";
        
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        // Kolon var mı kontrol et
        var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = @"
            SELECT COUNT(*) 
            FROM pragma_table_info('Kullanicilar') 
            WHERE name='ProfilFotoUrl'";
        
        var exists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;
        
        if (!exists)
        {
            Console.WriteLine("ProfilFotoUrl kolonu ekleniyor...");
            var addCommand = connection.CreateCommand();
            addCommand.CommandText = "ALTER TABLE Kullanicilar ADD COLUMN ProfilFotoUrl TEXT";
            addCommand.ExecuteNonQuery();
            Console.WriteLine("ProfilFotoUrl kolonu eklendi!");
        }
        else
        {
            Console.WriteLine("ProfilFotoUrl kolonu zaten mevcut.");
        }
    }
    
    public static void CreateOdemeYontemleriTable()
    {
        var connectionString = "Data Source=PremiumYol.db";
        
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        // Tablo var mı kontrol et
        var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = @"
            SELECT COUNT(*) 
            FROM sqlite_master 
            WHERE type='table' AND name='OdemeYontemleri'";
        
        var exists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;
        
        if (!exists)
        {
            Console.WriteLine("OdemeYontemleri tablosu oluşturuluyor...");
            var createCommand = connection.CreateCommand();
            createCommand.CommandText = @"
                CREATE TABLE OdemeYontemleri (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Ad TEXT NOT NULL,
                    Aciklama TEXT,
                    Tip INTEGER NOT NULL,
                    BankaAdi TEXT,
                    IbanNo TEXT,
                    HesapSahibi TEXT,
                    EkBilgi TEXT,
                    Aktif INTEGER NOT NULL DEFAULT 1,
                    Sira INTEGER NOT NULL DEFAULT 1,
                    OlusturmaTarihi TEXT NOT NULL
                )";
            createCommand.ExecuteNonQuery();
            Console.WriteLine("OdemeYontemleri tablosu oluşturuldu!");
            
            // Varsayılan ödeme yöntemlerini ekle
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                INSERT INTO OdemeYontemleri (Ad, Aciklama, Tip, Aktif, Sira, OlusturmaTarihi) VALUES
                ('Kredi Kartı', 'Online kredi kartı ile ödeme', 1, 1, 1, datetime('now')),
                ('Havale/EFT', 'Banka havalesi veya EFT ile ödeme', 2, 1, 2, datetime('now')),
                ('Kapıda Ödeme', 'Ürün tesliminde nakit ödeme', 2, 1, 3, datetime('now'))";
            insertCommand.ExecuteNonQuery();
            Console.WriteLine("Varsayılan ödeme yöntemleri eklendi!");
        }
        else
        {
            Console.WriteLine("OdemeYontemleri tablosu zaten mevcut.");
        }
    }
}
