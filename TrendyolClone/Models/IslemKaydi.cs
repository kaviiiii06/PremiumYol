namespace TrendyolClone.Models
{
    public class IslemKaydi
    {
        public int Id { get; set; }
        public int? KullaniciId { get; set; }
        public int? YoneticiId { get; set; }
        public string Islem { get; set; }
        public string VarlikTipi { get; set; }
        public int? VarlikId { get; set; }
        public string EskiDeger { get; set; }
        public string YeniDeger { get; set; }
        public string IpAdresi { get; set; }
        public string KullaniciAjanı { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
    }
}
