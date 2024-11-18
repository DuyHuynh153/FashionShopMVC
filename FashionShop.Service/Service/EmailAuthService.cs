using FashionShopMVC.Service.Model;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace FashionShopMVC.Service.Service
{
    public class EmailAuthService : IEmailAuthService
    {
        private readonly EmailConfig _emailConfig;

        public EmailAuthService(EmailConfig emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendAuthEmail(Message message)
        {
            var emailMessage =  CreateEmailMessage(message);
            Send(emailMessage);
        }

        private void Send(MimeMessage emailMessage)
        {
            // using MailKit.Net.Smtp;
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                
                client.Send(emailMessage);  
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Công Ty TNHH BLVCK", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content};

            return emailMessage;
        }
    }
}
