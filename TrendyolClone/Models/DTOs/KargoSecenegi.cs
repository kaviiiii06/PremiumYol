namespace TrendyolClone.Models.DTOs
{
    public class KargoSecenegi
    {
        public int Id { get; set; }
        public string FirmaAdi { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public decimal Ucret { get; set; }
        public int TahminiGun { get; set; }
        public bool UcretsizKargo { get; set; }
        public string Aciklama { get; set; } = string.Empty;
    }
}
