using Hospital.DAL.Contexts;
using Hospital.DAL.Models.Shared;
using Hospital.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity>(ApplicationDbContext dbContext) : IGenericReposirory<TEntity> where TEntity : class
    {
        public ApplicationDbContext _dbContext = dbContext;
        public IEnumerable<TEntity> GetAll(bool withTracking = false)
        {
            if (withTracking)
                return _dbContext.Set<TEntity>().ToList();
            else
                return _dbContext.Set<TEntity>().AsNoTracking().ToList();
        }
        public TEntity? GetById(int id) => _dbContext.Set<TEntity>().Find(id);
        public void Add(TEntity entity) => _dbContext.Set<TEntity>().Add(entity);
        public void Delete(TEntity entity)=>_dbContext.Set<TEntity>().Remove(entity);
        public void Update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);
    }
}
