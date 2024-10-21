using FashionShopMVC.Data;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO;
using FashionShopMVC.Models.DTO.CategoriesDTO;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;

namespace FashionShopMVC.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FashionShopDBContext _context;
        public CategoryRepository(FashionShopDBContext context)
        {
            _context = context;
        }
        public List<CategoryDTO> GetAllCategory()
        {
            var allCategory = _context.Categories.Select(Categorys => new CategoryDTO()
            {
                ID = Categorys.ID,
                Name = Categorys.Name
            }).ToList();
            return allCategory;
        }
        public CategoryByIdDTO GetCategoryById(int id)
        {
            var CategoryWithDomain = _context.Categories.Where(n => n.ID == id);
            var CategoryWithIdDTO = CategoryWithDomain.Select(Categorys => new CategoryByIdDTO()
            {
                Name = Categorys.Name
            }).FirstOrDefault();
            return CategoryWithIdDTO;
        }
        public Category AddCategory(AddCategoryRequestDTO addCategoryRequestDTO)
        {
            var CategoryDomainModel = new Category
            {
                Name = addCategoryRequestDTO.Name
            };
            _context.Categories.Add(CategoryDomainModel);
            _context.SaveChanges();
            return CategoryDomainModel;
        }
        public AddCategoryRequestDTO? UpdateCategoryById(int id, AddCategoryRequestDTO CategoryDTO)
        {
            var CategoryDomain = _context.Categories.FirstOrDefault(n => n.ID == id);
            if (CategoryDomain != null)
            {
                CategoryDomain.Name = CategoryDTO.Name;
                _context.SaveChanges();
            }
            return CategoryDTO;
        }
        public Category DeleteCategoryById(int id)
        {
            var CategoryDomain = _context.Categories.FirstOrDefault(n => n.ID == id);
            if (CategoryDomain != null)
            {
                _context.Categories.Remove(CategoryDomain);
                _context.SaveChanges();
            }
            return CategoryDomain;
        }

        public async  Task<IEnumerable<GetCategoryDTO>> GetAllCategoryAsync()
        {
            var listCategory = await _context.Categories.Select(Categorys => new GetCategoryDTO()
            {
                ID = Categorys.ID,
                Name = Categorys.Name
            }).ToListAsync();
            if (!listCategory.Any())
            {
                Console.WriteLine("No category found");
            }
            return listCategory;
        }

        public async Task< GetCategoryDTO>  GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                Console.WriteLine("Category not found with ID: {id}", id);
                return null;
            }
            return new GetCategoryDTO
            {
                Name = category.Name
            };

        }

        public async Task< CreateCategoryDTO> AddCategoryAsync(CreateCategoryDTO createCategory)
        {
            var exsitingCategory = await _context.Categories.FirstOrDefaultAsync(n => n.Name == createCategory.Name);

            if (exsitingCategory != null)
            {
                Console.WriteLine("Category already exists with name: {name}", createCategory.Name);
                return null;
            }
           
            var newCategory = new Category
            {
                Name = createCategory.Name
            };
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return createCategory;
            
        }

        public async Task <UpdateCategoryDTO> UpdateByIdAsync(int id, UpdateCategoryDTO CategoryDTO)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                Console.WriteLine("Category not found with ID: {id}", id);
                return null;
            }
            // there is an existing category with the same id
            existingCategory.Name = CategoryDTO.Name;
            await _context.SaveChangesAsync();
            return CategoryDTO;

        }

        public async Task< GetCategoryDTO> DeleteByIdAsync(int id)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                Console.WriteLine("Category not found with ID: {id}", id);
                return null;
            }
            _context.Categories.Remove(existingCategory);
            await _context.SaveChangesAsync();
            return new GetCategoryDTO
            {
                Name = existingCategory.Name
            };
        }
    }
}
