using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project_PRN222.Models;

namespace Project_PRN222.Pages.checkout
{
    public class IndexModel : PageModel
    {
        private readonly ProjectPrn222Context _context;

        public IndexModel(ProjectPrn222Context context)
        {
            _context = context;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<ShipmentMethod> ShipmentMethods { get; set; } = new List<ShipmentMethod>();

        public User currUser { get; set; }
        public decimal TotalPrice { get; set; }

        public async Task<IActionResult> OnGet()
        {
            int userId = 1;

            currUser = await _context.Users.FirstOrDefaultAsync(c => c.UserId == userId);

            CartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            ShipmentMethods = await _context.ShipmentMethods.ToListAsync();

            TotalPrice = CartItems.Sum(c => (c.Product?.Price ?? 0) * c.Quantity);

            return Page();
        }

        [HttpPost]
        public JsonResult OnPostCheckout([FromForm] string payment, [FromForm] string shipment, [FromForm] string address)
        {
            int userId = 1;

            var cartItems = _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            if (cartItems.Count == 0)
            {
                return new JsonResult(new { success = false, message = "Cart is empty." });
            }

            var shipmentMethod = _context.ShipmentMethods.FirstOrDefault(c => c.ShipmentMethodId == int.Parse(shipment));

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                ShipmentMethodId = shipmentMethod.ShipmentMethodId,
                PaymentMethod = payment,
                ShippingAddress = address,
                BillingAddress = address,
                OrderStatus = "Not paid",
                TotalAmount = cartItems.Sum(c => (c.Product?.Price ?? 0) * c.Quantity) + shipmentMethod.Cost
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            var delivery = new Delivery
            {
                Order = order,
                DeliveryStatus = "Pending",
                ShipmentMethodId = shipmentMethod.ShipmentMethodId,
                ShippingAddress = address
            };
            _context.Deliveries.Add(delivery);
            _context.SaveChanges();

            foreach (var cartItem in cartItems)
            {
                var orderDetail = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId ?? 0,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Product?.Price ?? 0,
                    ProductName = cartItem.Product?.ProductName ?? string.Empty
                };

                _context.OrderItems.Add(orderDetail);
            }

            _context.SaveChanges();

            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            return new JsonResult(new { success = true, message = "Place order successfully." });
        }
    }
}
