using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GotIt.BLL.Managers;
using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Configuration;
using GotIt.Common.Helper;
using GotIt.MSSQL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GotIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationManager _notificationManager;
        private readonly RequestAttributes _requestAttributes;
        public NotificationController(RequestAttributes requestAttributes, NotificationManager notificationManager)
        {
            _requestAttributes = requestAttributes;
            _notificationManager = notificationManager;
        }

        /// <summary>
        /// Get paginated notifications for specific user
        /// </summary>
        /// <param name="pageNo">page number on pagination system</param>
        /// <param name="pageSize">number of items on the page</param>
        /// <returns></returns>
        [HttpGet]
        [Authrization(EUserType.regular)]
        public Result<List<NotificationViewModel>> GetNotifications([FromQuery] int pageNo, [FromQuery] int pageSize)
        {
            return _notificationManager.GetUserNotifications(_requestAttributes.Id, pageNo, pageSize);
        }

        /// <summary>
        /// Mark notification as read (isSeen = true)
        /// </summary>
        /// <param name="id">notification id</param>
        /// <param name="notification">notification model</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/read")]
        [Authrization(EUserType.regular)]
        public Result<bool> ReadNotification([FromRoute] int id, [FromBody] NotificationViewModel notification)
        {
            return _notificationManager.ReadNotification(_requestAttributes.Id, id, notification);
        }
    }
}
