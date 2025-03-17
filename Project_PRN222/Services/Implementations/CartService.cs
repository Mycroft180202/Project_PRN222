using Project_PRN222.DTO;
using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(ICartRepository cartRepository, IHttpContextAccessor httpContextAccessor)
        {
            _cartRepository = cartRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? GetCurrentUserId()
        {
            var userIdString = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId) || userId == 0)
            {
                return null; // Trả về null nếu không lấy được UserId
            }
            return userId;
        }

        public async Task AddToCart(int productId, int quantity)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                throw new Exception("User not logged in."); // Giữ nguyên ngoại lệ cho API
            }

            var existingItem = await _cartRepository.GetByUserIdAndProductId(userId.Value, productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _cartRepository.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId.Value,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedDate = DateTime.Now
                };
                await _cartRepository.Add(cartItem);
            }
        }

        public async Task<IEnumerable<CartItemDto>> GetCartItems()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return new List<CartItemDto>(); // Trả về danh sách rỗng nếu không lấy được UserId
            }

            var cartItems = await _cartRepository.GetByUserId(userId.Value);
            return cartItems.Select(c => new CartItemDto
            {
                CartItemId = c.CartItemId,
                UserId = c.UserId,
                ProductId = c.ProductId,
                ProductName = c.Product?.ProductName ?? "Unknown",
                Price = c.Product?.Price ?? 0,
                Quantity = c.Quantity,
                AddedDate = c.AddedDate
            });
        }

        public async Task UpdateCartItem(int cartItemId, int quantity)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                throw new Exception("User not logged in."); // Giữ nguyên ngoại lệ cho API
            }

            var cartItem = await _cartRepository.GetById(cartItemId);
            if (cartItem == null || cartItem.UserId != userId)
            {
                throw new Exception("Cart item not found or not owned by user.");
            }
            cartItem.Quantity = quantity;
            await _cartRepository.Update(cartItem);
        }

        public async Task RemoveFromCart(int cartItemId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                throw new Exception("User not logged in."); // Giữ nguyên ngoại lệ cho API
            }

            var cartItem = await _cartRepository.GetById(cartItemId);
            if (cartItem == null || cartItem.UserId != userId)
            {
                throw new Exception("Cart item not found or not owned by user.");
            }
            await _cartRepository.Delete(cartItemId);
        }
    }
}