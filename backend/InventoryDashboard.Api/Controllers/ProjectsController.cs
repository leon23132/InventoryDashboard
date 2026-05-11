using InventoryDashboard.Api.Dtos.Projects;
using InventoryDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectsService _projectsService;
        public ProjectsController(ProjectsService projectsService)
        {
            _projectsService = projectsService;
        }

        //Get Projects with Filters
        [HttpGet]
        public async Task<ActionResult<List<ProjectListItemDTO>>> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            var items = await _projectsService.GetAllAsync(search, page, pageSize);
            return Ok(items);
        }

        //Get Project by Id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDto>> GetById(int id)
        {
            var item = await _projectsService.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        //Create Project
        [HttpPost]
        public async Task<ActionResult<CreateProjectDto>> Create([FromBody] CreateProjectDto dto)
        {

            var id = await _projectsService.CreateAsync(dto);
            //CreateAtAction with GetById
            return CreatedAtAction(nameof(GetById), new { id = id }, new { id });
        }

        //Update Project
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UpdateProjectDto>> Update(int id, [FromBody] UpdateProjectDto dto)
        {
            //Update Project
            var ok = await _projectsService.UpdateAsync(id, dto);
            //Return NoContent or NotFound
            return ok ? NoContent() : NotFound();
        }

        //Delete Project
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _projectsService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

    }
}