#nullable enable
namespace TrendyolClone.Models
{
    public class HataModeli
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
#nullable restore
