using FashionShopMVC.Models.Domain;

namespace FashionShopMVC.Areas.Admin.Repo.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Categories { get; }
        Task<int> CompleteAsync();
    }
}
