namespace FurnitureGardenDesign.Services.Core.Interfaces.Account
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}