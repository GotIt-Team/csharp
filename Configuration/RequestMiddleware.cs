using GotIt.Common.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.Configuration
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;
        private RequestAttributes _requestAttributes;

        public RequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, RequestAttributes requestAttributes)
        {
            
            _requestAttributes = requestAttributes;
            BeforExecution(httpContext);
            await _next(httpContext);
        }
        private void BeforExecution(HttpContext httpContext)
        {
            _requestAttributes.AppBaseUrl = String.Format("{0}://{1}", httpContext.Request.Scheme, httpContext.Request.Host.Value);
        }
    }
}
