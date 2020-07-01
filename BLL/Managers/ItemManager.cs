using GotIt.BLL.Providers;
using GotIt.BLL.ViewModels;
using GotIt.Common.Enums;
using GotIt.Common.Helper;
using GotIt.MSSQL;
using GotIt.MSSQL.Models;
using GotIt.MSSQL.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
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
        private readonly IServiceScopeFactory _services;
        private readonly HttpProvider _httpProvider;
        public ItemManager(GotItDbContext dbContext, 
            IServiceScopeFactory services, 
            HttpProvider httpProvider) : base(dbContext) 
        {
            _services = services;
            _httpProvider = httpProvider;
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

        public async Task Match(ItemEntity item, ItemDetailsViewModel itemDetails)
        {
            try
            {
                using IServiceScope scope = _services.CreateScope();
                var _probablyMatchManager = scope.ServiceProvider.GetRequiredService<ProbablyMatchManager>();
                var _notificationManager = scope.ServiceProvider.GetRequiredService<NotificationManager>();
                var _itemManager = scope.ServiceProvider.GetRequiredService<ItemManager>();

                var data = _itemManager.GetAll(i => i.Type == item.Type && i.IsLost != item.IsLost);
                if (data == null)
                {
                    data = new List<ItemEntity>();
                }

                data = data.Where(k =>
                {
                    if (item.Attributes.Count < k.Attributes.Count)
                    {
                        return item.Attributes.All(i => k.Attributes.Any(j => j.Key == i.Key && j.Value == i.Value));
                    }
                    return k.Attributes.All(i => item.Attributes.Any(j => j.Key == i.Key && j.Value == i.Value));
                }).ToList();

                var requestData = new MatchRequestViewModel
                {
                    Known = new KnownViewModel
                    {
                        Boxes = itemDetails.Boxes,
                        Embeddings = item.Embeddings,
                        Images = itemDetails.Images
                    },
                    Candidates = data.Select(i => new CandidateViewModel
                    {
                        Embeddings = i.Embeddings,
                        ItemId = i.Id
                    }).ToList()
                };
                var path = string.Format("{0}/match", item.Type.ToString().ToLower());

                var result = await _httpProvider.SendRequest<MatchRequestViewModel, Result<MatchResultViewModel>>
                    (_httpProvider.PythonUrl, path, requestData);
                if (!result.IsSucceeded)
                {
                    throw new Exception(result.Message);
                }

                item.Embeddings = result.Data.Embeddings;
                _itemManager.Update(item, i => i.Embeddings);
                if (result.Data.Scores.Count == 0)
                {
                    if (!_itemManager.SaveChanges())
                    {
                        throw new Exception(EResultMessage.DatabaseError.ToString());
                    }
                    return;
                }

                var propMatch = result.Data.Scores.Select(i => new ProbablyMatchEntity
                {
                    ItemId = item.Id,
                    MatchedItemId = i.ItemId,
                    Score = i.Score
                }).ToList();
                _probablyMatchManager.Add(propMatch);

                var notification = _notificationManager.AddNotification(item.UserId, new NotificationViewModel
                {
                    Content = "matched item",
                    Link = "",
                    Type = ENotificationType.Match
                });

                if (notification == null || !_itemManager.SaveChanges())
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }
                return;
            }
            catch (Exception)
            {
                return;
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
                Task.Run(() => Match(data, item));
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
