using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Attributes;
using Project_PRN222.Models;
using System.Diagnostics;

namespace Project_PRN222.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(); // Returns Views/Home/Index.cshtml
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