using GotIt.Common.Helper;
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
        PageResult<List<TEntity>> GetAllPaginated(Expression<Func<TEntity, bool>> condition, int pageNo, int pageSize, params string[] includes);
        // add
        TEntity Add(TEntity entity);
        // add list
        List<TEntity> Add(List<TEntity> entities);
        // update
        void Update(TEntity entity);
        // update list
        void Update(List<TEntity> entities);
        // update specific properties
        void Update(TEntity entity, params Expression<Func<TEntity, object>>[] properties);
        // delete
        void Delete(TEntity entity);
        // delete list
        void Delete(List<TEntity> entities);
        // delete by id
        void DeleteById(params int[] ids);
        // save changed
        bool SaveChanges();
    }
}
