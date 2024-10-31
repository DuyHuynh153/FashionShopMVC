using FashionShopMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace FashionShopMVC.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly FashionShopDBContext _context;

        protected readonly DbSet<T> _dbSet;

        public Repository(FashionShopDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }   
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
