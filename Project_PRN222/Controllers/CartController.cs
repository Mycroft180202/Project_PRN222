using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Attributes;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add")]
        [RoleAuthorize(1, 2, 3)]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            try
            {
                await _cartService.AddToCart(productId, quantity);
                return Ok(new { Message = "Product added to cart successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet]
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

        [HttpPut("{cartItemId}")]
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

        [HttpDelete("{cartItemId}")]
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
}