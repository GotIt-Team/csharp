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
        private readonly CommentManager _commentManager;
        public ItemController(RequestAttributes requestAttributes, 
            ItemManager manager, 
            RequestManager requestManager,
            CommentManager commentManager)
        {
            _requestAttributes = requestAttributes;
            _manager = manager;
            _requestManager = requestManager;
            _commentManager = commentManager;
        }

        [HttpGet]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<List<ItemDetailsViewModel>> GetItems([FromQuery] int pageNo, [FromQuery] int pageSize)
        {
            return _manager.GetItems(null, true, pageNo, pageSize);
        }

        [HttpGet]
        [Route("{id}")]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<ItemDetailsViewModel> GetItem([FromRoute] int id)
        {
            return _manager.GetItemDetails(id);
        }

        [HttpPost]
        [Authrization(EUserType.regular)]
        public Result<bool> AddItem([FromBody] ItemDetailsViewModel item)
        {
            return _manager.AddItem(_requestAttributes.Id, item);
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
        [Authrization(EUserType.regular)]
        public Result<bool> ItemRequest([FromBody]RequestViewModel request)
        {
            return _requestManager.ItemRequest(_requestAttributes.Id, request);
        }

        [HttpPost]
        [Route("request/reply")]
        [Authrization(EUserType.organization)]
        public Result<bool> ReplyRequest([FromBody]RequestViewModel request)
        {
            return _requestManager.ReplyRequest(_requestAttributes.Id, request);
        }

        [HttpGet]
        [Route("{id}/comments")]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<List<CommentViewModel>> GetItemComments([FromRoute] int id, [FromQuery] int pageNo, [FromQuery] int pageSize)
        {
            return _commentManager.GetItemComments(id, pageNo, pageSize);
        }

        [HttpPost]
        [Route("{id}/comment")]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<int> AddComment([FromRoute] int id ,[FromBody] CommentViewModel comment)
        {
            return _commentManager.AddComment(_requestAttributes.Id,id,comment);
        }
    }
}
