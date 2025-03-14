using Microsoft.EntityFrameworkCore;
using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;

namespace Project_PRN222.Repositories.Implementations
{
    public class VendorRepository : IVendorRepository
    {
        private readonly ProjectPrn222Context _context;

        public VendorRepository(ProjectPrn222Context context)
        {
            _context = context;
        }

        public async Task<Vendor> GetById(int id)
        {
            return await _context.Vendors.FindAsync(id);
        }

        public async Task<Vendor> GetByUserId(int userId)
        {
            return await _context.Vendors.FirstOrDefaultAsync(v => v.UserId == userId);
        }

        public async Task<IEnumerable<Vendor>> GetAll()
        {
            return await _context.Vendors.ToListAsync();
        }

        public async Task Add(Vendor vendor)
        {
            await _context.Vendors.AddAsync(vendor);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Vendor vendor)
        {
            _context.Vendors.Update(vendor);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var vendor = await GetById(id);
            if (vendor != null)
            {
                _context.Vendors.Remove(vendor);
                await _context.SaveChangesAsync();
            }
        }
    }
}
