using FashionShopMVC.Areas.Admin.Repo.UnitOfWork;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.CategoriesDTO;

namespace FashionShopMVC.Areas.Admin.Repo
{
    public class CategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GetCategoryDTO>> GetAllCategoryAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(c => new GetCategoryDTO { ID = c.ID, Name = c.Name });
        }

        public async Task<GetCategoryDTO> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                Console.WriteLine("Category not found with ID: {id}", id);
                return null;
            }
            return new GetCategoryDTO { ID = category.ID, Name = category.Name };
        }

        public async Task<CreateCategoryDTO> AddCategoryAsync(CreateCategoryDTO createCategory)
        {
            var existingCategory = (await _unitOfWork.Categories.GetAllAsync())
                .FirstOrDefault(c => c.Name == createCategory.Name);

            if (existingCategory != null)
            {
                Console.WriteLine("Category already exists with name: {name}", createCategory.Name);
                return null;
            }

            var newCategory = new Category { Name = createCategory.Name };
            await _unitOfWork.Categories.AddAsync(newCategory);
            await _unitOfWork.CompleteAsync();
            return createCategory;
        }

        public async Task<UpdateCategoryDTO> UpdateByIdAsync(int id, UpdateCategoryDTO categoryDTO)
        {
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existingCategory == null)
            {
                Console.WriteLine("Category not found with ID: {id}", id);
                return null;
            }

            existingCategory.Name = categoryDTO.Name;
            _unitOfWork.Categories.Update(existingCategory);
            await _unitOfWork.CompleteAsync();
            return categoryDTO;
        }

        public async Task<GetCategoryDTO> DeleteByIdAsync(int id)
        {
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existingCategory == null)
            {
                Console.WriteLine("Category not found with ID: {id}", id);
                return null;
            }

            _unitOfWork.Categories.Delete(existingCategory);
            await _unitOfWork.CompleteAsync();
            return new GetCategoryDTO { ID = existingCategory.ID, Name = existingCategory.Name };
        }
    }

}
