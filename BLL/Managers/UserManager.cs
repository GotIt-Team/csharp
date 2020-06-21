using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Common.Exceptions;
using GotIt.Common.Helper;
using GotIt.MSSQL;
using GotIt.MSSQL.Models;
using GotIt.MSSQL.Repository;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GotIt.BLL.Managers
{
    public class UserManager : Repository<UserEntity>
    {
        private readonly TokenManager _tokenManager;
        private readonly RequestAttributes _requestAttributes;
        
        public UserManager(GotItDbContext dbContext, RequestAttributes requestAttributes, 
            TokenManager tokenManager) : base(dbContext)
        {
            _requestAttributes = requestAttributes;
            _tokenManager = tokenManager;
        }

        public async Task<Result<bool>> AddUser(RegisterationViewModel userViewModel)
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
                
                // Add user
                var result = Add(user);

                if (result == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                SaveChanges();

                var token = _tokenManager.GenerateUserToken(result).Token;
                
                var confirmLink = string.Format("{0}/api/user/confirm-account", _requestAttributes.AppBaseUrl);
                
                string body = File.ReadAllText("wwwroot/html/registartion.html");
                body = body.Replace("{link-path}", confirmLink);
                body = body.Replace("{user-name}", user.Name);
                body = body.Replace("{user-id}", user.Id.ToString());
                body = body.Replace("{user-token}", token);


                await EmailProvider.SendMailAsync(new EmailMessageViewModel
                {
                    From = EmailProvider.SMTP_USER,
                    To = user.Email,
                    IsBodyHtml = true,
                    Body = body,
                    Subject = "Confirm your account on Got It"
                });

                return ResultHelper.Succeeded(true);
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

        public Result<bool> Confirm(int? id, string token)
        {
            try
            {
                if (id == null || token == null)
                {
                    throw new Exception("Wrong Link");
                }
                var user = Get(u => u.Id == id);
                if (user == null)
                {
                    throw new Exception("Not found User");
                }
                    
                int userId = _tokenManager.ValidateToken(token, user.Type).Id; 
                if (userId != id)
                {
                    throw new Exception("Wrong Link");
                }

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

                return ResultHelper.Succeeded(_tokenManager.GenerateUserToken(userResult));
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<TokenViewModel>(message: e.Message);
            }
        }
    }
}
