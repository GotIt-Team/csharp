using GotIt.BLL.ViewModels;
using GotIt.Common.Helper;
using GotIt.MSSQL;
using GotIt.MSSQL.Models;
using GotIt.MSSQL.Repository;
using System;
using System.Net.Http;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;

namespace GotIt.BLL.Managers
{
    public class SystemManager : Repository<FeedbackEntity>
    {
        public SystemManager(GotItDbContext dbContext) : base(dbContext) {}
        
        public Result<bool> AddFeedback(int userId, FeedbackViewModel feedbackViewModel)
        {
            try
            {
                var obj = new FeedbackEntity
                {
                    Rate = feedbackViewModel.Rate,
                    Opinion = feedbackViewModel.Opinion,
                    UserId = userId
                };

                Add(obj);
                
                SaveChanges();
                
                return ResultHelper.Succeeded(data: true);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed(data: false , message: e.Message);
            }
        }

        public async Task<bool> ContactUsAsync(string from,string message)
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("hassan.sheimy88@gmail.com", "Password"),
                    EnableSsl = true
                };
                
                MailMessage msg = new MailMessage(from,"hassan.sheimy98@yahoo.com");
                msg.Subject = "ومعانا اول تيست ونقول بسم الله";
                client.Send(msg);
                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                return await Task.FromResult(false);
            }
        }
    }
}
