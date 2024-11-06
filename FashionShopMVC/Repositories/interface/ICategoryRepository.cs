using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.CartDTO;
using FashionShopMVC.Models.DTO.CategoriesDTO;

namespace FashionShopMVC.Repositories.@interface
{
    public interface ICategoryRepository
    {
        List<CategoryDTO> GetAllCategory();
        CategoryByIdDTO GetCategoryById(int id);
        Category AddCategory(AddCategoryRequestDTO addCategoryRequestDTO);
        AddCategoryRequestDTO? UpdateCategoryById(int id, AddCategoryRequestDTO CategoryDTO);
        Category DeleteCategoryById(int id);


        Task<IEnumerable<GetCategoryDTO>> GetAllCategoryAsync();
        Task<GetCategoryDTO> GetByIdAsync(int id);
        Task<CreateCategoryDTO> AddCategoryAsync(CreateCategoryDTO createCategory);
        Task<UpdateCategoryDTO> UpdateByIdAsync(int id, UpdateCategoryDTO CategoryDTO);
        Task<GetCategoryDTO> DeleteByIdAsync(int id);
    }
}
