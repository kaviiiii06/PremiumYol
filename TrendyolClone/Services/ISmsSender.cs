namespace TrendyolClone.Services
{
    public interface ISmsSender
    {
        Task<bool> SendSmsAsync(string phoneNumber, string message);
        Task<bool> SendBulkSmsAsync(List<string> phoneNumbers, string message);
    }
}
