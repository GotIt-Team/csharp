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
    public class ItemController : ControllerBase
    {
        public ItemController()
        {

        }

        [HttpGet]
        public Result<object> GetItems()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{id}")]
        public Result<object> GetItem([FromRoute] int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Result<object> AddItem()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("request")]
        public Result<object> ItemRequest()
        {
            throw new NotImplementedException();
        }
    }
}
