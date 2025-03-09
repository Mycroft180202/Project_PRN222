using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project_PRN222.Models;

namespace Project_PRN222.Pages.orders
{
    public class IndexModel : PageModel
    {
        private ProjectPrn222Context _context;

        public IndexModel(ProjectPrn222Context context)
        {
            _context = context;
        }
        public List<Order> Orders { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            int userId = 1;

            Orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.ShipmentMethod)
                .Include(o => o.OrderItems)
                .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
