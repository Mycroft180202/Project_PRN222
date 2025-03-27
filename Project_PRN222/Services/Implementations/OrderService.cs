using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;
using Project_PRN222.Services.Interfaces;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;

namespace Project_PRN222.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly VnpayPayment _vnpayPayment;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IDeliveryRepository deliveryRepository,
            ICartRepository cartRepository,
            IHttpContextAccessor httpContextAccessor,
            VnpayPayment vnpayPayment)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _deliveryRepository = deliveryRepository;
            _cartRepository = cartRepository;
            _httpContextAccessor = httpContextAccessor;
            _vnpayPayment = vnpayPayment;
        }

        private int GetCurrentUserId()
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0) throw new Exception("User not logged in.");
            return userId;
        }

        public async Task<string> Checkout(string shippingAddress, string billingAddress, int shipmentMethodId, string paymentMethod)
        {
            var userId = GetCurrentUserId();
            var cartItems = await _cartRepository.GetByUserId(userId);
            if (cartItems == null || !cartItems.Any()) throw new Exception("Cart is empty.");

            if (cartItems.Any(item => item.Product == null))
            {
                throw new Exception("One or more cart items have no associated product.");
            }

            decimal totalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                OrderStatus = "Pending",
                ShippingAddress = shippingAddress,
                BillingAddress = billingAddress,
                PaymentMethod = paymentMethod,
                ShipmentMethodId = shipmentMethodId,
                CreatedDate = DateTime.Now
            };
            var createdOrder = await _orderRepository.CreateOrder(order);

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = createdOrder.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price,
                    ProductName = item.Product.ProductName
                };
                await _orderItemRepository.AddOrderItem(orderItem);
            }

            var delivery = new Delivery
            {
                OrderId = createdOrder.OrderId,
                DeliveryStatus = "Pending",
                ShipmentMethodId = shipmentMethodId,
                ShippingAddress = shippingAddress,
                CreatedDate = DateTime.Now
            };
            await _deliveryRepository.AddDelivery(delivery);

            if (paymentMethod == "VNPAY")
            {
                var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                var request = new PaymentRequest
                {
                    PaymentId = createdOrder.OrderId,
                    Money = (double)totalAmount,
                    Description = $"Thanh toan don hang {createdOrder.OrderId}",
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = DateTime.Now,
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese
                };

                return _vnpayPayment.CreatePaymentUrl(request);
            }

            foreach (var item in cartItems)
            {
                await _cartRepository.Delete(item.CartItemId);
            }
            return "Order created successfully.";
        }

        public async Task<IActionResult> ProcessVnpayCallback(IQueryCollection query)
        {
            Console.WriteLine($"Callback Query: {string.Join(", ", query.Select(q => $"{q.Key}: {q.Value}"))}");

            var paymentResult = _vnpayPayment.GetPaymentResult(query);
            if (paymentResult == null)
            {
                Console.WriteLine("PaymentResult is null.");
                return new BadRequestObjectResult(new { Message = "Payment result is null." });
            }

            // Tạo response dạng JSON
            var response = new
            {
                paymentId = query["vnp_TxnRef"],
                isSuccess = paymentResult.IsSuccess,
                description = paymentResult.PaymentResponse?.Description ?? "No description",
                timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                vnpayTransactionId = query["vnp_TransactionNo"],
                paymentMethod = query["vnp_CardType"],
                paymentResponse = new
                {
                    code = (int?)paymentResult.PaymentResponse?.Code ?? -1, 
                    description = paymentResult.PaymentResponse?.Description ?? "Unknown"
                },
                transactionStatus = new
                {
                    code = (int?)paymentResult.TransactionStatus?.Code ?? -1,
                    description = paymentResult.TransactionStatus?.Description ?? "Unknown"
                },
                bankingInfo = new
                {
                    bankCode = query["vnp_BankCode"],
                    bankTransactionId = query["vnp_BankTranNo"]
                }
            };

            if (paymentResult.IsSuccess)
            {
                if (!query.TryGetValue("vnp_TxnRef", out var txnRef))
                {
                    Console.WriteLine("vnp_TxnRef not found.");
                    return new BadRequestObjectResult(new { Message = "Cannot retrieve OrderId from vnp_TxnRef." });
                }

                int orderId = int.Parse(txnRef);
                var order = await _orderRepository.GetById(orderId);
                if (order == null)
                {
                    Console.WriteLine($"Order {orderId} not found.");
                    return new BadRequestObjectResult(new { Message = $"Order {orderId} not found." });
                }

                order.OrderStatus = "Paid";
                order.UpdatedDate = DateTime.Now;
                await _orderRepository.UpdateOrder(order);
                Console.WriteLine($"Order {orderId} updated to Paid.");

                if (order.UserId.HasValue)
                {
                    var cartItems = await _cartRepository.GetByUserId(order.UserId.Value);
                    if (cartItems != null && cartItems.Any())
                    {
                        foreach (var item in cartItems)
                        {
                            await _cartRepository.Delete(item.CartItemId);
                        }
                        Console.WriteLine($"Deleted {cartItems.Count()} cart items for UserId {order.UserId.Value}.");
                    }
                }

                return new ObjectResult(response) { StatusCode = 200 };
            }

            return new BadRequestObjectResult(response);
        }
    }
}