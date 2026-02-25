using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;

namespace TrendyolClone
{
    public static class AddSaticiTablesHelper
    {
        public static void AddSaticiTables(UygulamaDbContext context)
        {
            // Satici tablosu
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Saticilar')
                BEGIN
                    CREATE TABLE Saticilar (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        KullaniciId INT NOT NULL,
                        MagazaAdi NVARCHAR(100) NOT NULL,
                        Slug NVARCHAR(100) NOT NULL UNIQUE,
                        Aciklama NVARCHAR(1000),
                        Logo NVARCHAR(500),
                        KapakResmi NVARCHAR(500),
                        VergiNo NVARCHAR(20),
                        VergiDairesi NVARCHAR(100),
                        TicariUnvan NVARCHAR(200),
                        Telefon NVARCHAR(20),
                        Email NVARCHAR(100),
                        Adres NVARCHAR(500),
                        KomisyonOrani DECIMAL(5,2) DEFAULT 15,
                        Durum INT DEFAULT 0,
                        OnayTarihi DATETIME2,
                        KayitTarihi DATETIME2 DEFAULT GETDATE(),
                        AktifMi BIT DEFAULT 1,
                        ToplamSatis INT DEFAULT 0,
                        ToplamKazanc DECIMAL(18,2) DEFAULT 0,
                        OrtalamaPuan DECIMAL(3,2) DEFAULT 0,
                        DegerlendirmeSayisi INT DEFAULT 0,
                        FOREIGN KEY (KullaniciId) REFERENCES Kullanicilar(Id)
                    )
                END
            ");

            // SaticiUrun tablosu
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaticiUrunler')
                BEGIN
                    CREATE TABLE SaticiUrunler (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        SaticiId INT NOT NULL,
                        UrunId INT NOT NULL,
                        Stok INT DEFAULT 0,
                        Fiyat DECIMAL(18,2) NOT NULL,
                        IndirimliFiyat DECIMAL(18,2),
                        KargoSuresi INT DEFAULT 3,
                        Durum INT DEFAULT 0,
                        OnayTarihi DATETIME2,
                        EklenmeTarihi DATETIME2 DEFAULT GETDATE(),
                        GuncellenmeTarihi DATETIME2,
                        RedSebep NVARCHAR(500),
                        GoruntulemeSayisi INT DEFAULT 0,
                        SatisSayisi INT DEFAULT 0,
                        ToplamSatisTutari DECIMAL(18,2) DEFAULT 0,
                        FOREIGN KEY (SaticiId) REFERENCES Saticilar(Id),
                        FOREIGN KEY (UrunId) REFERENCES Urunler(Id)
                    )
                END
            ");

            // SaticiSiparis tablosu
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaticiSiparisler')
                BEGIN
                    CREATE TABLE SaticiSiparisler (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        SaticiId INT NOT NULL,
                        SiparisId INT NOT NULL,
                        UrunId INT NOT NULL,
                        Adet INT NOT NULL,
                        BirimFiyat DECIMAL(18,2) NOT NULL,
                        ToplamTutar DECIMAL(18,2) NOT NULL,
                        KomisyonOrani DECIMAL(5,2) NOT NULL,
                        KomisyonTutari DECIMAL(18,2) NOT NULL,
                        SaticiKazanci DECIMAL(18,2) NOT NULL,
                        Durum INT DEFAULT 0,
                        KargoTakipNo NVARCHAR(50),
                        KargoTarihi DATETIME2,
                        TeslimTarihi DATETIME2,
                        SiparisTarihi DATETIME2 DEFAULT GETDATE(),
                        Notlar NVARCHAR(500),
                        FOREIGN KEY (SaticiId) REFERENCES Saticilar(Id),
                        FOREIGN KEY (SiparisId) REFERENCES Siparisler(Id),
                        FOREIGN KEY (UrunId) REFERENCES Urunler(Id)
                    )
                END
            ");

            // SaticiOdeme tablosu
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaticiOdemeler')
                BEGIN
                    CREATE TABLE SaticiOdemeler (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        SaticiId INT NOT NULL,
                        Tutar DECIMAL(18,2) NOT NULL,
                        KomisyonTutari DECIMAL(18,2) NOT NULL,
                        NetTutar DECIMAL(18,2) NOT NULL,
                        Donem INT NOT NULL,
                        OdemeTarihi DATETIME2,
                        OdemeTipi INT DEFAULT 0,
                        Durum INT DEFAULT 0,
                        Aciklama NVARCHAR(500),
                        ReferansNo NVARCHAR(100),
                        TalepTarihi DATETIME2 DEFAULT GETDATE(),
                        OnaylayanYoneticiId INT,
                        OnayTarihi DATETIME2,
                        FOREIGN KEY (SaticiId) REFERENCES Saticilar(Id)
                    )
                END
            ");

            // SaticiDegerlendirme tablosu
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaticiDegerlendirmeler')
                BEGIN
                    CREATE TABLE SaticiDegerlendirmeler (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        SaticiId INT NOT NULL,
                        KullaniciId INT NOT NULL,
                        SiparisId INT NOT NULL,
                        Puan INT NOT NULL CHECK (Puan >= 1 AND Puan <= 5),
                        Yorum NVARCHAR(1000),
                        UrunKalitesiPuan INT DEFAULT 0,
                        KargoPuan INT DEFAULT 0,
                        IletisimPuan INT DEFAULT 0,
                        Tarih DATETIME2 DEFAULT GETDATE(),
                        OnaylandiMi BIT DEFAULT 0,
                        SaticiYaniti NVARCHAR(1000),
                        YanitTarihi DATETIME2,
                        FOREIGN KEY (SaticiId) REFERENCES Saticilar(Id),
                        FOREIGN KEY (KullaniciId) REFERENCES Kullanicilar(Id),
                        FOREIGN KEY (SiparisId) REFERENCES Siparisler(Id)
                    )
                END
            ");

            // İndeksler
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Saticilar_KullaniciId')
                    CREATE INDEX IX_Saticilar_KullaniciId ON Saticilar(KullaniciId);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Saticilar_Slug')
                    CREATE INDEX IX_Saticilar_Slug ON Saticilar(Slug);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SaticiUrunler_SaticiId')
                    CREATE INDEX IX_SaticiUrunler_SaticiId ON SaticiUrunler(SaticiId);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SaticiSiparisler_SaticiId')
                    CREATE INDEX IX_SaticiSiparisler_SaticiId ON SaticiSiparisler(SaticiId);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SaticiOdemeler_SaticiId')
                    CREATE INDEX IX_SaticiOdemeler_SaticiId ON SaticiOdemeler(SaticiId);
            ");
        }
    }
}
