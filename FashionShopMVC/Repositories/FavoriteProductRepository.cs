

using FashionShopMVC.Data;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.FavoriteProductDTO;
using FashionShopMVC.Models.DTO.ProductDTO;
using Microsoft.EntityFrameworkCore;
using FashionShopMVC.Models.DTO.FavoriteProductDTO;

namespace FashionShopMVC.Repositories
{
    public interface IFavoriteProductRepository
    {
        public Task<List<GetFavoriteProductByUserIdDTO>> GetByUserID(string userID);
        public Task<CreateFavoriteProductDTO> Create(CreateFavoriteProductDTO createFavoriteProductDTO);
        public Task<bool> Delete(int productID, string userID);
    }
}

namespace FashionShopMVC.Repositories
{
    public class FavoriteProductRepository : IFavoriteProductRepository
    {
        private readonly FashionShopDBContext _fashionShopDBContext;

        public FavoriteProductRepository(FashionShopDBContext fashionShopDBContext)
        {
            _fashionShopDBContext = fashionShopDBContext;
        }
        public async Task<List<GetFavoriteProductByUserIdDTO>> GetByUserID(string userID)
        {
            var listFavoriteByUser = await _fashionShopDBContext.FavoriteProducts.Where(f => f.UserID == userID).Select(favoriteProduct => new GetFavoriteProductByUserIdDTO()
            {
                UserID = favoriteProduct.UserID,
                ProductID = favoriteProduct.ProductID,
                ProductDTO = _fashionShopDBContext.Products.SingleOrDefault(p => p.ID == favoriteProduct.ProductID),
                AddedDate = favoriteProduct.AddedDate
            }).OrderByDescending(f => f.AddedDate).ToListAsync();

            return listFavoriteByUser;
        }

        public async Task<CreateFavoriteProductDTO> Create(CreateFavoriteProductDTO createFavoriteProductDTO)
        {
            var favoriteProduct = new FavoriteProduct()
            {
                ProductID = createFavoriteProductDTO.ProductID,
                UserID = createFavoriteProductDTO.UserID,
                AddedDate = DateTime.Now,
            };
            await _fashionShopDBContext.FavoriteProducts.AddAsync(favoriteProduct);
            await _fashionShopDBContext.SaveChangesAsync();

            return createFavoriteProductDTO;
        }

        public async Task<bool> Delete(int productID, string userID)
        {
            var favoriteProduct = await _fashionShopDBContext.FavoriteProducts.SingleOrDefaultAsync(f => f.ProductID == productID && f.UserID == userID);

            if (favoriteProduct != null)
            {
                _fashionShopDBContext.Remove(favoriteProduct);
                await _fashionShopDBContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

    }
}
