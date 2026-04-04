
using FurnitureGardenDesign.Services.Core.Interfaces.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Services.Core.Implementations.Account
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var apiKey = _configuration["SendGrid:ApiKey"];
                var fromEmail = _configuration["SendGrid:FromEmail"];
                var fromName = _configuration["SendGrid:FromName"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("SendGrid API key is not configured");
                    return false;
                }

                if (string.IsNullOrEmpty(fromEmail))
                {
                    _logger.LogError("SendGrid FromEmail is not configured");
                    return false;
                }

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(fromEmail, fromName ?? "Furniture Garden Design");
                var toEmail = new EmailAddress(to);
                var htmlContent = body;
                var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, null, htmlContent);

                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Email sent to {to}");
                    return true;
                }
                else
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError($"Failed to send email to {to}. Status: {response.StatusCode}, Response: {responseBody}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception when sending email to {to}");
                return false;
            }
        }
    }
}