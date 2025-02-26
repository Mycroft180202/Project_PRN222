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
        public void Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if(category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Category> GetAll()
        {
            return _context.Categories.ToList();
        }

        public Category GetById(int id)
        {
            return _context.Categories.Find(id);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }
    }
}
