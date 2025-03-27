using Project_PRN222.Models;

namespace Project_PRN222.Repositories.Interfaces
{
    public interface IVendorRepository
    {
        Task<Vendor> GetById(int id);
        Task<Vendor> GetByUserId(int userId);
        Task<IEnumerable<Vendor>> GetAll();
        Task Add(Vendor vendor);
        Task Update(Vendor vendor);
        Task Delete(int id);
    }
}
