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

        public Result<bool> ItemRequest(int senderId, RequestViewModel requestViewModel)
        {
            try
            {
                var request = new RequestEntity
                {
                    SendDate = DateTime.UtcNow,
                    Title = requestViewModel.Title,
                    Content = requestViewModel.Content,
                    State = ERequestState.Pending,
                    ItemId = requestViewModel.Item.Id,
                    SenderId = senderId,
                    ReceiverId = requestViewModel.Receiver.Id
                };

                var data = Add(request);

                var notification = new NotificationViewModel
                {
                    Link = "link ",
                    Content = " content ",
                    Sender = new UserViewModel
                    {
                        Id = senderId
                    },
                    Type = ENotificationType.Request,
                    
                };
                var notificationResult = _notificationManager.AddNotification(requestViewModel.Receiver.Id, notification);

                if (data == null || notificationResult == null || !SaveChanges())
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }
                return ResultHelper.Succeeded(true);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(message: e.Message);
            }
        }

        public Result<bool> ReplyRequest(int senderId, RequestViewModel requestViewModel)
        {
            try
            {
                var request = Get(r => r.Id == requestViewModel.Id);
                if(request == null)
                {
                    throw new Exception(EResultMessage.NotFound.ToString());
                }

                request.ReplyDate = DateTime.UtcNow;
                request.State = requestViewModel.State;
                request.ReplyMessage = requestViewModel.ReplyMessage;

                Update(request);

                var notification = new NotificationViewModel
                {
                    Link = "link ",
                    Content = " content ",
                    Sender = new UserViewModel
                    {
                        Id = senderId
                    },
                    Type = ENotificationType.Request,
                };

                var notificationResult = _notificationManager.AddNotification(request.SenderId, notification);
                if (notificationResult == null || !SaveChanges())
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }
                return ResultHelper.Succeeded(true);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(message: e.Message);
            }
        }

        public Result<bool> DeleteRequest(int userId, int requestId, ERequestState state)
        {
            try
            {
                if(state == ERequestState.Approved)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                var request = new RequestEntity();
                if (state == ERequestState.Pending)
                {
                    request = Get(r => r.Id == requestId);
                }

                DeleteById(requestId);
                
                if(state == ERequestState.Rejected)
                {
                    return ResultHelper.Succeeded(true);
                }

                var notification = new NotificationViewModel
                {
                    Link = "link ",
                    Content = " content ",
                    Sender = new UserViewModel
                    {
                        Id = userId
                    },
                    Type = ENotificationType.Request,
                };

                var notificationResult = _notificationManager.AddNotification(request.ReceiverId, notification);
                if (notificationResult == null || !SaveChanges())
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                return ResultHelper.Succeeded(true);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(message: e.Message);
            }
        }

        public Result<List<RequestViewModel>> GetRequests(int userId, EUserType type, ERequestState? state)
        {
            try
            {
                var requests = GetAll(r => ((type == EUserType.regular && r.SenderId == userId) 
                    || (type == EUserType.organization && r.ReceiverId == userId)) && r.State == (state ?? r.State),
                    type == EUserType.regular ? "Receiver" : "Sender");
                if(requests == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                var result = requests.Select(r => new RequestViewModel
                {
                    Id = r.Id,
                    Content = r.Content,
                    Title = r.Title,
                    SendDate = r.SendDate,
                    State = r.State,
                    ReplyDate = r.ReplyDate,
                    ReplyMessage = r.ReplyMessage,
                    Receiver = r.Receiver != null ? new UserViewModel
                    {
                        Id = r.Receiver.Id,
                        Name = r.Receiver.Name,
                        Picture = r.Receiver.Picture
                    } : null,
                    Sender = r.Sender != null ? new UserViewModel
                    {
                        Id = r.Sender.Id,
                        Name = r.Sender.Name,
                        Picture = r.Sender.Picture
                    } : null
                }).ToList();

                return ResultHelper.Succeeded(result);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<List<RequestViewModel>>(message: e.Message);
            }
        }
    }
}
