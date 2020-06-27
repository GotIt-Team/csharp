using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GotIt.BLL.Managers;
using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Common.Helper;
using GotIt.Configuration;
using GotIt.MSSQL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GotIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly RequestAttributes _requestAttributes;
        private readonly ChatManager _manager;
        private readonly MessageManager _messageManager;
        public ChatController(RequestAttributes requestAttributes , ChatManager manager , MessageManager messageManager)
        {
            _requestAttributes = requestAttributes;
            _manager = manager;
            _messageManager = messageManager;
        }

        [HttpGet]
        [Authrization(EUserType.regular, EUserType.organization)]
        public Result<List<ChatViewModel>> GetChats()
        {
            return _manager.GetChatList(_requestAttributes.Id);
        }

        [HttpGet]
        [Route("{id}")]
        public Result<List<MessageViewModel>> GetChatMessages([FromRoute] int id)
        {
            return _messageManager.GetMessages(id);
        }
    }
}
