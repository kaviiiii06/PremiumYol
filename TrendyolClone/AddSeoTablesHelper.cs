using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;

namespace TrendyolClone
{
    public static class AddSeoTablesHelper
    {
        public static void AddSeoTables(UygulamaDbContext context)
        {
            try
            {
                // SeoAyarlari tablosunu oluştur
                context.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS SeoAyarlari (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SayfaTipi TEXT NOT NULL,
                        ReferansId INTEGER,
                        Baslik TEXT NOT NULL,
                        Aciklama TEXT NOT NULL,
                        Anahtar TEXT NOT NULL,
                        CanonicalUrl TEXT NOT NULL,
                        OgBaslik TEXT NOT NULL,
                        OgAciklama TEXT NOT NULL,
                        OgResim TEXT NOT NULL,
                        Indexlenebilir INTEGER NOT NULL DEFAULT 1,
                        TakipEdilebilir INTEGER NOT NULL DEFAULT 1,
                        GuncellemeTarihi TEXT NOT NULL
                    )
                ");

                // Index'ler
                context.Database.ExecuteSqlRaw(@"
                    CREATE INDEX IF NOT EXISTS IX_SeoAyarlari_SayfaTipi 
                    ON SeoAyarlari(SayfaTipi)
                ");

                context.Database.ExecuteSqlRaw(@"
                    CREATE INDEX IF NOT EXISTS IX_SeoAyarlari_ReferansId 
                    ON SeoAyarlari(ReferansId)
                ");

                Console.WriteLine("✅ SEO tabloları başarıyla oluşturuldu!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SEO tabloları oluşturulurken hata: {ex.Message}");
            }
        }
    }
}
