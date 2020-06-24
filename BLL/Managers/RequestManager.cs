using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Common.Helper;
using GotIt.MSSQL;
using GotIt.MSSQL.Models;
using GotIt.MSSQL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.BLL.Managers
{
    public class RequestManager : Repository<RequestEntity>
    {
        public readonly NotificationManager _notificationManager;
        public RequestManager(GotItDbContext dbContext, NotificationManager notificationManager) : base(dbContext) {
            _notificationManager = notificationManager;
        }

        public Result<bool> itemRequest(RequestViewModel requestViewModel,int senderId)
        {
            try
            {
                var request = new RequestEntity {
                    SendDate=DateTime.UtcNow,
                    State=requestViewModel.State,
                    ItemId=requestViewModel.Item.Id,
                    UserId=requestViewModel.User.Id
                };

                Add(request);
                var result = SaveChanges();
                if (!result)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }
                var notification = new NotificationEntity
                {
                    Link = "link ",
                    Content = " content ",
                    IsSeen = false,
                    SenderId = senderId,
                    ReceiverId = requestViewModel.User.Id,
                    Type = ENotificationType.Request
                };
                
                return _notificationManager.RequestNotification(notification);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(false,message: e.Message);
            }
        }

        public Result<bool> ReplyRequest(RequestViewModel requestViewModel, int senderId)
        {
            try
            {
                var request = Get(r => r.Id == requestViewModel.Id, "Item.User");
                request.ReplyDate = DateTime.UtcNow;
                request.State = requestViewModel.State;
                request.ReplyMessage = requestViewModel.ReplyMessage;

                Update(request);
                var result = SaveChanges();
                if (!result)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }
                var notification = new NotificationEntity
                {
                    Link = "link ",
                    Content = " content ",
                    IsSeen = false,
                    SenderId = senderId,
                    ReceiverId = request.Item.User.Id,
                    Type = ENotificationType.Request,
                    Date = DateTime.UtcNow
                };

                return _notificationManager.RequestNotification(notification);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(false, message: e.Message);
            }
        }
    }
}
