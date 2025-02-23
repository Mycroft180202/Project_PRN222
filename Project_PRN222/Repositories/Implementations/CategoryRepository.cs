using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;

namespace Project_PRN222.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProjectPrn222Context _context;
        public CategoryRepository(ProjectPrn222Context context)
        {
            _context = context;
        }
        void ICategoryRepository.Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        void ICategoryRepository.Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if(category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }

        IEnumerable<Category> ICategoryRepository.GetAll()
        {
            return _context.Categories.ToList();
        }

        Category ICategoryRepository.GetById(int id)
        {
            return _context.Categories.Find(id);
        }

        void ICategoryRepository.Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }
    }
}
