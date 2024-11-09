using FashionShopMVC.Models.DTO.CartDTO;

namespace FashionShopMVC.Repositories.@interface
{ 
    
    public interface ICartRepository
    {
        public Task<List<PurchaseDTO>> GetallPurchase();
        public Task<int> AddPurchaseasynx(PurchaseDTO model);
        public Task UpdatePurchaseasynx(PurchaseDTO model,int id);
        public Task DeletePurchaseasynx(int purchaseId);
        public Task<PurchaseDTO> GetPurchasebyID(int ID);

        public Task SaveChange ();
    }
}
