using InventoryDashboard.Api.Dtos.Suppliers;
using InventoryDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly SuppliersService _suppliersService;

        public SuppliersController(SuppliersService suppliersService)
        {
            _suppliersService = suppliersService;
        }

        //Get Suppliers with Filters
        [HttpGet]
        public async Task<ActionResult<List<SupplierListItemDto>>> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? contactPerson,
            [FromQuery] string? city,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            var items = await _suppliersService.GetAllAsync(search, contactPerson, city, page, pageSize);
            return Ok(items);
        }

        //Get Supplier by Id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SupplierDto>> GetById(int id)
        {
            var item = await _suppliersService.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        //Create Supplier
        [HttpPost]
        public async Task<ActionResult<SupplierDto>> Create([FromBody] CreateSupplierDto dto)
        {
            var id = await _suppliersService.CreateAsync(dto);
            //CreateAtAction with GetById
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        //Update Supplier
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SupplierDto>> Update(int id, [FromBody] UpdateSupplierDto dto)
        {
            //Update Supplier
            var ok = await _suppliersService.UpdateAsync(id, dto);
            //Return NoContent or NotFound
            return ok ? NoContent() : NotFound();
        }

        //Delete Supplier
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            //Delete Supplier
            var ok = await _suppliersService.DeleteAsync(id);
            //Return NoContent or Not Found
            return ok ? NoContent() : NotFound();
        }
    }
}