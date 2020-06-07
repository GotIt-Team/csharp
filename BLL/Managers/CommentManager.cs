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
    }
}
