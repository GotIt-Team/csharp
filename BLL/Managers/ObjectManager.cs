using GotIt.MSSQL;
using GotIt.MSSQL.Models;
using GotIt.MSSQL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.BLL.Managers
{
    public class ObjectManager : Repository<ObjectEntity>
    {
        public ObjectManager(GotItDbContext dbContext) : base(dbContext) {}
    }
}
