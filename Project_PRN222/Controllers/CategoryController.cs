using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Attributes;
using Project_PRN222.Models;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Xem danh sách danh mục - mọi vai trò đều được phép
        public IActionResult Index()
        {
            var categories = _categoryService.GetAllCategories();
            return View(categories);
        }

        [HttpGet("api/categories")]
        public IActionResult GetCategoriesApi()
        {
            var categories = _categoryService.GetAllCategories();
            return Ok(categories);
        }

        [HttpGet("api/categories/{id}")]
        public IActionResult GetCategoryByIdApi(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // Chỉ Admin được tạo danh mục
        [HttpPost("api/categories")]
        [RoleAuthorize(1)] 
        public IActionResult CreateCategoryApi([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _categoryService.CreateCategory(category);
            return CreatedAtAction(nameof(GetCategoryByIdApi), new { id = category.CategoryId }, category);
        }

        // Chỉ Admin được sửa danh mục
        [HttpPut("api/categories/{id}")]
        [RoleAuthorize(1)] 
        public IActionResult UpdateCategoryApi(int id, [FromBody] Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest("ID in request path and body do not match.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingCategory = _categoryService.GetCategoryById(id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            _categoryService.UpdateCategory(category);
            return NoContent();
        }

        // Chỉ Admin được xóa danh mục
        [HttpDelete("api/categories/{id}")]
        [RoleAuthorize(1)] 
        public IActionResult DeleteCategoryApi(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            _categoryService.DeleteCategory(id);
            return NoContent();
        }
    }
}