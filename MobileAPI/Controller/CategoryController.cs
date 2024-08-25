using Application.InterfaceService;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _CategoryService;
        private readonly ICurrentTime _currentTime;
        public CategoryController(ICategoryService CategoryService, ICurrentTime currentTime)
        {
            _CategoryService = CategoryService;
            _currentTime = currentTime;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var Categorys = await _CategoryService.GetAllCategory();
            return Ok(Categorys);
        }
        [HttpGet]
        public async Task<IActionResult> GetCurrentTime()
        {
            var Categorys = _currentTime.GetCurrentTime();
            return Ok(Categorys);
        }
    }
}

