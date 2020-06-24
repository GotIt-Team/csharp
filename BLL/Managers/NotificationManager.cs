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
    public class NotificationManager : Repository<NotificationEntity>
    {
        public NotificationManager(GotItDbContext dbContext) : base(dbContext) {}

        public Result<List<NotificationViewModel>> GetUserNotifications(int userId, int pageNo, int pageSize)
        {
            try
            {
                var notifications = GetAllPaginated(n => n.ReceiverId == userId, pageNo, pageSize, "Sender");

                if(notifications.Data == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                var result = notifications.Data.Select(n => new NotificationViewModel
                {
                    Id = n.Id,
                    Content = n.Content,
                    IsSeen = n.IsSeen,
                    Link = n.Link,
                    type = n.Type,
                    Date = n.Date,
                    Sender = new UserViewModel
                    {
                        Id = n.Sender.Id,
                        Name = n.Sender.Name,
                        Picture = n.Sender.Picture
                    }
                }).ToList();

                return ResultHelper.Succeeded(result, notifications.Count);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<List<NotificationViewModel>>(message: e.Message);
            }
        }

        public Result<bool> ReadNotification(int userId, int notificationId, NotificationViewModel notification)
        {
            try
            {
                var model = new NotificationEntity
                {
                    Id = notificationId,
                    IsSeen = true,
                    Content = notification.Content,
                    Link = notification.Link,
                    ReceiverId = userId,
                    SenderId = notification.Sender.Id
                };
                Update(model);

                var result = SaveChanges();

                if (!result)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                return ResultHelper.Succeeded(result);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed(false, message: e.Message);
            }
        }
        public Result<bool> AddNotification(NotificationViewModel notification, int receiverId)
        {
            try
            {
                var model = new NotificationEntity
                {
                    IsSeen = false,
                    Content = notification.Content,
                    Link = notification.Link,
                    ReceiverId = receiverId,
                    SenderId = notification.Sender.Id,
                    Type = notification.type,
                    Date = DateTime.UtcNow
                };
                Add(model);
                var result = SaveChanges();
                if (!result)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }
                return ResultHelper.Succeeded(result);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed(false, message: e.Message);
            }
        }

        public Result<bool> RequestNotification(NotificationEntity notification)
        {
            try
            {
                Add(notification);
                var result = SaveChanges();
                if (!result)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }
                return ResultHelper.Succeeded(result);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed(false, message: e.Message);
            }
        }
    }
}
