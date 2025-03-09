using Project_PRN222.DTO;
using Project_PRN222.Models;

namespace Project_PRN222.Services.Interfaces
{
    public interface ICartService
    {
        Task AddToCart(int productId, int quantity);
        Task<IEnumerable<CartItemDto>> GetCartItems();
        Task UpdateCartItem(int cartItemId, int quantity);
        Task RemoveFromCart(int cartItemId);
    }
}
