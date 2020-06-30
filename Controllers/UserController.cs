using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GotIt.BLL.Managers;
using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Configuration;
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
        private readonly RequestManager _requestManager;
        private readonly RequestAttributes _requestAttributes;

        public UserController(RequestAttributes requestAttributes, UserManager manager, ItemManager itemManager, RequestManager requestManager)
        {
            _requestAttributes = requestAttributes;
            _manager = manager;
            _itemManager = itemManager;
            _requestManager = requestManager;
        }

        [HttpPost]
        [Route("sign-in")]
        public Result<TokenViewModel> Login([FromBody] LoginViewModel user)
        {
            return _manager.Login(user);
        }

        [HttpPost]
        [Route("sign-up")]
        public async Task<Result<bool>> Registration([FromBody] RegisterationViewModel newUser)
        {
            return await _manager.AddUser(newUser);
        }

        [HttpGet]
        [Route("confirm-account")]
        public Result<bool> Confirm([FromQuery]int UserId, [FromQuery]string Token)
        {
            return _manager.Confirm(UserId, Token);
        }

        [HttpGet]
        [Route("settings")]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<UserViewModel> GetSettings()
        {
            return _manager.GettSettings(_requestAttributes.Id);
        }

        [HttpPut]
        [Route("settings")]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<bool> EditSettings([FromBody] UserViewModel User)
        {
            return _manager.EditSettings(_requestAttributes.Id , User);
        }

        [HttpPut]
        [Route("change-password")]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<bool> ChangePassword([FromBody] UserPasswordViewModel password)
        {
            return _manager.ChangePassword(_requestAttributes.Id, password);
        }

        [HttpGet]
        [Route("items")]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<List<ItemViewModel>> LostItems([FromQuery] bool isLost, [FromQuery] int pageNo, [FromQuery] int pageSize)
        {
            return _itemManager.GetItems(_requestAttributes.Id, isLost, pageNo, pageSize);
        }

        [HttpGet]
        [Route("requests")]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<List<RequestViewModel>> GetRequests([FromQuery] ERequestState? state)
        {
            return _requestManager.GetRequests(_requestAttributes.Id, _requestAttributes.Type, state);
        }

        [HttpDelete]
        [Route("request/{id}")]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<bool> DeleteRequest([FromRoute] int id, [FromQuery] ERequestState state)
        {
            return _requestManager.DeleteRequest(_requestAttributes.Id, id, state);
        }
    }
}
