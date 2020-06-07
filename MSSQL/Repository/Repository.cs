using GotIt.Common.Extentions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GotIt.MSSQL.Repository
{
    public class Repository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class, new()
    {
        private readonly DbContext _dbContext;
        private bool _disposed;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> Get()
        {
            try
            {
                return _dbSet;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public TEntity Get(Expression<Func<TEntity, bool>> condition, params string[] includes)
        {
            try
            {
                var result = _dbSet.Where(condition);
                
                if (includes.Length > 0)
                {
                    result = result.IncludeMultiple(includes);
                }

                return result.AsNoTracking().FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<TEntity> GetAll(Expression<Func<TEntity, bool>> condition, params string[] includes)
        {
            try
            {
                var result = _dbSet.Where(condition);

                if (includes.Length > 0)
                {
                    result = result.IncludeMultiple(includes);
                }

                return result.AsNoTracking().ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<TEntity> GetAllPaginated(Expression<Func<TEntity, bool>> condition, int pageNo, int pageSize, params string[] includes)
        {
            try
            {
                var skip = (pageNo - 1) * pageSize;
                var result = _dbSet.Where(condition);
                var count = result.Count();

                if (includes.Length > 0)
                {
                    result = result.IncludeMultiple(includes);
                }

                return result.Skip(skip).Take(pageSize).AsNoTracking().ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public TEntity Add(TEntity entity)
        {
            try
            {
                return _dbSet.Add(entity).Entity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<TEntity> Add(List<TEntity> entities)
        {
            try
            {
                var result = new List<TEntity>();
                foreach(var entity in entities)
                {
                    result.Add(Add(entity));
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Update(TEntity entity)
        {
            try
            {
                _dbSet.Update(entity);
                return SaveChanges() > 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Update(List<TEntity> entities)
        {
            try
            {
                _dbSet.UpdateRange(entities);
                return SaveChanges() > 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Delete(TEntity entity)
        {
            try
            {
                _dbSet.Remove(entity);
                return SaveChanges() > 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Delete(List<TEntity> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
                return SaveChanges() > 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool DeleteById(params int[] ids)
        {
            try
            {
                // check if the TEntity have id property or not
                Type type = typeof(TEntity);
                var property = type.GetProperty("id");
                if (property == null)
                {
                    throw new Exception("Entity should have property id");
                }

                var entities = new List<TEntity>();
                foreach (var id in ids)
                {
                    var entity = new TEntity();
                    property.SetValue(entity, id);
                    entities.Add(entity);
                }
                return Delete(entities);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public int SaveChanges()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
