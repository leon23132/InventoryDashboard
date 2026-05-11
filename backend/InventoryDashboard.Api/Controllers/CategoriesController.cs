using Microsoft.AspNetCore.Mvc;
using InventoryDashboard.Api.Services;
namespace InventoryDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //Get All Categories with optional filter
        [HttpGet]
        public async Task<ActionResult<List<CategoryListItemDto>>> GetAll(
            [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var items = await _categoryService.GetAllAsync(search, page, pageSize);
            return Ok(items);
        }

        //Get Category by Id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDetailDto>> GetById(int id)
        {
            var item = await _categoryService.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        //Create Category
        [HttpPost]
        public async Task<ActionResult<CategoryDetailDto>> Create([FromBody] CreateCategoryDto dto)
        {
            //Check Model State

            var id = await _categoryService.CreateAsync(dto);
            //CreateAtAction with GetById
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        //Update Category
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UpdateCategoryDto>> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            //Check Ids match
            var ok = await _categoryService.UpdateAsync(id, dto);

            return ok ? NoContent() : NotFound();
        }

        //Delete Category
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            //Delete Category
            var ok = await _categoryService.DeleteAsync(id);
            //Return NoContent or NotFound
            return ok ? NoContent() : NotFound();
        }


    }
}