using Project_PRN222.Models;

namespace Project_PRN222.Services.Interfaces
{
    public interface IVendorService
    {
        Task<Vendor> GetById(int id);
        Task<Vendor> GetByUserId(int userId);
        Task<IEnumerable<Vendor>> GetAll();
        Task CreateVendor(Vendor vendor);
        Task UpdateVendor(Vendor vendor);
        Task DeleteVendor(int id);
    }
}
