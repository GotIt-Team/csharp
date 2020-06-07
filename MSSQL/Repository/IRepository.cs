using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GotIt.MSSQL.Repository
{
    public interface IRepository<TEntity> where TEntity : class, new()
    {
        // get queryable
        IQueryable<TEntity> Get();
        // get
        TEntity Get(Expression<Func<TEntity, bool>> condition, params string[] includes);
        // get list
        List<TEntity> GetAll(Expression<Func<TEntity, bool>> condition, params string[] includes);
        // get paginated
        List<TEntity> GetAllPaginated(Expression<Func<TEntity, bool>> condition, int pageNo, int pageSize, params string[] includes);
        // add
        TEntity Add(TEntity entity);
        // add list
        List<TEntity> Add(List<TEntity> entities);
        // update
        bool Update(TEntity entity);
        // update list
        bool Update(List<TEntity> entities);
        // delete
        bool Delete(TEntity entity);
        // delete list
        bool Delete(List<TEntity> entities);
        // delete by id
        bool DeleteById(params int[] ids);
        // save changed
        int SaveChanges();
    }
}
