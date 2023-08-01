using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using userMS.Domain.Entities.Common;

namespace userMS.Application.Repositories
{
    // TPrimaryKey to allow the repository to handle entities with different primary key types
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : IEntity<TPrimaryKey>
    {
        IEnumerable<TEntity> GetAll();

        Task<IEnumerable<TEntity>> GetAllAsync();

        TEntity GetById(TPrimaryKey id);

        Task<TEntity> GetByIdAsync(TPrimaryKey id);

        TEntity Add(TEntity entity);

        Task<TEntity> AddAsync(TEntity entity);

        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);

        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities);

        Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities);

        bool Delete(TEntity entity);

        Task<bool> DeleteAsync(TEntity entity);

        bool DeleteById(TPrimaryKey id);

        Task<bool> DeleteByIdAsync(TPrimaryKey id);

        bool DeleteRange(IEnumerable<TEntity> entities);

        Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities);

        IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate);

        bool Any(Expression<Func<TEntity, bool>> predicate);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        int Count(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
