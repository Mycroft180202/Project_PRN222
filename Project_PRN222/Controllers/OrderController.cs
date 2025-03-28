using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Attributes;
using Project_PRN222.DTO;
using Project_PRN222.Models;
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
        [HttpGet]
        [Route("api/Order/GetOrders")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var orders = await _orderService.GetOrdersWithDetails();
                var orderDtos = orders.Select(MapToOrderDto).ToList();
                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving orders: {ex.Message}" });
            }
        }
        private OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate ?? DateTime.Now,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                ShippingAddress = order.ShippingAddress,
                BillingAddress = order.BillingAddress,
                User = order.User != null ? new User { 
                    UserId = order.User.UserId,
                    UserName = order.User.UserName,
                    Email = order.User.Email,
                    PhoneNumber = order.User.PhoneNumber
                } : null,
                OrderItems = order.OrderItems?.Select(item => new OrderItemDto
                {
                    OrderItemId = item.OrderItemId,
                    ProductId = item.ProductId ?? 0,
                    ProductName = item.Product?.ProductName ?? $"Product #{item.ProductId}",
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            };
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            try
            {
                var order = await _orderService.GetOrderById(id);
                if (order == null)
                {
                    return NotFound(new { message = $"Order with ID {id} not found" });
                }
        
                // Create a DTO to avoid circular references
                var orderDto = new OrderDto
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate ?? DateTime.MinValue,
                    OrderStatus = order.OrderStatus,
                    TotalAmount = order.TotalAmount,
                    PaymentMethod = order.PaymentMethod,
                    ShippingAddress = order.ShippingAddress,
                    BillingAddress = order.BillingAddress,
                    User = order.User, // This is safe as long as User doesn't reference back to Orders
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId ?? 0,
                        ProductName = oi.ProductName, // This is safe as long as Product doesn't reference back to OrderItems
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                };
        
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving order details: {ex.Message}" });
            }
        }
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            try
            {
                var success = await _orderService.UpdateOrderStatus(id, "Confirmed");
                if (!success)
                {
                    return NotFound(new { message = $"Order with ID {id} not found or cannot be confirmed" });
                }
                return Ok(new { message = "Order confirmed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error confirming order: {ex.Message}" });
            }
        }
        [HttpPut("{id}/ship")]
        public async Task<IActionResult> ShipOrder(int id)
        {
            try
            {
                var success = await _orderService.UpdateOrderStatus(id, "Shipped");
                if (!success)
                {
                    return NotFound(new { message = $"Order with ID {id} not found or cannot be shipped" });
                }
                return Ok(new { message = "Order marked as shipped successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error shipping order: {ex.Message}" });
            }
        }
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var success = await _orderService.UpdateOrderStatus(id, "Cancelled");
                if (!success)
                {
                    return NotFound(new { message = $"Order with ID {id} not found or cannot be cancelled" });
                }
                return Ok(new { message = "Order cancelled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error cancelling order: {ex.Message}" });
            }
        }
        [HttpPut("{id}/deliver")]
        public async Task<IActionResult> DeliverOrder(int id)
        {
            try
            {
                var success = await _orderService.UpdateOrderStatus(id, "Delivered");
                if (!success)
                {
                    return NotFound(new { message = $"Order with ID {id} not found or cannot be marked as delivered" });
                }
                return Ok(new { message = "Order marked as delivered successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating order: {ex.Message}" });
            }
        }
        [HttpGet]
        [Route("Order/MyOrders")]
        [RoleAuthorize(1, 2, 3)] // Allow all authenticated users
        public async Task<IActionResult> MyOrders()
        {
            try
            {
                // Get current user ID from session
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Get all orders for the current user
                var orders = await _orderService.GetOrdersByUserId(userId);
                return View(orders);
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