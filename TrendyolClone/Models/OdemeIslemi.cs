namespace TrendyolClone.Models
{
    public class OdemeIslemi
    {
        public int Id { get; set; }
        public int SiparisId { get; set; }
        public Siparis Siparis { get; set; }
        public string OdemeId { get; set; }
        public string IslemId { get; set; }
        public decimal Tutar { get; set; }
        public string Durum { get; set; }
        public string OdemeYontemi { get; set; }
        public string KartSonDortHane { get; set; }
        public string HataKodu { get; set; }
        public string HataMesaji { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? TamamlanmaTarihi { get; set; }
    }
}
