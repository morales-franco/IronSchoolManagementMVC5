using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace IronSchool.Utils
{
    public class EmailSender
    {
        public enum MailType
        {
            AccountActivation
        }

        private static SmtpClient _smtpClient;
        public static object isLocked = true;

        private static SmtpClient smtpClient
        {
            get
            {
                if (_smtpClient == null)
                    _smtpClient = new SmtpClient();                    
                return _smtpClient;
            }
        }

        public static void Send(string toEmail, string subject, string body)
        {
            Send_Async(toEmail, subject, body);
        }

        public static void Send_Async(string toEmail, string subject, string body)
        {
            //obtengo los datos solamente una vez desde el proxy de configuración
            SmtpClient _smtpClient2;
            _smtpClient2 = new SmtpClient();
            
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(toEmail));
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTPCCOAccount"]))
            {
                mailMessage.Bcc.Add(new MailAddress(ConfigurationManager.AppSettings["SMTPCCOAccount"]));
            }

            _smtpClient2.SendCompleted += (s, e) =>
            {
                _smtpClient2.Dispose();
                mailMessage.Dispose();
            };
            _smtpClient2.SendAsync(mailMessage, new object());

        }

        public static void Send_NoAsync(string toEmail, string subject, string body)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(toEmail));
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTPCCOAccount"]))
            {
                mailMessage.Bcc.Add(new MailAddress(ConfigurationManager.AppSettings["SMTPCCOAccount"]));
            }
            smtpClient.Send(mailMessage);
            smtpClient.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);
        }

        private static void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            isLocked = true;
        }
    }
}
