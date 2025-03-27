using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Attributes;
using Project_PRN222.DTO;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        // GET: Order/Checkout - Hiển thị trang checkout
        [HttpGet]
        [Route("Order/Checkout")]
        [RoleAuthorize(1, 2, 3)]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                // Lấy danh sách sản phẩm trong giỏ hàng
                var cartItems = await _cartService.GetCartItems();
                
                // Kiểm tra nếu giỏ hàng trống
                if (!cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty. Please add products to your cart before checkout.";
                    return RedirectToAction("Cart", "Cart");
                }
                
                return View(cartItems.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: api/Order/checkout - Xử lý thanh toán
        [HttpPost]
        [Route("api/Order/checkout")]
        [RoleAuthorize(1, 2, 3)]
        public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutRequest request)
        {
            try
            {
                var result = await _orderService.Checkout(
                    request.ShippingAddress,
                    request.BillingAddress,
                    request.ShipmentMethodId,
                    request.PaymentMethod);
                
                if (request.PaymentMethod == "VNPAY")
                {
                    return Ok(new { PaymentUrl = result });
                }
                
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // GET: api/Order/callback - Xử lý callback từ VNPAY
        [HttpGet]
        [Route("api/Order/callback")]
        public async Task<IActionResult> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    IActionResult result = await _orderService.ProcessVnpayCallback(Request.Query);
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Callback error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    return BadRequest(ex.Message);
                }
            }
            return NotFound("Không tìm thấy thông tin thanh toán.");
        }

        // GET: api/Order/ipn - Xử lý IPN từ VNPAY
        [HttpGet]
        [Route("api/Order/ipn")]
        public async Task<IActionResult> IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    IActionResult result = await _orderService.ProcessVnpayCallback(Request.Query);
                    if (result is OkObjectResult)
                    {
                        return Ok();
                    }
                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
    }

    public class CheckoutRequest
    {
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public int ShipmentMethodId { get; set; }
        public string PaymentMethod { get; set; }
    }
}