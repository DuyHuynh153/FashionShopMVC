
using System.Net;
using System.Net.Mail;

namespace FashionShopMVC.Helper
{
    public class EmailOrderSucessService
    {
        public static void SendMail(string name, string subject, string content, string toMail)
        {
            try
            {
                MailMessage message = new MailMessage();

                var smtp = new SmtpClient();
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential("hlamduy116@gmail.com", "nade sflp womy mjxm");
                    smtp.Timeout = 20000;
                }

                MailAddress fromAddress = new MailAddress("hlamduy116@gmail.com", name);

                message.From = fromAddress;
                message.To.Add(toMail);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = content;

                smtp.Send(message);
            }
            catch(Exception ex)
            {
            }

        }
        

    }
}
