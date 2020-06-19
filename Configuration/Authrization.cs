using GotIt.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System;
using Microsoft.Extensions.DependencyInjection;
using GotIt.BLL.Managers;
using GotIt.Common.Helper;

namespace GotIt.Configuration
{
    public class Authrization : ActionFilterAttribute
    {
        private readonly EUserType [] _userTypes;
        
        public Authrization(params EUserType[] userTypes)
        {
            _userTypes = userTypes;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var requestAttributes = context.HttpContext.RequestServices.GetService<RequestAttributes>();
                var tokenManager = context.HttpContext.RequestServices.GetService<TokenManager>();
                string authToken = context.HttpContext.Request.Headers[HeaderNames.Authorization];

                if (string.IsNullOrWhiteSpace(authToken))
                {
                    throw new Exception();
                }

                string token = authToken.Substring("Bearer ".Length).Trim();
                var claimsPrincipal = tokenManager.ValidateToken(token);

                requestAttributes.CopyFrom(tokenManager.ExtractAttributes(claimsPrincipal, _userTypes));
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
            }

            return;
        }
    }
}
