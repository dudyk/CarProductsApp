using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MimeKit;
using CarProduct.Persistence.Models;

namespace CarProduct.Application.UserNotification
{
    public class EmailNotification : IUserNotification
    {
        private readonly EmailNotificationSettings _notificationSettings;
        private readonly SmtpSettings _smtpSettings;

        public EmailNotification(
            IOptions<EmailNotificationSettings> emailNotificationSettings,
            IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
            _notificationSettings = emailNotificationSettings.Value;
        }

        public async Task NotifyCreateProduct(Product product)
        {
            var bodyBuilder = new BodyBuilder { TextBody = $"{product.VehicleId} product is created" };

            var message = new MimeMessage();

            var from = new MailboxAddress("no-reply@example.com", _notificationSettings.FromUser);
            message.From.Add(from);

            var to = MailboxAddress.Parse(_notificationSettings.ToUser);
            message.To.Add(to);

            message.Subject = _notificationSettings.Subject;
            message.Body = bodyBuilder.ToMessageBody();

            await SendMail(message);
        }

        private async Task SendMail(MimeMessage email)
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();

            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, _smtpSettings.UseSll);
            await client.AuthenticateAsync(_smtpSettings.User, _smtpSettings.Password);

            await client.SendAsync(email);
        }
    }
}
