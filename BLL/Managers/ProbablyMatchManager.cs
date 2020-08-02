using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Common.Helper;
using GotIt.MSSQL;
using GotIt.MSSQL.Models;
using GotIt.MSSQL.Repository;
using System;
using System.Linq;

namespace GotIt.BLL.Managers
{
    public class ProbablyMatchManager : Repository<ProbablyMatchEntity>
    {
        public ProbablyMatchManager(GotItDbContext dbContext): base(dbContext) {}

        public void NextMatch(int itemId, int take)
        {
            try
            {
                var items = GetAll(i => i.ItemId == itemId, "MatchedItem.Images", "MatchedItem.Attributes", "MatchedItem.User");
                if(items == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }
                if(items.Count == 0)
                {
                    throw new Exception(EResultMessage.NotFound.ToString());
                }

                var item = items.OrderByDescending(i => i.Score).Skip(take).FirstOrDefault().MatchedItem;
                var result = new ItemDetailsViewModel
                {
                    Id = item.Id,
                    Content = item.Content,
                    CreationDate = item.CreationDate,
                    Type = item.Type,
                    IsLost = item.IsLost,
                    Attributes = item.Attributes.ToDictionary(i => i.Key, i => i.Value),

                    Images = item.Images.Select(i => i.Image).ToList(),

                    User = new UserViewModel
                    {
                        Id = item.Id,
                        Name = item.User.Name,
                        Picture = item.User.Picture,
                        Address = item.User.Address,
                        PhoneNumber = item.User.PhoneNumber
                    },
                };
            }
            catch (Exception)
            {
                return;  
            }
        }
    }
}