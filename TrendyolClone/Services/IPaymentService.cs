using TrendyolClone.Models;

namespace TrendyolClone.Services
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
        Task<PaymentResult> RefundPaymentAsync(string paymentId, decimal amount);
        Task<PaymentStatus> GetPaymentStatusAsync(string paymentId);
    }

    public class PaymentRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpireMonth { get; set; }
        public string ExpireYear { get; set; }
        public string Cvc { get; set; }
        public Adres BillingAddress { get; set; }
        public Adres ShippingAddress { get; set; }
        public Kullanici User { get; set; }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string PaymentId { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public string TransactionId { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed,
        Refunded,
        Cancelled
    }
}
