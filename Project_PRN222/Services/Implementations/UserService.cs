using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetById(int id)
        {
            return await _userRepository.GetById(id);
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _userRepository.GetByEmail(email);
        }

        public async Task<User> GetByUsername(string username)
        {
            return await _userRepository.GetByUsername(username);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userRepository.GetAll();
        }

        public async Task<User> Create(User user)
        {
            await _userRepository.Add(user);
            return user;
        }

        public async Task Update(User user)
        {
            await _userRepository.Update(user);
        }

        public async Task Delete(int id)
        {
            await _userRepository.Delete(id);
        }
    }
}
