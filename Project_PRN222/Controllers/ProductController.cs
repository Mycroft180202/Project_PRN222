using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Attributes;
using Project_PRN222.DTO;
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

        // Xem danh sách sản phẩm - mọi vai trò đều được phép
        public IActionResult Index()
        {
            var products = _productService.GetAllProducts();
            return View(products); // Assuming you have an Index.cshtml in Views/Product to display products
        }

        [HttpGet("api/products")]
        public IActionResult Products()
        {
            var products = _productService.GetAllProducts();
            var productDtos = products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                VendorId = p.VendorId,
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate
            });
            return Ok(productDtos);
        }

        [HttpGet("api/products/{id}")]
        public IActionResult GetProductByIdApi(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                VendorId = product.VendorId,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate
            };

            return Ok(productDto);
        }
        // GET: Product/Detail/5
        public IActionResult Detail(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
    
            
            ViewBag.CategoryName = product.Category?.CategoryName ?? "Uncategorized";
            ViewBag.VendorName = product.Vendor?.VendorName ?? "Unknown Vendor";
    
            
            if (product.CategoryId.HasValue)
            {
                var relatedProducts = _productService.GetAllProducts()
                    .Where(p => p.CategoryId == product.CategoryId && p.ProductId != product.ProductId)
                    .Take(4)
                    .ToList();
                ViewBag.RelatedProducts = relatedProducts;
            }
    
            return View(product);
        }

        // Action to return Create Product View (Admin, Vendor)
        [HttpGet("CreateView")]
        [RoleAuthorize(1, 2)]
        public IActionResult Create()
        {
            return View("Create"); // Assuming you have a Create.cshtml in Views/Product for creating products
        }

        [HttpPost("api/products")]
        [RoleAuthorize(1, 2)]
        public IActionResult CreateProductApi([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Request body is empty or invalid.");
            }

            var roleId = int.Parse(HttpContext.Session.GetString("RoleId") ?? "0");
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                if (roleId != 1)
                {
                    return BadRequest(ModelState);
                }
            }

            _productService.CreateProduct(product);

            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                VendorId = product.VendorId,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate
            };

            return CreatedAtAction(nameof(GetProductByIdApi), new { id = productDto.ProductId }, productDto);
        }

        // Action to return Edit Product View (Admin, Vendor)
        [HttpGet("EditView/{id}")]
        [RoleAuthorize(1, 2)]
        public IActionResult EditView(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("Edit", product); // Assuming you have an Edit.cshtml in Views/Product for editing products, passing the product model
        }


        // Chỉ Admin và Vendor (nếu sở hữu sản phẩm) được sửa
        [HttpPut("api/products/{id}")]
        [RoleAuthorize(1, 2)]
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

        // Action to return Delete Product Confirmation View (Admin, Vendor) - Optional
        [HttpGet("DeleteConfirmationView/{id}")]
        [RoleAuthorize(1, 2)]
        public IActionResult DeleteConfirmationView(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("DeleteConfirmation", product); // Assuming you have a DeleteConfirmation.cshtml in Views/Product
        }


        // Chỉ Admin và Vendor (nếu sở hữu sản phẩm) được xóa
        [HttpDelete("api/products/{id}")]
        [RoleAuthorize(1, 2)]
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