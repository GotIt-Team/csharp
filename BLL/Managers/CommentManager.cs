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
    public class CommentManager : Repository<CommentEntity>
    {
        public CommentManager(GotItDbContext dbContext) : base(dbContext) {}


        public Result<List<CommentViewModel>> GetItemComments(int itemId, int pageNo, int pageSize)
        {
            try
            {
                var comments = GetAllPaginated(c => c.ItemId == itemId, pageNo, pageSize, "User");
                if(comments.Data == null)
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                var result = comments.Data.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    Date = c.Date,
                    User = new UserViewModel
                    {
                        Id = c.User.Id,
                        Name = c.User.Name,
                        Picture = c.User.Picture
                    }
                }).ToList();

                return ResultHelper.Succeeded(result, result.Count);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<List<CommentViewModel>>(message: e.Message);
            }
        }

        public Result<int> AddComment(int userId, int itemId, CommentViewModel comment)
        {
            try
            {
                var data = new CommentEntity
                {
                    Content = comment.Content,
                    Date = DateTime.UtcNow,
                    ItemId = itemId,
                    UserId = userId
                };
                var result = Add(data);

                if (result == null || !SaveChanges())
                {
                    throw new Exception(EResultMessage.DatabaseError.ToString());
                }

                return ResultHelper.Succeeded(result.Id);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<int>(message: e.Message);
            }
        }

        public Result<bool> AddComment(int commentId)
        {
            try
            {
                DeleteById(commentId);

                if (!SaveChanges())
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
    }
}
