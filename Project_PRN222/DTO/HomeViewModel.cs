using System.Collections.Generic;

namespace Project_PRN222.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> FeaturedProducts { get; set; }
        public int ProductId { get; set; }
    }
}