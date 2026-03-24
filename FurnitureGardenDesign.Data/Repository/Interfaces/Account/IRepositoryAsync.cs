using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Account
{
    public interface IRepositoryAsync<TEntity, TKey>
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        IQueryable<TEntity> GetAllIncludingDeleted();
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetCategoriesAsync();
        Task<int> CountAsync();
        Task AddAsync(TEntity item);

        Task AddRangeAsync(IEnumerable<TEntity> items);

        Task<bool> HardDeleteAsync(TEntity entity);

        Task<bool> DeleteAsync(TEntity entity);

        Task<bool> UpdateAsync(TEntity item);


        //Task<TEntity> FindByConditionAsync(Expression<Func<TEntity, bool>> predicate);


        Task SaveChangesAsync();


        IQueryable<TEntity> GetAllAttachedAsync();
        Task<bool> ToggleStatusAsync(TEntity entity);


























    }
}
