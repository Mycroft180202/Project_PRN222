using Project_PRN222.Models;

namespace Project_PRN222.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Category GetById(int id);
        IEnumerable<Category> GetAll();
        void Add(Category category);
        void Update(Category category);
        void Delete(int id);
    }
}
