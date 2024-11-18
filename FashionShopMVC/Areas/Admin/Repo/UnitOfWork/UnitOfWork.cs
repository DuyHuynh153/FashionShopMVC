using FashionShopMVC.Data;
using FashionShopMVC.Models.Domain;

namespace FashionShopMVC.Areas.Admin.Repo.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FashionShopDBContext _context;
        private IGenericRepository<Category> _categories;

        public UnitOfWork(FashionShopDBContext context)
        {
            _context = context;
        }

        public IGenericRepository<Category> Categories
        {
            get
            {
                return _categories ??= new GenericRepository<Category>(_context);
            }
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
