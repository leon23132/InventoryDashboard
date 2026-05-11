using Microsoft.AspNetCore.Mvc;
using InventoryDashboard.Api.Services;
using InventoryDashboard.Api.Dtos.Products;

namespace InventoryDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        //Get Products with Filters
        [HttpGet]
        public async Task<ActionResult<List<ProductListItemDto>>> GetAll(
         [FromQuery] string? search,
         [FromQuery] int? categoryId,
         [FromQuery] int? supplierId,
         [FromQuery] int page = 1,
         [FromQuery] int pageSize = 10
         )
        {
            var items = await _productService.GetAllAsync(search, categoryId, supplierId, page, pageSize);
            return Ok(items);
        }

        //Get Product by Id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDetailDto>> GetById(int id)
        {
            var item = await _productService.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        //Create Product
        [HttpPost]
        public async Task<ActionResult<CreateProductDto>> Create([FromBody] CreateProductDto dto)
        {

            var id = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        //Update Product
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UpdateProductDto>> Update(int id, [FromBody] UpdateProductDto dto)
        {

            var ok = await _productService.UpdateAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }

        //Delete Product
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _productService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}