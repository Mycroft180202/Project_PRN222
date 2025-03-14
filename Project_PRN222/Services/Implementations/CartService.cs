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

        private int GetCurrentUserId()
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                throw new Exception("User not logged in.");
            }
            return userId;
        }

        public async Task AddToCart(int productId, int quantity)
        {
            var userId = GetCurrentUserId();
            var existingItem = await _cartRepository.GetByUserIdAndProductId(userId, productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _cartRepository.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
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
            var cartItems = await _cartRepository.GetByUserId(userId);
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
            var cartItem = await _cartRepository.GetById(cartItemId);
            if (cartItem == null || cartItem.UserId != userId)
            {
                throw new Exception("Cart item not found or not owned by user.");
            }
            await _cartRepository.Delete(cartItemId);
        }
    }
}