using GotIt.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GotIt.BLL.Managers
{
    public class EmailProvider
    {
        public static readonly string SMTP_USER = "gotit.noreply@gmail.com";
        public static readonly string PASSWORD = "R290LUl0LTE5OTg=";
        public static readonly string SMTP_SERVER = "smtp.gmail.com";
        public static readonly int SMTP_PORT = 587;

        public async Task<Result<bool>> SendMailAsync(MailMessage message)
        {
            try
            {
                var client = new SmtpClient(SMTP_SERVER, SMTP_PORT)
                {
                    Credentials = new NetworkCredential(SMTP_USER, Protected.DecodeText(PASSWORD)),
                    EnableSsl = true
                };
                await client.SendMailAsync(message);
                return ResultHelper.Succeeded(true);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(message: e.Message);
            }
        }
    }
}
