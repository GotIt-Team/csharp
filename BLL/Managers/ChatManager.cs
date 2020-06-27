using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Common.Helper;
using GotIt.MSSQL;
using GotIt.MSSQL.Models;
using GotIt.MSSQL.Repository;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.BLL.Managers
{
    public class ChatManager : Repository<ChatEntity>
    {
        private readonly RequestAttributes _requestAttributes;

        public ChatManager(GotItDbContext dbContext, RequestAttributes requestAttributes) : base(dbContext)
        {
            _requestAttributes = requestAttributes;
        }
        public Result<List<ChatViewModel>> GetChatList(int userId)
        {
            try
            {
                var chatList = GetAll(i => i.Users.Any(j => j.UserId == userId),
                    "Messages.User", "Users.User");

                if (chatList == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                var result = chatList.Select(i =>
                {
                    var user = i.Users.FirstOrDefault(j => userId != j.UserId).User;
                    var lastMessage = i.Messages.OrderByDescending(j => j.Time).FirstOrDefault();

                    return new ChatViewModel
                    {
                        Id = i.Id,
                        User = new UserViewModel
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Picture = user.Picture
                        },
                        LastMessage = lastMessage == null ? null : new MessageViewModel
                        {
                            Content = lastMessage.Content,
                            Time = lastMessage.Time,
                            Type = lastMessage.Type,
                            Sender = new UserViewModel
                            {
                                Id = lastMessage.User.Id,
                                Name = lastMessage.User.Name,
                                Picture = lastMessage.User.Picture
                            }
                        }
                    };
                }).ToList();

                return ResultHelper.Succeeded(result);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<List<ChatViewModel>>(message: e.Message);
            }
        }
    }
}
