using Microsoft.EntityFrameworkCore;
using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;

namespace Project_PRN222.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ProjectPrn222Context _context;

        public OrderRepository(ProjectPrn222Context context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> GetById(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
