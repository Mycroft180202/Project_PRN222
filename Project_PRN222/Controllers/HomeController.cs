using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Attributes;
using Project_PRN222.Models;
using System.Diagnostics;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public HomeController(
            ILogger<HomeController> logger,
            ICategoryService categoryService,
            IProductService productService,
            IOrderService orderService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _productService = productService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            // Load categories and featured products for the home page
            var categories = _categoryService.GetAllCategories();
            var featuredProducts = _productService.GetAllProducts().Take(8).ToList(); // Get first 8 products
            
            // Create a view model to hold both data sets
            var viewModel = new HomeViewModel
            {
                Categories = categories,
                FeaturedProducts = featuredProducts
            };
            
            return View(viewModel); // Pass the view model to the view
        }

        // Admin Dashboard View
        [HttpGet("AdminDashboard")]
        [RoleAuthorize(1)] // Admin only access
        public IActionResult AdminDashboard()
        {
            return View("AdminDashboard"); // Assuming you have AdminDashboard.cshtml in Views/Home
        }

        // Vendor Dashboard View
        [HttpGet("VendorDashboard")]
        [RoleAuthorize(2)] // Vendor only access
        public IActionResult VendorDashboard()
        {
            return View("VendorDashboard"); // Assuming you have VendorDashboard.cshtml in Views/Home
        }

        public IActionResult Privacy()
        {
            return View(); // Returns Views/Home/Privacy.cshtml
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Returns Views/Home/Error.cshtml
        }

        
    }
}