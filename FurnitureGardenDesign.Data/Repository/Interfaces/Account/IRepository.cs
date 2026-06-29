namespace FurnitureGardenDesign.Data.Repository.Interfaces.Account
{
    public interface IRepository<TEntity, TKey>
    {
        TEntity? GetById(TKey id);

        int Count();

        IQueryable<TEntity> GetAllIncludingDeleted();

        TEntity? SingleOrDefault(Func<TEntity, bool> predicate);

        TEntity? FirstOrDefault(Func<TEntity, bool> predicate);

        IEnumerable<TEntity> GetAll();

        IQueryable<TEntity> GetAllAttached();

        void Add(TEntity item);

        void AddRange(IEnumerable<TEntity> items);

        bool Delete(TEntity entity);

        bool HardDelete(TEntity entity);

        bool Update(TEntity item);

        void SaveChanges();
    }
}