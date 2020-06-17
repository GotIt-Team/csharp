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
        private readonly UserManager _manager;
        private readonly ItemManager _itemManager;
        private readonly RequestAttributes _requestAttributes;

        public UserController(RequestAttributes requestAttributes, UserManager manager, ItemManager itemManager)
        {
            _requestAttributes = requestAttributes;
            _manager = manager;
            _itemManager = itemManager;
        }

        [HttpPost]
        [Route("sign-in")]
        public Result<TokenViewModel> Login([FromBody] LoginViewModel user)
        {
            return _manager.Login(user);
        }

        [HttpPost]
        [Route("sign-up")]
        public Result<TokenViewModel> Registration([FromBody] RegisterationViewModel newUser)
        {
            return _manager.AddUser(newUser);
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
        [Route("items")]
        public Result<List<ItemViewModel>> LostItems([FromQuery] bool isLost, [FromQuery] int pageNo, [FromQuery] int pageSize)
        {
            return _itemManager.GetItems(_requestAttributes.Id, isLost, pageNo, pageSize);
        }
    }
}
