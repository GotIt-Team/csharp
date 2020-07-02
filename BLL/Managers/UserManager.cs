using GotIt.BLL.Providers;
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
        private readonly MailProvider _mailProvider;

        public UserManager(GotItDbContext dbContext, RequestAttributes requestAttributes, 
            TokenManager tokenManager, MailProvider mailProvider) : base(dbContext)
        {
            _requestAttributes = requestAttributes;
            _tokenManager = tokenManager;
            _mailProvider = mailProvider;
        }

        public async Task<Result<bool>> AddUser(RegisterationViewModel userViewModel)
        {
            try
            {
                var user = new UserEntity {
                    Name = userViewModel.Name,
                    Email = userViewModel.Email,
                    PhoneNumber = userViewModel.PhoneNumber,
                    Address = userViewModel.Address,
                    Type = userViewModel.Type,
                    Picture = userViewModel.Picture,
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


                await _mailProvider.SendAsync(new MailMessageViewModel
                {
                    From = MailProvider.SMTP_USER,
                    To = user.Email,
                    IsBodyHtml = true,
                    Body = body,
                    Subject = "Confirm your account on Got It"
                });

                return ResultHelper.Succeeded(true, message: EResultMessage.RegistrationDone.ToString());
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

        public Result<bool> ChangePassword(int userId, UserPasswordViewModel password)
        {
            try
            {
                if(password.NewPassword != password.RepeatedNewPassword)
                {
                    throw new Exception(EResultMessage.InvalidData.ToString());
                }

                var user = Get(u => u.Id == userId);
                if (user == null)
                {
                    throw new Exception(EResultMessage.NotFound.ToString());
                }

                if(!Protected.Validate(password.OldPassword, user.HashPassword))
                {
                    throw new Exception(EResultMessage.WrongPassword.ToString());
                }

                user.HashPassword = Protected.CreatePasswordHash(password.NewPassword);
                Update(user);

                if(!SaveChanges())
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                return ResultHelper.Succeeded(true);
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

        public Result<UserViewModel> Login(LoginViewModel user)
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
                return ResultHelper.Failed<UserViewModel>(message: e.Message);
            }
        }

        public Result<UserViewModel> GettSettings(int userId)
        {
            try
            {
                var user = Get(i => i.Id == userId);

                var result = new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Address=user.Address,
                    Picture=user.Picture,
                };
                return ResultHelper.Succeeded(result);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<UserViewModel>(message: e.Message);
            }
        }

        public Result<bool> EditSettings(int userId, UserViewModel user)
        {
            try
            {
                var data = new UserEntity
                {
                    Id = userId,
                    Name = user.Name,
                    Picture=user.Picture,
                    Address=user.Address,
                    PhoneNumber=user.PhoneNumber
                   
                };

                Update(data, i => i.Name, i => i.Picture , i => i.Address , i => i.PhoneNumber);

                var result = SaveChanges();

                if (!result)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                return ResultHelper.Succeeded(result);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(message: e.Message);
            }
        }

    }
}
