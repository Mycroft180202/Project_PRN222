using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Models;
using Project_PRN222.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Project_PRN222.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public CategoryMenuViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IViewComponentResult Invoke()
        {
            // Get all categories
            var allCategories = _categoryService.GetAllCategories();
            
            // Filter to get only parent categories (those with no ParentCategoryId)
            var parentCategories = allCategories.Where(c => c.ParentCategoryId == null).ToList();
            
            return View(parentCategories);
        }
    }
}