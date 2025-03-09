using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;

namespace Project_PRN222.Repositories.Implementations
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly ProjectPrn222Context _context;

        public OrderItemRepository(ProjectPrn222Context context)
        {
            _context = context;
        }

        public async Task AddOrderItem(OrderItem orderItem)
        {
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
        }
    }
}
