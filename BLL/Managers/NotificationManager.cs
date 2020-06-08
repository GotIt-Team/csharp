using GotIt.BLL.ViewModels;
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
                var notifications = GetAllPaginated(n => n.UserId == userId, pageNo, pageSize);

                if(notifications.Data == null)
                {
                    throw new Exception("Unexpected error happend in database");
                }

                var result = notifications.Data.Select(n => new NotificationViewModel
                {
                    Id = n.Id,
                    Content = n.Content,
                    IsSeen = n.IsSeen,
                    Link = n.Link
                }).ToList();

                return ResultHelper.Succeeded(result, notifications.Count);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<List<NotificationViewModel>>(null, message: e.Message);
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
                    UserId = userId
                };
                var result = Update(model);


                if (!result)
                {
                    throw new Exception("Unexpected error happend in database");
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
