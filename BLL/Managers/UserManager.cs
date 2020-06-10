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
                var user = new UserEntity{ 
                    Name = userViewModel.FirstName + " " + userViewModel.LastName ,
                    Email=userViewModel.Email,
                    PhoneNumber = userViewModel.PhoneNumber,
                    City = userViewModel.City,
                    Type = userViewModel.Type,
                    Country = userViewModel.Country,
                    Gender = userViewModel.Gender,
                };
                
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
                var userResult = Get(u => u.Email == user.Email);

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
    }
}
