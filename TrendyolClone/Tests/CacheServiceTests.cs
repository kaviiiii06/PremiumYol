using Xunit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using TrendyolClone.Services;

namespace TrendyolClone.Tests
{
    public class CacheServiceTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<ILogger<CacheService>> _mockLogger;
        private readonly CacheService _cacheService;

        public CacheServiceTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _mockLogger = new Mock<ILogger<CacheService>>();
            _cacheService = new CacheService(_memoryCache, _mockLogger.Object);
        }

        [Fact]
        public void Set_And_Get_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";

            // Act
            _cacheService.Set(key, value);
            var result = _cacheService.Get<string>(key);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void Get_NonExistentKey_ShouldReturnDefault()
        {
            // Arrange
            var key = "non-existent-key";

            // Act
            var result = _cacheService.Get<string>(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Remove_ShouldDeleteKey()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            _cacheService.Set(key, value);

            // Act
            _cacheService.Remove(key);
            var result = _cacheService.Get<string>(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Exists_ShouldReturnCorrectValue()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";

            // Act & Assert
            Assert.False(_cacheService.Exists(key));
            
            _cacheService.Set(key, value);
            Assert.True(_cacheService.Exists(key));
            
            _cacheService.Remove(key);
            Assert.False(_cacheService.Exists(key));
        }

        [Fact]
        public void Set_WithExpiration_ShouldExpire()
        {
            // Arrange
            var key = "expiring-key";
            var value = "expiring-value";
            var expiration = TimeSpan.FromMilliseconds(100);

            // Act
            _cacheService.Set(key, value, expiration);
            var resultBefore = _cacheService.Get<string>(key);
            
            System.Threading.Thread.Sleep(150);
            
            var resultAfter = _cacheService.Get<string>(key);

            // Assert
            Assert.Equal(value, resultBefore);
            Assert.Null(resultAfter);
        }
    }
}
