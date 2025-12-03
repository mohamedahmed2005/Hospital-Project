using Hospital.DAL.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Repositories.Interfaces
{
    public interface IGenericReposirory<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll(bool withTracking = false);
        TEntity? GetById(int id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
