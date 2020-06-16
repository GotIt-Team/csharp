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
    public class ItemController : ControllerBase
    {
        private readonly ItemManager _itemManager;
        private readonly RequestAttributes _requestAttributes;
        public ItemController(ItemManager itemManager, RequestAttributes requestAttributes)
        {
            _itemManager = itemManager;
            _requestAttributes = requestAttributes;
        }

        [HttpGet]
        [Route("Items")]
        [Authrization(EUserType.regular)]
        public Result<List<ItemViewModel>> GetItems([FromQuery] bool isLost, [FromQuery] int pageNo, [FromQuery] int pageSize)
        {
            return _itemManager.GetItems(_requestAttributes.Id, isLost, pageNo, pageSize);
        }

        [HttpGet]
        [Route("Detail/{id}")]
        [Authrization(EUserType.regular)]
        public Result<ItemDetailsViewModel> GetItem([FromRoute] int id)
        {
            return _itemManager.GetItem(id);
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
