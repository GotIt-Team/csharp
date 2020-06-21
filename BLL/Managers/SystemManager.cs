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
using System.IO;

namespace GotIt.BLL.Managers
{
    public class SystemManager : Repository<FeedbackEntity>
    {
        private readonly EmailProvider _emailProvider;

        public SystemManager(GotItDbContext dbContext, EmailProvider emailProvider) : base(dbContext) 
        {
            _emailProvider = emailProvider;
        }
        
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

        public async Task<Result<bool>> ContactUs(ContactUsViewModel contactUs)
        {
            try
            {
                MailMessage msg = new MailMessage(contactUs.Email, EmailProvider.SMTP_USER)
                {
                    Subject = contactUs.Subject,
                    Body = contactUs.Message
                };

                return await _emailProvider.SendMailAsync(msg);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(message: e.Message);
            }
        }
    }
}
