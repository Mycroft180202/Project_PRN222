using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("api/categories")] 
        public IActionResult CreateCategoryApi([FromBody] Category category) 
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState); 
            }
            _categoryService.CreateCategory(category);
            return CreatedAtAction(nameof(GetCategoryByIdApi), new { id = category.CategoryId }, category);  
        }

        [HttpPut("api/categories/{id}")] 
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

        [HttpDelete("api/categories/{id}")] 
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
