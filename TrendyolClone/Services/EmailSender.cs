using System.Net;
using System.Net.Mail;

namespace TrendyolClone.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"] ?? "";
                var smtpPass = _configuration["Email:SmtpPass"] ?? "";
                var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
                var fromName = _configuration["Email:FromName"] ?? "Trendyol Clone";

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUser, smtpPass)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                
                _logger.LogInformation($"Email başarıyla gönderildi: {email}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Email gönderme hatası: {email}");
                return false;
            }
        }

        public async Task<bool> SendEmailWithTemplateAsync(string email, string templateName, Dictionary<string, string> parameters)
        {
            // Template'i yükle ve parametreleri uygula
            var template = LoadTemplate(templateName);
            
            foreach (var param in parameters)
            {
                template = template.Replace($"{{{param.Key}}}", param.Value);
            }

            return await SendEmailAsync(email, GetTemplateSubject(templateName), template);
        }

        private string LoadTemplate(string templateName)
        {
            // Basit bir template sistemi - gerçek uygulamada dosyadan veya veritabanından yüklenebilir
            return templateName switch
            {
                "SIPARIS_ONAY" => @"
                    <h2>Siparişiniz Alındı!</h2>
                    <p>Merhaba {AdSoyad},</p>
                    <p>Sipariş numaranız: <strong>{SiparisNo}</strong></p>
                    <p>Toplam tutar: <strong>{ToplamTutar} TL</strong></p>
                    <p>Siparişiniz en kısa sürede hazırlanacaktır.</p>
                ",
                "KARGO_BILGI" => @"
                    <h2>Kargoya Verildi!</h2>
                    <p>Merhaba {AdSoyad},</p>
                    <p>Sipariş numaranız: <strong>{SiparisNo}</strong></p>
                    <p>Kargo takip no: <strong>{KargoTakipNo}</strong></p>
                    <p>Kargo firması: <strong>{KargoFirma}</strong></p>
                ",
                _ => "<p>{Mesaj}</p>"
            };
        }

        private string GetTemplateSubject(string templateName)
        {
            return templateName switch
            {
                "SIPARIS_ONAY" => "Siparişiniz Alındı",
                "KARGO_BILGI" => "Siparişiniz Kargoya Verildi",
                _ => "Bildirim"
            };
        }
    }
}
