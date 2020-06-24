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
    public class ItemController : ControllerBase
    {
        private readonly ItemManager _manager;
        private readonly RequestAttributes _requestAttributes;
        private readonly RequestManager _requestManager;
        public ItemController(RequestAttributes requestAttributes, ItemManager manager, RequestManager requestManager)
        {
            _requestAttributes = requestAttributes;
            _manager = manager;
            _requestManager = requestManager;
        }

        [HttpGet]
        [Authrization(EUserType.regular)]
        public Result<List<ItemViewModel>> GetItems([FromQuery] int pageNo, [FromQuery] int pageSize)
        {
            return _manager.GetItems(null, true, pageNo, pageSize);
        }

        [HttpGet]
        [Route("{id}")]
        [Authrization(EUserType.regular)]
        public Result<ItemDetailsViewModel> GetItem([FromRoute] int id)
        {
            return _manager.GetItemDetails(id);
        }

        [HttpPost]
        public Result<object> AddItem()
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("{id}")]
        [Authrization(EUserType.regular)]
        public Result<bool> EditItem([FromRoute] int id, [FromBody] ItemViewModel item)
        {
            return _manager.EditItem(_requestAttributes.Id, id, item);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authrization(EUserType.regular)]
        public Result<bool> DeleteItem([FromRoute] int id)
        {
            return _manager.DeleteItem(id);
        }

        [HttpPost]
        [Route("request")]
        //[Authrization(EUserType.regular)]
        public Result<bool> ItemRequest([FromBody]RequestViewModel request)
        {
            return _requestManager.itemRequest(request,_requestAttributes.Id);
        }

        [HttpPost]
        [Route("replyRequest")]
        //[Authrization(EUserType.organization)]
        public Result<bool> ReplyRequest([FromBody]RequestViewModel request)
        {
            return _requestManager.ReplyRequest(request, _requestAttributes.Id);
        }
    }
}
