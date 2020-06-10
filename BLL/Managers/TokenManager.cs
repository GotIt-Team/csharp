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
                var claims = new List<Claim>();

                // add email
                claims.Add(new Claim(ClaimTypes.Email, user.Email));

                // add name
                claims.Add(new Claim(ClaimTypes.Name, user.Name));

                // add user Id
                claims.Add(new Claim("UserId", user.Id.ToString()));

                // add user type
                if (user.Type == EUserType.regular)
                {
                    claims.Add(new Claim("Type", user.Type.ToString()));
                }
                if (user.Type == EUserType.organization)
                {
                    claims.Add(new Claim("Type", user.Type.ToString()));
                }

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
                return ResultHelper.Succeeded(data: resultToken, count: 1, message: EStatusCode.ProcessSuccess.ToString());
            }
            // any un excpected error happened
            catch (Exception e)
            {
                return ResultHelper.Failed<TokenViewModel>(data: null, count:null, message: EStatusCode.GenerateTokenFaild.ToString());
            }
        }
    }
}
