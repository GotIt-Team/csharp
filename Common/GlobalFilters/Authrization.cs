using GotIt.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GotIt.Common.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace GotIt.Common.GlobalFilters
{
    public class Authrization : ActionFilterAttribute
    {
        private string GOT_IT_TOKEN_SECRET_KEY;
        private readonly EUserType [] _userTypes;
        private RequestAttributes _requestAttributes;

        public Authrization(params EUserType[] userTypes)
        {
            _userTypes = userTypes;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                _requestAttributes = context.HttpContext.RequestServices.GetService<RequestAttributes>();
                GOT_IT_TOKEN_SECRET_KEY = context.HttpContext.RequestServices.GetService<IConfiguration>()["Jwt:JWTSecret"];
                string authToken = context.HttpContext.Request.Headers[HeaderNames.Authorization];

                if (string.IsNullOrWhiteSpace(authToken))
                {
                    throw new Exception();
                }

                string token = authToken.Substring("Bearer ".Length).Trim();
                var data = ValidateToken(token); 

                var userId = data.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                var name = data.Claims.FirstOrDefault(c => c.Type == "Name")?.Value;
                var email = data.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
                var type = data.Claims.FirstOrDefault(c => c.Type == "Type")?.Value;

                var userType = (EUserType)Enum.Parse(typeof(EUserType), type);
                if (!_userTypes.Contains(userType))
                {
                    throw new Exception();
                }

                _requestAttributes.Id = int.Parse(userId);
                _requestAttributes.Name = name;
                _requestAttributes.Email = email;
                _requestAttributes.Type = userType;
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
            }

            return;
        }

        private ClaimsPrincipal ValidateToken(string token)
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

            return tokenHandler.ValidateToken(token, TokenValidationParameters, out SecurityToken validatedToken);
        }
    }
}
