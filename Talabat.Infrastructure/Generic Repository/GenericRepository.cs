using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Infrastructure.Data;

namespace Talabat.Infrastructure.Generic_Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            //if (typeof(T) == typeof(Product))
            //    return (IEnumerable<T>)await _dbContext.Set<Product>().Include(p => p.Brand).Include(p => p.Category).ToListAsync();//Eager loading - inner join between Product&ProductBrand | Product&ProductCategory 
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            //if (typeof(T) == typeof(Product))
            //    return await _dbContext.Set<Product>().Where(p => p.Id == id).Include(p => p.Brand).Include(p => p.Category).SingleOrDefaultAsync() as T;
            return await _dbContext.Set<T>().FindAsync(id);
        }



        //Using Specification Design Pattern
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdWithSpecAsync(ISpecification<T> spec)
        {
            return await SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec).FirstOrDefaultAsync();
        }

        private IQueryable<T> ApplySpecifications(ISpecification<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }

        public async Task<int> GetCountAsync(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).CountAsync();
        }

        public void Add(T entity)
            => _dbContext.Set<T>().Add(entity);


        public void Update(T entity)
            => _dbContext.Set<T>().Update(entity);

        public void Delete(T entity)
            => _dbContext.Set<T>().Remove(entity);
    }
}
