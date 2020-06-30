using GotIt.BLL.ViewModels;
using GotIt.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GotIt.BLL.Managers
{
    public static class MailProvider
    {
        public static readonly string SMTP_USER = "gotit.noreply@gmail.com";
        public static readonly string PASSWORD = "R290LUl0LTE5OTg=";
        public static readonly string SMTP_SERVER = "smtp.gmail.com";
        public static readonly int SMTP_PORT = 587;

        public static async Task SendAsync(MailMessageViewModel message)
        {
            try
            {
                var client = new SmtpClient(SMTP_SERVER, SMTP_PORT)
                {
                    Credentials = new NetworkCredential(SMTP_USER, Protected.DecodeText(PASSWORD)),
                    EnableSsl = true
                };

                await client.SendMailAsync(new MailMessage(message.From, message.To)
                {
                    IsBodyHtml = message.IsBodyHtml,
                    Body = message.Body,
                    Subject = message.Subject
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
