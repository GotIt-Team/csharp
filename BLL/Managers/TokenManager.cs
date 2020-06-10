using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using GotIt.Common.Helper;
using GotIt.BLL.ViewModels;
using GotIt.MSSQL.Models;
using System.Text;
using GotIt.Common.Enums;

namespace GotIt.BLL.Managers
{
    public class TokenManager
    {
        private readonly IConfiguration _configuration;
        
        public TokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Result<TokenViewModel> GenerateUserToken(UserEntity user)
        {
            try
            {
                //symmetric security key
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:JWTSecret"]));

                // signing credentials
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                // add claims
                var claims = new List<Claim>
                {

                    // add email
                    new Claim("Email", user.Email),

                    // add name
                    new Claim("Name", user.Name),

                    // add user Id
                    new Claim("Id", user.Id.ToString()),

                    // add user type
                    new Claim("Type", user.Type.ToString())
                };

                // Token security information
                var token = new JwtSecurityToken(
                    issuer: "GotIt",
                    audience: "GotIt-Users",
                    expires: DateTime.Now.AddYears(1),
                    signingCredentials: signingCredentials,
                    claims: claims
                );

                // result
                var resultToken = new TokenViewModel
                {
                    Name = user.Name,
                    Country = user.Country,
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                };

                //return token
                return ResultHelper.Succeeded(resultToken);
            }
            // any un excpected error happened
            catch (Exception)
            {
                return ResultHelper.Failed<TokenViewModel>(message: EResultMessage.GenerateTokenFaild.ToString());
            }
        }
    }
}
