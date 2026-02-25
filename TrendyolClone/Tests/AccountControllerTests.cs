using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolClone.Controllers;
using TrendyolClone.Data;
using TrendyolClone.Models;
using Microsoft.AspNetCore.Http;
using Moq;

namespace TrendyolClone.Tests
{
    public class AccountControllerTests
    {
        private readonly UygulamaDbContext _context;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            var options = new DbContextOptionsBuilder<UygulamaDbContext>()
                .UseInMemoryDatabase(databaseName: "TestAccountDb")
                .Options;

            _context = new UygulamaDbContext(options);
            _controller = new AccountController(_context);

            // Mock HttpContext and Session
            var httpContext = new DefaultHttpContext();
            var session = new Mock<ISession>();
            httpContext.Session = session.Object;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            SeedTestData();
        }

        private void SeedTestData()
        {
            if (!_context.Roller.Any())
            {
                _context.Roller.Add(new Rol 
                { 
                    Id = 1, 
                    Ad = "Müşteri", 
                    Aktif = true 
                });
                _context.SaveChanges();
            }
        }

        [Fact]
        public void Login_Get_ReturnsView()
        {
            // Act
            var result = _controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Register_Get_ReturnsView()
        {
            // Act
            var result = _controller.Register();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Register_Post_WithValidData_RedirectsToHome()
        {
            // Arrange
            var kullanici = new Kullanici
            {
                Email = "test@example.com",
                KullaniciAdi = "testuser",
                Sifre = "password123",
                Ad = "Test",
                Soyad = "User",
                TelefonNumarasi = "1234567890"
            };

            // Act
            var result = _controller.Register(kullanici);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public void Register_Post_WithDuplicateEmail_ReturnsViewWithError()
        {
            // Arrange
            var mevcutKullanici = new Kullanici
            {
                Email = "duplicate@example.com",
                KullaniciAdi = "existing",
                Sifre = BCrypt.Net.BCrypt.HashPassword("password"),
                Ad = "Existing",
                Soyad = "User",
                RolId = 1,
                Aktif = true
            };
            _context.Kullanicilar.Add(mevcutKullanici);
            _context.SaveChanges();

            var yeniKullanici = new Kullanici
            {
                Email = "duplicate@example.com",
                KullaniciAdi = "newuser",
                Sifre = "password123",
                Ad = "New",
                Soyad = "User"
            };

            // Act
            var result = _controller.Register(yeniKullanici);

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull(_controller.ViewBag.Error);
        }
    }
}
