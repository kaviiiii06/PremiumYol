using Microsoft.AspNetCore.Mvc;
using TrendyolClone.Data;
using TrendyolClone.Models;
using TrendyolClone.Services;
using Microsoft.EntityFrameworkCore;

namespace TrendyolClone.Controllers
{
    public class PaymentController : Controller
    {
        private readonly UygulamaDbContext _db;
        private readonly IPaymentService _paymentService;
        private readonly ILoggingService _loggingService;

        public PaymentController(
            UygulamaDbContext db, 
            IPaymentService paymentService,
            ILoggingService loggingService)
        {
            _db = db;
            _paymentService = paymentService;
            _loggingService = loggingService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            try
            {
                var siparis = await _db.Siparisler
                    .Include(o => o.Kullanici)
                    .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.KullaniciId == int.Parse(userId));

                if (siparis == null)
                    return Json(new { success = false, message = "Sipariş bulunamadı!" });

                if (siparis.Durum != SiparisDurumu.Beklemede)
                    return Json(new { success = false, message = "Bu sipariş için ödeme alınamaz!" });

                // Ödeme işlemini gerçekleştir
                var paymentResult = await _paymentService.ProcessPaymentAsync(request);

                // Ödeme işlemini kaydet
                var transaction = new OdemeIslemi
                {
                    SiparisId = siparis.Id,
                    OdemeId = paymentResult.PaymentId,
                    IslemId = paymentResult.TransactionId,
                    Tutar = request.Amount,
                    Durum = paymentResult.Success ? "Success" : "Failed",
                    OdemeYontemi = "CreditCard",
                    KartSonDortHane = request.CardNumber?.Substring(request.CardNumber.Length - 4),
                    HataKodu = paymentResult.ErrorCode,
                    HataMesaji = paymentResult.Message,
                    OlusturmaTarihi = DateTime.Now,
                    TamamlanmaTarihi = paymentResult.Success ? DateTime.Now : null
                };

                _db.OdemeIslemleri.Add(transaction);

                if (paymentResult.Success)
                {
                    siparis.Durum = SiparisDurumu.Hazirlaniyor;
                    siparis.OdemeTarihi = DateTime.Now;
                    
                    await _loggingService.LogPaymentAsync(siparis.Id, "Success", 
                        $"Payment completed: {paymentResult.PaymentId}");
                }
                else
                {
                    await _loggingService.LogPaymentAsync(siparis.Id, "Failed", 
                        $"Payment failed: {paymentResult.Message}");
                }

                await _db.SaveChangesAsync();

                return Json(new 
                { 
                    success = paymentResult.Success, 
                    message = paymentResult.Message,
                    paymentId = paymentResult.PaymentId
                });
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("PaymentController", ex.Message, ex);
                return Json(new { success = false, message = "Ödeme işlemi sırasında bir hata oluştu!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RefundPayment(int orderId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            try
            {
                var siparis = await _db.Siparisler
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.KullaniciId == int.Parse(userId));

                if (siparis == null)
                    return Json(new { success = false, message = "Sipariş bulunamadı!" });

                var transaction = await _db.OdemeIslemleri
                    .FirstOrDefaultAsync(pt => pt.SiparisId == orderId && pt.Durum == "Success");

                if (transaction == null)
                    return Json(new { success = false, message = "Ödeme işlemi bulunamadı!" });

                var refundResult = await _paymentService.RefundPaymentAsync(transaction.OdemeId, transaction.Tutar);

                if (refundResult.Success)
                {
                    transaction.Durum = "Refunded";
                    siparis.Durum = SiparisDurumu.Iptal;
                    
                    await _loggingService.LogPaymentAsync(siparis.Id, "Refunded", 
                        $"Refund completed: {transaction.OdemeId}");
                    
                    await _db.SaveChangesAsync();
                }

                return Json(new 
                { 
                    success = refundResult.Success, 
                    message = refundResult.Message 
                });
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("PaymentController", ex.Message, ex);
                return Json(new { success = false, message = "İade işlemi sırasında bir hata oluştu!" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentStatus(int orderId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş!" });

            var transaction = await _db.OdemeIslemleri
                .FirstOrDefaultAsync(pt => pt.SiparisId == orderId);

            if (transaction == null)
                return Json(new { success = false, message = "Ödeme işlemi bulunamadı!" });

            return Json(new 
            { 
                success = true,
                status = transaction.Durum,
                amount = transaction.Tutar,
                paymentId = transaction.OdemeId,
                createdDate = transaction.OlusturmaTarihi
            });
        }
    }
}
