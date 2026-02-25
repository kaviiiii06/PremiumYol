using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;

namespace TrendyolClone
{
    public static class AddYorumTablesHelper
    {
        public static void AddYorumTables(UygulamaDbContext context)
        {
            // UrunYorum tablosu
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UrunYorumlari')
                BEGIN
                    CREATE TABLE UrunYorumlari (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        UrunId INT NOT NULL,
                        KullaniciId INT NOT NULL,
                        SiparisId INT,
                        Baslik NVARCHAR(200) NOT NULL,
                        YorumMetni NVARCHAR(2000) NOT NULL,
                        GenelPuan INT NOT NULL CHECK (GenelPuan >= 1 AND GenelPuan <= 5),
                        UrunKalitesiPuan INT NOT NULL CHECK (UrunKalitesiPuan >= 1 AND UrunKalitesiPuan <= 5),
                        FiyatPerformansPuan INT NOT NULL CHECK (FiyatPerformansPuan >= 1 AND FiyatPerformansPuan <= 5),
                        KargoHiziPuan INT NOT NULL CHECK (KargoHiziPuan >= 1 AND KargoHiziPuan <= 5),
                        PaketlemePuan INT NOT NULL CHECK (PaketlemePuan >= 1 AND PaketlemePuan <= 5),
                        OnaylandiMi BIT DEFAULT 0,
                        Durum INT DEFAULT 0,
                        YardimciBuldum INT DEFAULT 0,
                        YardimciBulmadim INT DEFAULT 0,
                        SikayetSayisi INT DEFAULT 0,
                        Tarih DATETIME2 DEFAULT GETDATE(),
                        GuncellenmeTarihi DATETIME2,
                        RedSebep NVARCHAR(500),
                        FOREIGN KEY (UrunId) REFERENCES Urunler(Id),
                        FOREIGN KEY (KullaniciId) REFERENCES Kullanicilar(Id),
                        FOREIGN KEY (SiparisId) REFERENCES Siparisler(Id)
                    )
                END
            ");

            // YorumResim tablosu
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'YorumResimleri')
                BEGIN
                    CREATE TABLE YorumResimleri (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        YorumId INT NOT NULL,
                        ResimUrl NVARCHAR(500) NOT NULL,
                        Sira INT DEFAULT 0,
                        EklenmeTarihi DATETIME2 DEFAULT GETDATE(),
                        FOREIGN KEY (YorumId) REFERENCES UrunYorumlari(Id) ON DELETE CASCADE
                    )
                END
            ");

            // YorumVideo tablosu
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'YorumVideolari')
                BEGIN
                    CREATE TABLE YorumVideolari (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        YorumId INT NOT NULL,
                        VideoUrl NVARCHAR(500) NOT NULL,
                        Thumbnail NVARCHAR(500),
                        EklenmeTarihi DATETIME2 DEFAULT GETDATE(),
                        FOREIGN KEY (YorumId) REFERENCES UrunYorumlari(Id) ON DELETE CASCADE
                    )
                END
            ");

            // YorumEtkilesim tablosu
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'YorumEtkilesimleri')
                BEGIN
                    CREATE TABLE YorumEtkilesimleri (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        YorumId INT NOT NULL,
                        KullaniciId INT NOT NULL,
                        Tip INT NOT NULL,
                        Tarih DATETIME2 DEFAULT GETDATE(),
                        FOREIGN KEY (YorumId) REFERENCES UrunYorumlari(Id) ON DELETE CASCADE,
                        FOREIGN KEY (KullaniciId) REFERENCES Kullanicilar(Id),
                        UNIQUE (YorumId, KullaniciId)
                    )
                END
            ");

            // SaticiYorumYaniti tablosu
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaticiYorumYanitlari')
                BEGIN
                    CREATE TABLE SaticiYorumYanitlari (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        YorumId INT NOT NULL,
                        SaticiId INT NOT NULL,
                        Yanit NVARCHAR(1000) NOT NULL,
                        Tarih DATETIME2 DEFAULT GETDATE(),
                        GuncellenmeTarihi DATETIME2,
                        FOREIGN KEY (YorumId) REFERENCES UrunYorumlari(Id) ON DELETE CASCADE,
                        FOREIGN KEY (SaticiId) REFERENCES Saticilar(Id),
                        UNIQUE (YorumId)
                    )
                END
            ");

            // YorumRapor tablosu
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'YorumRaporlari')
                BEGIN
                    CREATE TABLE YorumRaporlari (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        YorumId INT NOT NULL,
                        RaporEdenKullaniciId INT NOT NULL,
                        Sebep NVARCHAR(100) NOT NULL,
                        Aciklama NVARCHAR(500),
                        Durum INT DEFAULT 0,
                        Tarih DATETIME2 DEFAULT GETDATE(),
                        InceleyenYoneticiId INT,
                        IncelemeTarihi DATETIME2,
                        YoneticiNotu NVARCHAR(500),
                        FOREIGN KEY (YorumId) REFERENCES UrunYorumlari(Id) ON DELETE CASCADE,
                        FOREIGN KEY (RaporEdenKullaniciId) REFERENCES Kullanicilar(Id)
                    )
                END
            ");

            // İndeksler
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UrunYorumlari_UrunId')
                    CREATE INDEX IX_UrunYorumlari_UrunId ON UrunYorumlari(UrunId);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UrunYorumlari_KullaniciId')
                    CREATE INDEX IX_UrunYorumlari_KullaniciId ON UrunYorumlari(KullaniciId);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UrunYorumlari_OnaylandiMi')
                    CREATE INDEX IX_UrunYorumlari_OnaylandiMi ON UrunYorumlari(OnaylandiMi);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_YorumResimleri_YorumId')
                    CREATE INDEX IX_YorumResimleri_YorumId ON YorumResimleri(YorumId);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_YorumEtkilesimleri_YorumId')
                    CREATE INDEX IX_YorumEtkilesimleri_YorumId ON YorumEtkilesimleri(YorumId);
            ");
        }
    }
}
