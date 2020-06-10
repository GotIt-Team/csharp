using GotIt.BLL.ViewModels;
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
    public class SystemManager : Repository<FeedbackEntity>
    {
        public SystemManager(GotItDbContext dbContext) : base(dbContext) {}
        
        public Result<bool> AddFeedback(int userId, FeedbackViewModel feedbackViewModel)
        {
            try
            {
                var obj = new FeedbackEntity
                {
                    Rate = feedbackViewModel.Rate,
                    Opinion = feedbackViewModel.Opinion,
                    UserId = userId
                };

                Add(obj);
                
                SaveChanges();
                
                return ResultHelper.Succeeded(data: true);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed(data: false , message: e.Message);
            }
        }
    }
}
