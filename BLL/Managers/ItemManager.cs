using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Common.Helper;
using GotIt.MSSQL;
using GotIt.MSSQL.Models;
using GotIt.MSSQL.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GotIt.BLL.Managers
{
    public class ItemManager : Repository<ItemEntity>
    {
        private readonly ProbablyMatchManager _probablyMatchManager;
        private readonly NotificationManager _notificationManager;
        public ItemManager(GotItDbContext dbContext , ProbablyMatchManager probablyMatchManager , NotificationManager notificationManager) : base(dbContext) {
            _probablyMatchManager = probablyMatchManager;
            _notificationManager = notificationManager;
        }


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

        public async Task Match(ItemEntity item)
        {
            try
            {
                var data = GetAll(i => i.Type == item.Type && i.IsLost != item.IsLost);
                data.Where(k =>
                {
                    if (item.Attributes.Count < k.Attributes.Count)
                    {
                        return item.Attributes.All(i => k.Attributes.Any(j => j.Key == i.Key && j.Value == i.Value));
                    }
                    return k.Attributes.All(i => item.Attributes.Any(j => j.Key == i.Key && j.Value == i.Value));
                }).ToList();
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:54040/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync(" ", data);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.StatusCode.ToString());
                }
                var result = await response.Content.ReadAsAsync<Result<MatchResultViewModel>>();
                if (!result.IsSucceeded)
                {
                    throw new Exception(result.Message);
                }
                item.Embeddings = result.Data.Embeddings;
                Update(item, i => i.Embeddings);
                if(result.Data.Scores.Count==0)
                {
                    return;
                }
                var propMatch = result.Data.Scores.Select(i => new ProbablyMatchEntity
                {
                    ItemId = item.Id,
                    MatchedItemId = i.ItemId,
                    Score = i.Score
                }).ToList();
                _probablyMatchManager.Add(propMatch);
                _notificationManager.AddNotification(item.UserId, new NotificationViewModel
                {
                    Content = "matched item",
                    Link = "",
                    Type = ENotificationType.Match
                }) ;
                var _result = SaveChanges();
                if (!_result)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return ;
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

        public Result<bool> AddItem(int userId,  ItemDetailsViewModel item)
        {
            try
            {
                var data = new ItemEntity
                {
                    Id = item.Id,
                    Content = item.Content,
                    IsLost = item.IsLost,
                    CreationDate = item.CreationDate,
                    Type = item.Type,
                    Embeddings = item.Embeddings,
                    Attributes = item.Attributes.Select(i => new ItemAttributeEntity
                    {
                        Key = i.Key,
                        Value = i.Value,
                    }).ToHashSet(),
                    UserId = userId,
                    Images = item.Images.Select(i => new ItemImageEntity
                    {
                        Image = i
                    }).ToHashSet()
                };

                var result = Add(data);
                if (result==null || !SaveChanges())
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }
                Task.Run(() => Match(data));
                return ResultHelper.Succeeded(true);
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
