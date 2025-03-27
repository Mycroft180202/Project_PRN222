using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Attributes;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: Cart/Cart - Hiển thị trang giỏ hàng
        [HttpGet("Cart/Cart")]
        [RoleAuthorize(1, 2, 3)]
        public async Task<IActionResult> Cart()
        {
            try
            {
                var cartItems = await _cartService.GetCartItems();
                // Chuyển đổi IEnumerable<CartItemDto> thành List<CartItemDto>
                return View("Cart", cartItems.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // API Endpoints
        [HttpPost("api/cart/add")]
        [RoleAuthorize(1, 2, 3)]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                await _cartService.AddToCart(request.ProductId, request.Quantity);
                return Ok(new { Message = "Product added to cart successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("api/cart")]
        [RoleAuthorize(1, 2, 3)]
        public async Task<IActionResult> GetCartItems()
        {
            try
            {
                var cartItems = await _cartService.GetCartItems();
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("api/cart/{cartItemId}")]
        [RoleAuthorize(1, 2, 3)]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] int quantity)
        {
            try
            {
                await _cartService.UpdateCartItem(cartItemId, quantity);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("api/cart/{cartItemId}")]
        [RoleAuthorize(1, 2, 3)]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            try
            {
                await _cartService.RemoveFromCart(cartItemId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}