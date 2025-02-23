using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Models;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService; 

        public ProductController(IProductService productService) 
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            var products = _productService.GetAllProducts();
            return View(products); 
        }

        [HttpGet("api/products")] 
        public IActionResult GetProductsApi()
        {
            var products = _productService.GetAllProducts();
            return Ok(products); 
        }

        [HttpGet("api/products/{id}")] 
        public IActionResult GetProductByIdApi(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound(); 
            }
            return Ok(product); 
        }

        [HttpPost("api/products")] 
        public IActionResult CreateProductApi([FromBody] Product product) 
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState); 
            }
            _productService.CreateProduct(product);
            return CreatedAtAction(nameof(GetProductByIdApi), new { id = product.ProductId }, product); 
        }

        [HttpPut("api/products/{id}")] 
        public IActionResult UpdateProductApi(int id, [FromBody] Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest("ID in request path and body do not match."); 
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingProduct = _productService.GetProductById(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            _productService.UpdateProduct(product);
            return NoContent();
        }

        [HttpDelete("api/products/{id}")] 
        public IActionResult DeleteProductApi(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            _productService.DeleteProduct(id);
            return NoContent(); 
        }
    }
}
