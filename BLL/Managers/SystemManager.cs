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
using Microsoft.AspNetCore.Http;

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
                
                await MailProvider.SendAsync(new MailMessageViewModel
                {
                    From = contactUs.Email,
                    To = MailProvider.SMTP_USER,
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

        public Result<List<string>> UploadImages(string basePath, IFormFileCollection files)
        {
            try
            {
                var result = new List<string>();
                foreach (var file in files)
                {
                    byte[] fileData = null;
                    const int maxContentLength = 1024 * 1024 * 3; //Size = 3 MB  
                    var allowedFileExtensions = new List<string> { "png", "jpg", "jpeg", "gif" };
                    var ext = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);
                    var extension = ext.ToLower();
                    if (!allowedFileExtensions.Contains(extension))
                    {
                        throw new Exception(EResultMessage.InvalidExtension.ToString());
                    }
                    if (file.Length > maxContentLength)
                    {
                        throw new Exception(EResultMessage.ExceedMaxContent.ToString());
                    }

                    using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                    {
                        fileData = binaryReader.ReadBytes((int)file.Length);
                    }

                    var fileName = string.Format("{0}{1}.{2}", Path.GetFileNameWithoutExtension(file.FileName), DateTime.UtcNow.ToString("yyyyMMdd-HHmmss.fff"), extension);
                    var path = string.Format("/images/{0}", fileName);
                    File.WriteAllBytes("wwwroot" + path, fileData);
                    result.Add(basePath + path);
                }

                return ResultHelper.Succeeded(result, result.Count);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<List<string>>(message: e.Message);
            }
        }
    }
}
