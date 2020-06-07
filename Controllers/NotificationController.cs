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
    public class NotificationController : ControllerBase
    {
        public NotificationController()
        {

        }

        [HttpGet]
        public Result<object> GetNotifications()
        {
            throw new NotImplementedException();
        }
    }
}
