using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project_PRN222.Models;

namespace Project_PRN222.Pages.order
{
    public class IndexModel : PageModel
    {
        private readonly ProjectPrn222Context _context;

        public IndexModel(ProjectPrn222Context context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true, Name = "oid")]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (OrderId <= 0)
            {
                return BadRequest("Invalid order ID.");
            }

            Order = await _context.Orders
                .Include(o => o.Delivery)
                .Include(o => o.ShipmentMethod)
                .Include(o => o.OrderItems)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == OrderId);

            if (Order == null)
            {
                return NotFound("Order not found.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int orderId)
        {
            var order = await _context.Orders.Include(o => o.Delivery).FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null || order.OrderStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) || order.OrderStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            order.OrderStatus = "Cancelled";
            order.Delivery.DeliveryStatus = "Cancelled";
            await _context.SaveChangesAsync();

            return Redirect("order?oid=" + orderId);
        }
    }
}
