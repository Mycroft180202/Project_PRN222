﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("api/products")]
        public IActionResult GetProductsApi()
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

            // Chuyển Product thành ProductDto để trả về
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
