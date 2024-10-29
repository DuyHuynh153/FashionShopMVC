using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace FashionShopMVC.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _smtpServer = configuration["Smtp:Server"];
            _smtpPort = int.Parse(configuration["Smtp:Port"]);
            _smtpUser = configuration["Smtp:User"];
            _smtpPass = configuration["Smtp:Pass"];

            // Kiểm tra nếu giá trị _smtpUser là null hoặc rỗng để báo lỗi rõ ràng
            if (string.IsNullOrEmpty(_smtpUser))
            {
                throw new ArgumentNullException(nameof(_smtpUser), "SMTP user is not configured properly.");
            }
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("BlvckShop", _smtpUser));
            mailMessage.To.Add(new MailboxAddress("", email));
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new SmtpClient())
            {
                // Sử dụng SecureSocketOptions.StartTls khi kết nối với cổng 587
                await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(mailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
