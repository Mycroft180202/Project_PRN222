using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public Category GetCategoryById(int id)
        {
            return _categoryRepository.GetById(id);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryRepository.GetAll();
        }

        public void CreateCategory(Category category)
        {
            _categoryRepository.Add(category);
        }

        public void UpdateCategory(Category category)
        {
            //Xu li logic thang nao duoc phep update
            //Check roleID 
            _categoryRepository.Update(category);
        }

        public void DeleteCategory(int id)
        {
            _categoryRepository.Delete(id);
        }
    }
}
