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
        [HttpPost("api/Order/checkout")]
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

        [HttpGet]
        [Route("api/order/callback")]
        public async Task<IActionResult> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    // Process the callback through the service
                    IActionResult serviceResult = await _orderService.ProcessVnpayCallback(Request.Query);

                    // Check if the result is an ObjectResult (which contains our JSON)
                    if (serviceResult is ObjectResult objResult)
                    {
                        // Extract the response value
                        dynamic responseValue = objResult.Value;

                        // Check if payment was successful
                        bool isSuccess = false;
                        try
                        {
                            isSuccess = responseValue.isSuccess;
                        }
                        catch
                        {
                            // If we can't access isSuccess, default to false
                        }

                        if (isSuccess)
                        {
                            // Extract order ID from the response
                            string orderId = null;
                            try
                            {
                                if (responseValue.paymentId is string[] paymentIdArray && paymentIdArray.Length > 0)
                                {
                                    orderId = paymentIdArray[0];
                                }
                                else if (responseValue.paymentId is string paymentIdString)
                                {
                                    orderId = paymentIdString;
                                }
                            }
                            catch
                            {
                                // If we can't extract the payment ID, try from query string
                                if (Request.Query.TryGetValue("vnp_TxnRef", out var txnRef))
                                {
                                    orderId = txnRef;
                                }
                            }

                            if (!string.IsNullOrEmpty(orderId) && int.TryParse(orderId, out int orderIdInt))
                            {
                                // Redirect to success page with order ID
                                return RedirectToAction("OrderSuccess", new { orderId = orderIdInt });
                            }

                            // Fallback if order ID extraction fails
                            return RedirectToAction("OrderSuccess");
                        }
                        else
                        {
                            // Extract error code if available
                            string errorCodeValue = "Unknown";
                            try
                            {
                                if (responseValue.paymentResponse != null && responseValue.paymentResponse.code != null)
                                {
                                    errorCodeValue = responseValue.paymentResponse.code.ToString();
                                }
                            }
                            catch
                            {
                                // If we can't extract the error code, try from query string
                                if (Request.Query.TryGetValue("vnp_ResponseCode", out var respCode))
                                {
                                    errorCodeValue = respCode;
                                }
                            }

                            // Redirect to failure page with error code
                            return RedirectToAction("OrderFailure", new { errorCode = errorCodeValue });
                        }
                    }

                    // Fallback to query parameters if we can't extract from JSON
                    if (Request.Query.TryGetValue("vnp_ResponseCode", out var vnpResponseCode))
                    {
                        if (vnpResponseCode == "00")
                        {
                            if (Request.Query.TryGetValue("vnp_TxnRef", out var orderIdValue) &&
                                int.TryParse(orderIdValue, out int orderId))
                            {
                                return RedirectToAction("OrderSuccess", new { orderId = orderId });
                            }

                            return RedirectToAction("OrderSuccess");
                        }
                        else
                        {
                            return RedirectToAction("OrderFailure", new { errorCode = vnpResponseCode });
                        }
                    }

                    // Fallback
                    return RedirectToAction("OrderFailure", new { errorCode = "UnknownResponse" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Callback error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    return RedirectToAction("OrderFailure", new { errorCode = "Exception" });
                }
            }

            return RedirectToAction("OrderFailure", new { errorCode = "NoQueryString" });
        }

// GET: Order/Success - Hiển thị trang thanh toán thành công
        [HttpGet]
        [Route("Order/Success")]
        public async Task<IActionResult> OrderSuccess(int? orderId)
        {
            if (orderId.HasValue)
            {
                var order = await _orderService.GetOrderById(orderId.Value);
                if (order != null)
                {
                    return View(order);
                }
            }
    
            // Fallback if order not found
            return View();
        }

// GET: Order/Failure - Hiển thị trang thanh toán thất bại
        [HttpGet]
        [Route("Order/Failure")]
        public IActionResult OrderFailure(string errorCode = "Unknown")
        {
            ViewBag.ErrorCode = errorCode;
            return View();
        }

        // GET: api/Order/ipn - Xử lý IPN từ VNPAY
        [HttpGet]
        [Route("api/order/ipn")]
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