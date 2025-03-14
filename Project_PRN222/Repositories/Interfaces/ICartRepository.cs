using Project_PRN222.Models;

namespace Project_PRN222.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<CartItem> GetById(int id);
        Task<IEnumerable<CartItem>> GetByUserId(int userId);
        Task<CartItem> GetByUserIdAndProductId(int userId, int productId); // Để kiểm tra sản phẩm đã có trong giỏ chưa
        Task Add(CartItem cartItem);
        Task Update(CartItem cartItem);
        Task Delete(int id);
    }
}
