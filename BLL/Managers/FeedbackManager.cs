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
    public class FeedbackManager : Repository<FeedbackEntity>
    {
        public FeedbackManager(GotItDbContext dbContext) : base(dbContext) {}
        public Result<bool> AddFeedback(FeedbackViewModel feedbackViewModel,int userID)
        {
            try
            {
                var obj = new FeedbackEntity
                {
                    Rate = feedbackViewModel.Rate,
                    Opinion = feedbackViewModel.Opinion,
                    UserId = userID
                };
                Add(obj);
                SaveChanges();
                return ResultHelper.Succeeded<bool>(data: true);
            }
            catch (Exception e)
            {
                return ResultHelper.Failed<bool>(data: false , message: e.Message);
            }
        }
    }
}
