using FurnitureGardenDesign.Data.Common;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
  
        public abstract class BaseRepository<TEntity, TKey> :
      IRepository<TEntity, TKey>, IRepositoryAsync<TEntity, TKey>
      where TEntity : class

        {
            protected readonly ApplicationDbContext _context;

            protected readonly DbSet<TEntity> _dbSet;

            protected BaseRepository(ApplicationDbContext context)
            {
                _context = context;
                _dbSet = context.Set<TEntity>();
            }






            public int Count()
                => _dbSet.Count();

            public async Task<int> CountAsync()
                => await _dbSet.CountAsync();



            public void Add(TEntity item)
            {
                _dbSet.Add(item);
                _context.SaveChanges();
            }


            public async Task AddAsync(TEntity item)
            {
                await _dbSet.AddAsync(item);
                await _context.SaveChangesAsync();

            }

            public void AddRange(IEnumerable<TEntity> items)
            {
                _dbSet.AddRange(items);
                _context.SaveChanges();
            }

            public async Task AddRangeAsync(IEnumerable<TEntity> items)
            {
                await _dbSet.AddRangeAsync(items);
                await _context.SaveChangesAsync();
            }

            public bool Delete(TEntity entity)
         => SoftDelete(entity) > 0;

            public async Task<bool> DeleteAsync(TEntity entity)
                => await SoftDeleteAsync(entity) > 0;


            //public Task<TEntity> FindByConditionAsync(Expression<Func<TEntity, bool>> predicate)
            //{
            //    throw new NotImplementedException();
            //}

            public TEntity? FirstOrDefault(Func<TEntity, bool> predicate)
            => _dbSet.FirstOrDefault(predicate);

            public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
                => await _dbSet.FirstOrDefaultAsync(predicate);


            public IEnumerable<TEntity> GetAll() => _dbSet.ToList();

        public async Task<IEnumerable<TEntity>> GetCategoriesAsync()
         => await _dbSet.ToListAsync();

        public IQueryable<TEntity> GetAllIncludingDeleted()
        {
            return _dbSet.IgnoreQueryFilters().AsQueryable();
        }


        public IQueryable<TEntity> GetAllAttachedAsync()
                => _dbSet.AsQueryable();

        public IQueryable<TEntity> GetAllAttached()
            => _dbSet.AsQueryable();

        public TEntity? GetById(TKey id)
                => _dbSet.Find(id);


            public async Task<TEntity?> GetByIdAsync(TKey id)
           => await _dbSet.FindAsync(id);


            public async Task<bool> HardDeleteAsync(TEntity entity)
            {
                _dbSet.Remove(entity);
                return await _context.SaveChangesAsync() > 0;
            }

            public bool HardDelete(TEntity entity)
            {
                _dbSet.Remove(entity);
                return _context.SaveChanges() > 0;
            }

            public void SaveChanges()
           => _context.SaveChanges();


            public async Task SaveChangesAsync()
           => await _context.SaveChangesAsync();

            public TEntity? SingleOrDefault(Func<TEntity, bool> predicate)
                => _dbSet.SingleOrDefault(predicate);

            public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
                => await _dbSet.SingleOrDefaultAsync(predicate);

            public bool Update(TEntity item)
            {
                try
                {
                    _dbSet.Attach(item);
                    _dbSet.Entry(item).State = EntityState.Modified;
                    _context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public async Task<bool> UpdateAsync(TEntity item)
            {
                try
                {
                    _dbSet.Attach(item);
                    _dbSet.Entry(item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch
                {
                    return false;
                }
            }


            private int SoftDelete(TEntity entity)
            {
                var flagProperty = GetFlagProperty();
                if (flagProperty != null && flagProperty.PropertyType == typeof(bool))
                {
                    flagProperty.SetValue(entity, true);
                    _dbSet.Attach(entity);
                    _dbSet.Entry(entity).State = EntityState.Modified;
                    return _context.SaveChanges();
                }
                throw new InvalidOperationException(ExceptionMessages.SoftDeleteNotSupported);
            }

        public async Task<bool> ToggleStatusAsync(TEntity entity)
        {
            var entry = _context.Entry(entity);

            if (entry.State == EntityState.Detached)
                _dbSet.Attach(entity);

            var property = entry.Property("IsDeleted");
            property.CurrentValue = !(bool)property.CurrentValue!;
            property.IsModified = true;

            return await _context.SaveChangesAsync() > 0;
        }



        private async Task<int> SoftDeleteAsync(TEntity entity)
            {
                var flagProperty = GetFlagProperty();
                if (flagProperty != null && flagProperty.PropertyType == typeof(bool))
                {
                    flagProperty.SetValue(entity, true);
                    _dbSet.Attach(entity);
                    _dbSet.Entry(entity).State = EntityState.Modified;
                    return await _context.SaveChangesAsync();
                }
                throw new InvalidOperationException(ExceptionMessages.SoftDeleteNotSupported);
            }

            private PropertyInfo? GetFlagProperty()

               => typeof(TEntity).GetProperty("IsDeleted");

        }

    
}
