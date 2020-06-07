using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GotIt.Common.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GotIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        public SystemController()
        {
        }

        [HttpPost]
        [Route("feedback")]
        public Result<object> Feedback()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("contact-us")]
        public Result<object> ContactUs()
        {
            throw new NotImplementedException();
        }
    }
}
