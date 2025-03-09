using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Attributes;
using Project_PRN222.DTO;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        [RoleAuthorize(1, 2, 3)]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
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

        [HttpGet("callback")]
        public async Task<IActionResult> Callback() // Changed return type to IActionResult
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    IActionResult result = await _orderService.ProcessVnpayCallback(Request.Query); // Get IActionResult directly
                    return result; // Return the IActionResult directly
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Callback error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    return BadRequest(ex.Message);
                }
            }
            return NotFound("Không tìm thấy thông tin thanh toán.");
        }

        [HttpGet("ipn")]
        public async Task<IActionResult> IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    IActionResult result = await _orderService.ProcessVnpayCallback(Request.Query);
                    if (result is OkObjectResult)
                    {
                        return Ok(); // For IPN, just return Ok on success
                    }
                    return BadRequest("Thanh toán thất bại"); // For IPN, return BadRequest on failure
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
    }
}