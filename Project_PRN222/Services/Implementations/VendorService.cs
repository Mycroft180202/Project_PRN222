using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Services.Implementations
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;

        public VendorService(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        public async Task<Vendor> GetById(int id)
        {
            return await _vendorRepository.GetById(id);
        }

        public async Task<Vendor> GetByUserId(int userId)
        {
            return await _vendorRepository.GetByUserId(userId);
        }

        public async Task<IEnumerable<Vendor>> GetAll()
        {
            return await _vendorRepository.GetAll();
        }

        public async Task CreateVendor(Vendor vendor)
        {
            await _vendorRepository.Add(vendor);
        }

        public async Task UpdateVendor(Vendor vendor)
        {
            await _vendorRepository.Update(vendor);
        }

        public async Task DeleteVendor(int id)
        {
            await _vendorRepository.Delete(id);
        }
    }
}
