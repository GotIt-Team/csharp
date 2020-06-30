using GotIt.MSSQL;
using GotIt.MSSQL.Models;
using GotIt.MSSQL.Repository;

namespace GotIt.BLL.Managers
{
    public class ProbablyMatchManager : Repository<ProbablyMatchEntity>
        
    {
        public ProbablyMatchManager(GotItDbContext dbContext ):base(dbContext)
        {
            
        }
    }
}