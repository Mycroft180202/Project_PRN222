using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project_PRN222.Models;

namespace Project_PRN222.Pages.cart
{
    public class IndexModel : PageModel
    {
        private ProjectPrn222Context _context;
        public List<CartItem> Carts { get; set; }

        public IndexModel(ProjectPrn222Context context)
        {
            _context = context;
        }


        public async Task<IActionResult> OnGet(int? pid, int? quantity)
        {

            int userId = 1;
            if (pid == null || quantity == null)
            {
                Carts = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Product)
                    .ToListAsync();
                return Page();
            }

            if (pid <= 0 || quantity <= 0)
            {
                return BadRequest("Invalid product or quantity.");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == pid);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == pid);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity.Value;
            }
            else
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = pid.Value,
                    Quantity = quantity.Value,
                    AddedDate = DateTime.UtcNow
                };
                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        [HttpPost]
        public JsonResult OnPostDeleteItem([FromForm] int productId)
        {
            var item = _context.CartItems.FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();
                return new JsonResult(new { success = true, productId });
            }

            return new JsonResult(new { success = false, message = "Product not found." });
        }

        [HttpPost]
        public JsonResult OnPostUpdateQuantity([FromForm] int productId, [FromForm] int quantity)
        {
            if (quantity < 1)
            {
                return new JsonResult(new { success = false, message = "Quantity must be at least 1." });
            }

            var item = _context.CartItems.Include(c => c.Product).FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                if(quantity > item.Product.StockQuantity)
                {
                    return new JsonResult(new { success = false, message = "Out of stock." });
                }
                item.Quantity = quantity;
                _context.SaveChanges();
                decimal newTotalPrice = item.Quantity * item.Product.Price;
                return new JsonResult(new { success = true, productId, newQuantity = quantity, newTotalPrice });
            }

            return new JsonResult(new { success = false, message = "Product not found." });
        }

    }
}

