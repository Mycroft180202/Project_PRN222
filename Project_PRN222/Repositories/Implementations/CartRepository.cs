using Microsoft.EntityFrameworkCore;
using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;

namespace Project_PRN222.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly ProjectPrn222Context _context;

        public CartRepository(ProjectPrn222Context context)
        {
            _context = context;
        }

        public async Task<CartItem> GetById(int id)
        {
            return await _context.CartItems.FindAsync(id);
        }

        public async Task<IEnumerable<CartItem>> GetByUserId(int userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product) // Tải thông tin sản phẩm nếu cần
                .ToListAsync();
        }

        public async Task<CartItem> GetByUserIdAndProductId(int userId, int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }

        public async Task Add(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var cartItem = await GetById(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}