using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Common.Exceptions;
using GotIt.Common.Helper;
using GotIt.MSSQL;
using GotIt.MSSQL.Models;
using GotIt.MSSQL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace GotIt.BLL.Managers
{
    public class UserManager : Repository<UserEntity>
    {
        private readonly TokenManager _tokenManager;
        
        public UserManager(GotItDbContext dbContext, TokenManager tokenManager) : base(dbContext)
        {
            _tokenManager = tokenManager;
        }
        public Result<bool> AddUser(RegisterationViewModel userViewModel)
        {
            try
            {
                var user = new UserEntity {
                    Name = userViewModel.Name,
                    Email = userViewModel.Email,
                    PhoneNumber = userViewModel.PhoneNumber,
                    City = userViewModel.City,
                    Type = userViewModel.Type,
                    Picture = userViewModel.Picture,
                    Country = userViewModel.Country,
                    Gender = userViewModel.Gender,
                };
                
                // Check for Type
                if (!Enum.IsDefined(typeof(EUserType), userViewModel.Type))
                {
                    throw new Exception(EResultMessage.NotUserType.ToString());
                }

                // Add user Typr
                user.Type = userViewModel.Type;

                if (userViewModel.Password != userViewModel.RepeatPassword)
                {
                    throw new Exception(EResultMessage.PasswordNotMatched.ToString());
                }

                // Hash user password
                user.HashPassword = Protected.CreatePasswordHash(userViewModel.Password);
                //user.IsConfirmed = true;

                // Add user
                var result = Add(user);

                if (result == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                SaveChanges();

                string token = _tokenManager.GenerateUserToken(result).Data.Token;
                var confirmLink = "https://localhost:5001/api/user/confirm-account?UserId=" + user.Id.ToString()+"&Token="+token;
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("hassan.sheimy88@gmail.com", "Password"),
                    EnableSsl = true
                };
                string body = @"
                    <html>
                        <body>
                            <h1>Welcome in GotIt ya ryes</h1>
                            <h3>Please click the Button to confirm your Account,<br></br></h3>
                            <form method='GET' action ='"+confirmLink+@"'>
                                <input type = 'submit' value = 'Confirm Your Account' />
                            </form>
                        </body>
                    </html>";
                
                MailMessage msg = new MailMessage("hassan.sheimy98@yahoo.com", "hassan.sheimy88@gmail.com");
                msg.IsBodyHtml = true;
                msg.Body = body;
                msg.Subject = "ومعانا اول تيست ونقول بسم الله";
                client.Send(msg);
                return ResultHelper.Succeeded(data: true);
            }
            catch (DuplicateDataException)
            {
                return ResultHelper.Failed(data: false, message: EResultMessage.EmailExists.ToString());
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(message: e.Message);
            }
        }
        public Result<bool> confirm(int? id, string token)
        {
            try
            {
                if (id == null || token == null)
                {
                    return ResultHelper.Failed<bool>(message: "Wrong Link");
                }
                var user = Get(u => u.Id == id);
                if (user == null)
                    return ResultHelper.Failed<bool>(message: "Not found User");
                int x = _tokenManager.ExtractAttributes(_tokenManager.ValidateToken(token), new EUserType[] { user.Type }).Id; 
                if (x!=id)
                    return ResultHelper.Failed<bool>(message: "Wrong Link");
                user.IsConfirmed = true;
                Update(user);
                SaveChanges();
                return ResultHelper.Succeeded(data: true);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(message: e.Message);
            }
        }
        public Result<TokenViewModel> Login(LoginViewModel user)
        {
            try
            {
                var userResult = Get(u => u.Email.ToUpper() == user.Email.ToUpper());

                if (userResult == null)
                {
                    throw new Exception(EResultMessage.EmailOrPasswordWrong.ToString());
                }
                if (!userResult.IsConfirmed)
                {
                    throw new Exception(EResultMessage.UserNotConfirmed.ToString());
                }
                
                if (!Protected.Validate(user.Password, userResult.HashPassword))
                {
                    throw new Exception(EResultMessage.EmailOrPasswordWrong.ToString());
                }

                return _tokenManager.GenerateUserToken(userResult);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<TokenViewModel>(message: e.Message);
            }
        }
    }
}
