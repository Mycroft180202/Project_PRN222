using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Attributes;
using Project_PRN222.DTO;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Action to return Checkout View
        [HttpGet("Order/Checkout")]
        //[RoleAuthorize(1, 2, 3)]
        public IActionResult Checkout()
        {
            return View("Checkout"); // Assuming you have a Checkout.cshtml in Views/Order
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
}