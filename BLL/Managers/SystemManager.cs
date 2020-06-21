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
using GotIt.Common.Enums;

namespace GotIt.BLL.Managers
{
    public class SystemManager : Repository<FeedbackEntity>
    {

        public SystemManager(GotItDbContext dbContext) : base(dbContext) 
        {
        }
        
        public Result<bool> AddFeedback(int userId, FeedbackViewModel feedbackViewModel)
        {
            try
            {
                var feedback = new FeedbackEntity
                {
                    Rate = feedbackViewModel.Rate,
                    Opinion = feedbackViewModel.Opinion,
                    UserId = userId
                };

                Add(feedback);
                
                var result = SaveChanges();

                if (!result)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }
                
                return ResultHelper.Succeeded(data: true);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed(data: false , message: e.Message);
            }
        }

        public async Task<Result<bool>> ContactUs(int userId, ContactUsViewModel contactUs)
        {
            try
            {
                var body = File.ReadAllText("wwwroot/html/contactus.html");
                body = body.Replace("{user-email}", contactUs.Email);
                body = body.Replace("{user-id}", userId.ToString());
                body = body.Replace("{user-message}", contactUs.Message);
                
                await EmailProvider.SendMailAsync(new EmailMessageViewModel
                {
                    From = contactUs.Email,
                    To = EmailProvider.SMTP_USER,
                    Subject = contactUs.Subject,
                    Body = body,
                    IsBodyHtml = true
                });

                return ResultHelper.Succeeded(true);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(message: e.Message);
            }
        }
    }
}
