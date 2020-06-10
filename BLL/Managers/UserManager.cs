using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
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
        public UserManager(GotItDbContext dbContext, TokenManager tokenManager) : base(dbContext)  { _tokenManager = tokenManager; }
        public Result<TokenViewModel> AddUser(RegisterationViewModel userViewModel)
        {
            try
            {
                var user = new UserEntity{ Name = userViewModel.FirstName + " " + userViewModel.LastName };
                
                // Check for Type
                if (!Enum.IsDefined(typeof(EUserType), userViewModel.Type))
                {
                    return  ResultHelper.Failed<TokenViewModel>(data: null, count : null);
                }

                // Add user Typr
                user.Type = userViewModel.Type;

                if (userViewModel.Password != userViewModel.RepeatPassword)
                {
                    return ResultHelper.Failed<TokenViewModel>(data: null, count : null );
                }

                if (ExistUser(user.Email, out EStateusCode errors))
                {
                    return ResultHelper.Failed<TokenViewModel>(data: null, count:null, message: errors.ToString());
                }
                // Hash user password
                user.HashPassword = Protected.CreatePasswordHash(userViewModel.Password);
                user.IsConfirmed = true;

                // Add user
                var result = Add(user);

                if (result == null)
                {
                    return ResultHelper.Failed<TokenViewModel > (data: null, count : null);
                }

                SaveChanges();

                return _tokenManager.GenerateUserToken(result);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<TokenViewModel>(data: null, count:null , message: e.Message);
            }
        }

        public Result<TokenViewModel> Login(UserLoginViewModel user)
        {
            try
            {
                var userResult = Get(u => u.Email == user.Email.ToUpper(), new string[] { "Teacher", "Student", "Roles.Role" });

                if (userResult == null)
                {
                    return ResultHelper.Failed<TokenViewModel>(data: null, count:null,  message: "Email or Password is wrong");
                }
                if (!userResult.IsConfirmed)
                {
                    return ResultHelper.Failed<TokenViewModel>(data: null, count:null, message: "Email isn't confirmed");
                }
                
                if (!Protected.Validate(user.Password, userResult.HashPassword))
                {
                    return ResultHelper.Failed<TokenViewModel>(data: null, count:null, message: "Email or Password is wrong");
                }

                return _tokenManager.GenerateUserToken(userResult);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<TokenViewModel>(data: null, count:null, message: e.Message);
            }
        }
        private bool ExistUser(string email, out EStateusCode errors)
        {
            bool result = false;
            errors = EStateusCode.InvalidData;
            // Check if email exist
            var emailResult = ExistEmail(email);
            if (emailResult.IsSucceeded && emailResult.Data)
            {
                errors = result ? EStateusCode.UserNameEmailExists : EStateusCode.EmailExists;
            }
            result |= (!emailResult.IsSucceeded || emailResult.Data);

            return result;
        }
        public Result<bool> ExistEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                if (addr.Address != email)
                {
                    return ResultHelper.Failed(data: false, count: null);
                }

                var emailResult = Any(u => u.Email == email);
                if (!emailResult.IsSucceeded)
                {
                    return ResultHelper.Failed(data: false, count: null);
                }

                return emailResult;
            }
            catch (Exception e)
            {
                return ResultHelper.Failed(data: false, count : null , message: e.Message);
            }
        }

    }
}
