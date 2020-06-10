using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GotIt.BLL.Managers;
using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Common.GlobalFilters;
using GotIt.Common.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GotIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly UserManager _userManager;
        public UserController(UserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        [Route("sign-in")]
        public Result<object> Login()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("sign-up")]
        public Result<TokenViewModel> Registration([FromBody] RegisterationViewModel newUser)
        {
            return _userManager.AddUser(newUser);
        }

        [HttpGet]
        [Route("settings")]
        public Result<object> GetSettings()
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("settings")]
        public Result<object> EditSettings()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("items/lost")]
        public Result<object> LostItems()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("items/found")]
        public Result<object> FoundItems()
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("items/{id}")]
        public Result<object> EditItem([FromRoute] int id)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("items/{id}")]
        public Result<object> RemoveItem([FromRoute] int id)
        {
            throw new NotImplementedException();
        }
    }
}
