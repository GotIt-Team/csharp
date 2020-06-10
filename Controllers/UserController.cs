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
        public Result<TokenViewModel> Login([FromBody] UserLoginViewModel user)
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
        [Authrization(EUserType.regular)]
        public Result<List<UserItemViewModel>> LostItems([FromQuery] bool isLost, [FromQuery] int pageNo, [FromQuery] int pageSize)
        {
            return _itemManager.GetUserItems(_requestAttributes.Id, isLost, pageNo, pageSize);
        }

        [HttpPut]
        [Route("items/{id}")]
        [Authrization(EUserType.regular)]
        public Result<object> EditItem([FromRoute] int id)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("items/{id}")]
        [Authrization(EUserType.regular)]
        public Result<bool> RemoveItem([FromRoute] int id)
        {
            return _itemManager.DeleteItem(id);
        }
    }
}
