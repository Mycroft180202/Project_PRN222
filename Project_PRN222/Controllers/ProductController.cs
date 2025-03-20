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
            return View(products);
        }

        // Hiển thị chi tiết sản phẩm
        [HttpGet("Detail/{id}")]
        public IActionResult Detail(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            // Bỏ phần gọi GetRelatedProducts để tránh lỗi
            ViewBag.RelatedProducts = new List<Product>(); // Truyền danh sách rỗng để tránh lỗi trong view
            return View(product);
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

        // Action to return Create Product View (Admin, Vendor)
        [HttpGet("Product/CreateView")]
        public IActionResult Create()
        {
            return View("CreateView");
        }

        [HttpPost("api/products")]
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
        public IActionResult EditView(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("Edit", product);
        }

        // Chỉ Admin và Vendor (nếu sở hữu sản phẩm) được sửa
        [HttpPut("api/products/{id}")]
public IActionResult UpdateProductApi(int id, [FromBody] Product product)
{
    try
    {
        if (id != product.ProductId)
        {
            return BadRequest("ID in request path and body do not match.");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage);
            return BadRequest(new { Errors = errors });
        }

        var existingProduct = _productService.GetProductById(id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        // Cập nhật các trường cần thiết
        existingProduct.ProductName = product.ProductName;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.StockQuantity = product.StockQuantity;
        existingProduct.ImageUrl = product.ImageUrl;
        existingProduct.CategoryId = product.CategoryId;
        existingProduct.VendorId = product.VendorId; // Giữ nguyên nếu có quyền chỉnh sửa
        existingProduct.UpdatedDate = DateTime.Now; // Cập nhật thời gian chỉnh sửa

        _productService.UpdateProduct(existingProduct);
        return NoContent();
    }
    catch (Exception ex)
    {
        // Log lỗi để debug
        Console.WriteLine($"Error updating product: {ex.Message}");
        return StatusCode(500, "An error occurred while updating the product.");
    }
}

        // Action to return Delete Product Confirmation View (Admin, Vendor) - Optional
        [HttpGet("DeleteConfirmationView/{id}")]
        public IActionResult DeleteConfirmationView(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("DeleteConfirmation", product);
        }

        // Chỉ Admin và Vendor (nếu sở hữu sản phẩm) được xóa
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