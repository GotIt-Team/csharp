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
        private readonly string GOT_IT_TOKEN_SECRET_KEY;

        public TokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
            GOT_IT_TOKEN_SECRET_KEY = _configuration["Jwt:JWTSecret"];
        }

        private RequestAttributes ExtractAttributes(ClaimsPrincipal claimsPrincipal, EUserType [] userTypes)
        {
            try
            {
                var userId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                var name = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "Name")?.Value;
                var email = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
                var type = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "Type")?.Value;

                var userType = (EUserType)Enum.Parse(typeof(EUserType), type);
                if (!userTypes.Contains(userType))
                {
                    throw new Exception("User type is not allowed to access method");
                }

                return new RequestAttributes
                {
                    Id = int.Parse(userId),
                    Name = name,
                    Email = email,
                    Type = userType
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public RequestAttributes ValidateToken(string token, params EUserType[] userTypes)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GOT_IT_TOKEN_SECRET_KEY));
                var TokenValidationParameters = new TokenValidationParameters
                {
                    //what to validate
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    //setup validate data
                    ValidIssuer = "GotIt",
                    ValidAudience = "GotIt-Users",
                    IssuerSigningKey = symmetricSecurityKey
                };

                var claimsPrincipal = tokenHandler.ValidateToken(token, TokenValidationParameters, out SecurityToken validatedToken);

                return ExtractAttributes(claimsPrincipal, userTypes);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public TokenViewModel GenerateUserToken(UserEntity user)
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
                return new TokenViewModel
                {
                    Name = user.Name,
                    Picture = user.Picture,
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                };
            }
            // any un excpected error happened
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
