using GotIt.Common.Exceptions;
using GotIt.Common.Extentions;
using GotIt.Common.Helper;
using Microsoft.Data.SqlClient;
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

        public PageResult<List<TEntity>> GetAllPaginated(Expression<Func<TEntity, bool>> condition, int pageNo, int pageSize, params string[] includes)
        {
            try
            {
                pageNo = pageNo <= 0 ? 1 : pageNo;
                pageSize = pageSize <= 0 ? 10 : pageSize;
                var skip = (pageNo - 1) * pageSize;
                var result = _dbSet.Where(condition);
                var count = result.Count();

                if (includes.Length > 0)
                {
                    result = result.IncludeMultiple(includes);
                }

                return new PageResult<List<TEntity>>
                {
                    Data = result.Skip(skip).Take(pageSize).AsNoTracking().ToList(), 
                    Count = count
                };
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

        public void Update(TEntity entity)
        {
            try
            {
                _dbSet.Update(entity);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update(List<TEntity> entities)
        {
            try
            {
                _dbSet.UpdateRange(entities);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        {
            try
            {
                var entry = _dbSet.Attach(entity);

                foreach (var property in properties)
                {
                    entry.Property(property).IsModified = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Delete(TEntity entity)
        {
            try
            {
                _dbSet.Remove(entity);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Delete(List<TEntity> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteById(params int[] ids)
        {
            try
            {
                // check if the TEntity have id property or not
                Type type = typeof(TEntity);
                var property = type.GetProperty("Id");
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

                Delete(entities);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool SaveChanges()
        {
            try
            {
                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                if (e.InnerException is SqlException innerException && (innerException.Number == 2627 || innerException.Number == 2601))
                {
                    throw new DuplicateDataException(innerException.Message);
                }

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
