using Project_PRN222.Models;

namespace Project_PRN222.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetById(int id);
        Task<User> GetByEmail(string email);
        Task<User> GetByUsername(string username);
        Task<IEnumerable<User>> GetAll();
        Task Add(User user);
        Task Update(User user);
        Task Delete(int id);
    }
}
