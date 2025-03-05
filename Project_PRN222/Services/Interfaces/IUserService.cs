using Project_PRN222.Models;

namespace Project_PRN222.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetById(int id);
        Task<User> GetByEmail(string email);
        Task<User> GetByUsername(string username);
        Task<IEnumerable<User>> GetAll();
        Task<User> Create(User user);
        Task Update(User user);
        Task Delete(int id);
    }
}
