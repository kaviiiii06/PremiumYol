using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrendyolClone.Services;
using TrendyolClone.Models;

namespace TrendyolClone.Tests
{
    public class PaymentServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<IyzicoPaymentService>> _mockLogger;
        private readonly IyzicoPaymentService _paymentService;

        public PaymentServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<IyzicoPaymentService>>();

            // Setup configuration
            _mockConfiguration.Setup(x => x["Payment:Iyzico:ApiKey"]).Returns("test-api-key");
            _mockConfiguration.Setup(x => x["Payment:Iyzico:SecretKey"]).Returns("test-secret-key");
            _mockConfiguration.Setup(x => x["Payment:Iyzico:BaseUrl"]).Returns("https://sandbox-api.iyzipay.com");

            _paymentService = new IyzicoPaymentService(_mockConfiguration.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ProcessPaymentAsync_InSandboxMode_ReturnsSuccess()
        {
            // Arrange
            var paymentRequest = new PaymentRequest
            {
                OrderId = 1,
                Amount = 100.00m,
                CardNumber = "5528790000000008",
                CardHolderName = "Test User",
                ExpireMonth = "12",
                ExpireYear = "2030",
                Cvc = "123"
            };

            // Act
            var result = await _paymentService.ProcessPaymentAsync(paymentRequest);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.PaymentId);
            Assert.NotNull(result.TransactionId);
            Assert.Contains("Test Modu", result.Message);
        }

        [Fact]
        public async Task RefundPaymentAsync_ValidPaymentId_ReturnsSuccess()
        {
            // Arrange
            var paymentId = "IYZICO_12345";
            var amount = 50.00m;

            // Act
            var result = await _paymentService.RefundPaymentAsync(paymentId, amount);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(paymentId, result.PaymentId);
        }

        [Fact]
        public async Task GetPaymentStatusAsync_ValidPaymentId_ReturnsStatus()
        {
            // Arrange
            var paymentId = "IYZICO_12345";

            // Act
            var status = await _paymentService.GetPaymentStatusAsync(paymentId);

            // Assert
            Assert.Equal(PaymentStatus.Success, status);
        }
    }
}
