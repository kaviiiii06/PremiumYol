namespace TrendyolClone.Services
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(string email, string subject, string message);
        Task<bool> SendEmailWithTemplateAsync(string email, string templateName, Dictionary<string, string> parameters);
    }
}
