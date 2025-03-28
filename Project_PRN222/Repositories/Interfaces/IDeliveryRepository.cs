using Project_PRN222.Models;

namespace Project_PRN222.Repositories.Interfaces
{
    public interface IDeliveryRepository
    {
        Task AddDelivery(Delivery delivery);
        Task<Delivery> GetByOrderId(int orderId);
        Task UpdateDelivery(Delivery delivery);
    }
}
