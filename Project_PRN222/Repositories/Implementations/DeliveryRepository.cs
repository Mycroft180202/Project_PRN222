using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;

namespace Project_PRN222.Repositories.Implementations
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly ProjectPrn222Context _context;

        public DeliveryRepository(ProjectPrn222Context context)
        {
            _context = context;
        }

        public async Task AddDelivery(Delivery delivery)
        {
            await _context.Deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();
        }
    }
}
