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
    public class MessageManager : Repository<MessageEntity>
    {
        public MessageManager(GotItDbContext dbContext) : base(dbContext) {}

        public Result<List<MessageViewModel>> GetMessages(int chatId)
        {
            try
            {
                var Messages = GetAll(i => i.ChatId == chatId);

                if (Messages == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                var result = Messages.Select(i =>
                {
                    return new MessageViewModel
                    {
                        Id = i.Id,
                        Content = i.Content,
                        Time = i.Time,
                        Type = i.Type,
                        SenderId = i.UserId
                    };
                }).ToList();

                return ResultHelper.Succeeded(result);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<List<MessageViewModel>>(message: e.Message);
            }
        }


    }
}
