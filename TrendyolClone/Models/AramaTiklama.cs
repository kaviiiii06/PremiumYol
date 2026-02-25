namespace TrendyolClone.Models
{
    public class AramaTiklama
    {
        public int Id { get; set; }

        // Hangi aramadan geldi
        public int AramaGecmisiId { get; set; }
        public virtual AramaGecmisi AramaGecmisi { get; set; } = null!;

        // Hangi ürüne tıklandı
        public int UrunId { get; set; }
        public virtual Urun Urun { get; set; } = null!;

        public DateTime TiklamaTarihi { get; set; } = DateTime.Now;

        // Tıklama sırası (arama sonuçlarında kaçıncı sıradaydı)
        public int Sira { get; set; }
    }
}
