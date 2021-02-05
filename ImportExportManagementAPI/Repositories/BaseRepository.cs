using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


/**
* @author Loi Nguyen
*
* @date - 1/30/2021 11:13:45 PM 
*/

namespace ImportExportManagement_API.Repositories
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected IEDbContext _dbContext;
        protected DbSet<TEntity> _dbSet;

        public BaseRepository()
        {
            _dbContext = new IEDbContext();
            _dbSet = _dbContext.Set<TEntity>();
        }
        public void Delete(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public List<TEntity> GetAll()
        {
            List<TEntity> entities = _dbSet.ToList();
            return entities;
        }

        public TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
