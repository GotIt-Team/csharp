using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Common.Exceptions;
using GotIt.Common.Helper;
using GotIt.MSSQL;
using GotIt.MSSQL.Models;
using GotIt.MSSQL.Repository;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
        
        public Result<TokenViewModel> AddUser(RegisterationViewModel userViewModel)
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
                user.IsConfirmed = true;

                // Add user
                var result = Add(user);

                if (result == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                SaveChanges();

                return _tokenManager.GenerateUserToken(result);
            }
            catch (DuplicateDataException)
            {
                return ResultHelper.Failed<TokenViewModel>(message: EResultMessage.EmailExists.ToString());
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<TokenViewModel>(message: e.Message);
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
