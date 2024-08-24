using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
namespace Application.Util
{
    public class SendMailHelper : ISendMailHelper
    {
        private readonly IConfiguration _configuration;
        public SendMailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> SendMailAsync(string email, string subject, string message)
        {
            try
            {
                var _email = _configuration["EmailSetting:Email"];
                var _epass = _configuration["EmailSetting:Password"];
                var _dispName = _configuration["EmailSetting:DisplayName"];
                //NET.SMTP 
                /* MailMessage myMessage = new MailMessage();
                 myMessage.IsBodyHtml = true;
                 myMessage.To.Add(email);
                 myMessage.From = new MailAddress(_email, _dispName);
                 myMessage.Subject = subject;
                 myMessage.Body = message;
                 using (SmtpClient smtp = new SmtpClient())
                 {
                     smtp.EnableSsl = true;
                     smtp.Host = "smtp.gmail.com";
                     smtp.Port = 465;
                     smtp.UseDefaultCredentials = false;
                     smtp.Credentials = new NetworkCredential(_email, _epass);
                     smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                     smtp.SendCompleted += (s, e) => { smtp.Dispose(); };
                     await smtp.SendMailAsync(myMessage);
                 }*/
                //SPIRE.MAIL
                /*  MailAddress addressFrom = new MailAddress(_email, "Good Exchange Sytem");
                  MailAddress addressTo = new MailAddress(email);
                  MailMessage mailMessage = new MailMessage(addressFrom, addressTo);

                  mailMessage.Date = DateTime.Now;
                  mailMessage.Subject = "Sending Email with HTML Body";
                  mailMessage.BodyHtml = message;
                  SmtpClient client = new SmtpClient();
                  client.Host = "smtp.gmail.com";
                  client.Port = 465;
                  client.Username = addressFrom.Address;
                  client.Password = _epass;
                  client.ConnectionProtocols = ConnectionProtocols.Ssl;
                  client.SendOne(mailMessage);*/
                //MAILKIT
                var mailMessage = new MimeMessage();
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = message;
                mailMessage.From.Add(new MailboxAddress(_dispName, _email));
                mailMessage.To.Add(new MailboxAddress("", email));
                mailMessage.Subject = subject;
                mailMessage.Body =bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    client.Authenticate(_email,_epass);
                    client.Send(mailMessage);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception details for troubleshooting
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> SendListMailAsync(List<string> emails, string subject, string message)
        {
            try
            {
                var _email = _configuration["EmailSetting:Email"];
                var _epass = _configuration["EmailSetting:Password"];
                var _dispName = _configuration["EmailSetting:DisplayName"];
              /*  MailMessage myMessage = new MailMessage();
                foreach (var email in emails)
                {
                    myMessage.To.Add(email);
                }
                myMessage.IsBodyHtml = true;
                myMessage.From = new MailAddress(_email, _dispName);
                myMessage.Subject = subject;
                myMessage.Body = message;
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.EnableSsl = true;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_email, _epass);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.SendCompleted += (s, e) => { smtp.Dispose(); };
                    await smtp.SendMailAsync(myMessage);
                }*/
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
    public interface ISendMailHelper
    {
        public Task<bool> SendMailAsync(string email, string subject, string message);
        public Task<bool> SendListMailAsync(List<string> emails, string subject, string message);
    }
}
