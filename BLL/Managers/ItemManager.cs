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


        public Result<List<ItemViewModel>> GetItems(int? userId, bool isLost, int pageNo, int pageSize)
        {
            try
            {
                var items = GetAllPaginated(i => i.UserId == (userId ?? i.UserId) && i.IsLost == isLost && i.MatchDate == null, pageNo, pageSize,
                    "Images", "User");
                
                if (items.Data == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                var result = items.Data.Select(i => new ItemViewModel
                {
                    Id = i.Id,
                    Content = i.Content,
                    CreationDate = i.CreationDate,
                    Type = i.Type,
                    Image = i.Images.FirstOrDefault()?.Image,
                    User = new UserViewModel
                    {
                        Name = i.User.Name,
                        Picture = i.User.Picture
                    }
                }).OrderByDescending(i => i.CreationDate).ToList();

                return ResultHelper.Succeeded(result, items.Count);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<List<ItemViewModel>>(message: e.Message);
            }
        }

        public Result<ItemDetailsViewModel> GetItemDetails(int id)
        {
            try
            {
                var item = Get(i => i.Id == id, "Images", "Attributes", "User", "Comments.User");

                if (item == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

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
                        Picture = item.User.Picture
                    },
                    
                    Comments = item.Comments.Select(i => new CommentViewModel
                    {
                        Id = i.Id,
                        Date = i.Date,
                        Content = i.Content,
                        User = new UserViewModel
                        {
                            Id = i.User.Id,
                            Name = i.User.Name,
                            Picture = i.User.Picture
                        }
                    }).ToList()
                };

                return ResultHelper.Succeeded(result);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<ItemDetailsViewModel>(message: e.Message);
            }
        }

        public bool Similar(ItemEntity item1, ItemEntity item2)
        {
            if (item1.Attributes.Count < item1.Attributes.Count)
            {
                return item1.Attributes.All(i => item2.Attributes.Any(j => j.Key == i.Key && j.Value == i.Value));
            }
            return item2.Attributes.All(i => item1.Attributes.Any(j => j.Key == i.Key && j.Value == i.Value));
        }

        public Result<bool> EditItem(int userId, int itemId, ItemViewModel item)
        {
            try
            {
                var data = new ItemEntity
                {
                    Id = itemId,
                    Content = item.Content,
                    UserId = userId
                };

                Update(data, i => i.Content);

                var result = SaveChanges();

                if (!result)
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

        public Result<bool> DeleteItem(int itemId)
        {
            try
            {
                DeleteById(itemId);

                var result = SaveChanges();

                if (!result)
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
