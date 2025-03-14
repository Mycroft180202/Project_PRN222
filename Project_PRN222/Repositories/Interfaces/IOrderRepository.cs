using Project_PRN222.Models;

namespace Project_PRN222.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrder(Order order);
        Task<Order> GetById(int orderId);
        Task UpdateOrder(Order order);
    }
}
