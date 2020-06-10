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
    public class ItemManager : Repository<ItemEntity>
    {
        public ItemManager(GotItDbContext dbContext) : base(dbContext) {}

        public Result<List<UserItemViewModel>> GetUserItems(int userId, bool isLost, int pageNo, int pageSize)
        {
            try
            {
                var items = GetAllPaginated(i => i.UserId == userId && i.IsLost == isLost && i.MatchDate == null, pageNo, pageSize,
                    "Person.Images", "_Object");

                if (items.Data == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                var result = items.Data.Select(i => new UserItemViewModel
                {
                    Id = i.Id,
                    Content = i.Content,
                    CreationDate = i.CreationDate,
                    Type = i.Type,
                    Image = i.Person != null ? i.Person.Images?.FirstOrDefault()?.Image : i._Object?.Image,
                    IsLost = i.IsLost
                }).ToList();

                return ResultHelper.Succeeded(result, items.Count);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<List<UserItemViewModel>>(message: e.Message);
            }
        }

        public Result<bool> DeleteItem(int itemId)
        {
            try
            {
                var result = DeleteById(itemId);

                if (result == false)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                return ResultHelper.Succeeded(result);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(message: e.Message);
            }
        }
    }
}
