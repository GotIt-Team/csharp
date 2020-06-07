using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GotIt.Common.Helper;
using GotIt.MSSQL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GotIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        public ChatController()
        {
        }

        [HttpGet]
        public Result<object> GetChats()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{id}")]
        public Result<object> GetChat([FromRoute] int id)
        {
            throw new NotImplementedException();
        }
    }
}
