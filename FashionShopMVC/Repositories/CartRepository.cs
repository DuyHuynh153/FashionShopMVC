using FashionShopMVC.Data;
using FashionShopMVC.Models.DTO.CartDTO;
using FashionShopMVC.Repositories.@interface;

namespace FashionShopMVC.Repositories


    
{
    public class CartRepository : ICartRepository
    {
        private FashionShopDBContext _fashionShopDBContext;

        public CartRepository(FashionShopDBContext fashionShopDBContext) 
        {
            _fashionShopDBContext = fashionShopDBContext;
        }
        public async Task<int> AddPurchaseasynx(PurchaseDTO model)
        {
            await _fashionShopDBContext.AddAsync(model);

            return model.Id;
        }

        public async Task SaveChange ()
        {
            await _fashionShopDBContext.SaveChangesAsync();
        }

        Task ICartRepository.DeletePurchaseasynx(int purchaseId)
        {
            throw new NotImplementedException();
        }

        Task<List<PurchaseDTO>> ICartRepository.GetallPurchase()
        {
            throw new NotImplementedException();
        }

        Task<PurchaseDTO> ICartRepository.GetPurchasebyID(int ID)
        {
            throw new NotImplementedException();
        }

        Task ICartRepository.UpdatePurchaseasynx(PurchaseDTO model, int id)
        {
            throw new NotImplementedException();
        }
    }
}
