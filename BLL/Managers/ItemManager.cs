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
                    "Person.Images", "Object", "User");
                
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
                    Image = i.Person != null ? i.Person.Images?.FirstOrDefault()?.Image : i.Object?.Image,
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
                var item = Get(i => i.Id == id, "Person.Images", "Object.Attributes", "User", "Comments.User");

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
                    
                    Person = item.Person != null ? new PersonViewModel
                    {
                        Id = item.Person.Id,
                        Name = item.Person.Name,
                        Gender = item.Person.Gender,
                        AgeStage = item.Person.AgeStage
                    } : null,
                    
                    Object = item.Object != null ? new ObjectViewModel
                    {
                        Id = item.Object.Id,
                        Class = item.Object.Class,
                        Attributes = item.Object.Attributes.ToDictionary(a => a.Key, a => a.Value)
                    } : null,

                    Images = item.Person != null ? item.Person?.Images.Select(i => i.Image).ToList() : new List<string> { item.Object?.Image },
                    
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
