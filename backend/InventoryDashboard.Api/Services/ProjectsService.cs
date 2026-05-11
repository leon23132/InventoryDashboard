using InventoryDashboard.Api.Dtos.Projects;
using Microsoft.EntityFrameworkCore;
using InventoryDashboard.Api.Data;


namespace InventoryDashboard.Api.Services

{
    public class ProjectsService
    {
        private readonly InventoryDbContext _context;
        public ProjectsService(InventoryDbContext context)
        {
            _context = context;
        }


        //Get Projects with Filters
        public async Task<List<ProjectListItemDTO>> GetAllAsync(string? q, int page, int pageSize)
        {
            var query = _context.Projects
            .AsNoTracking()
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var s = q.Trim();
                query = query.Where(p => p.ProjectName.Contains(s));
            }

            // Pagination validation
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            return await query
                .OrderBy(p => p.ProjectName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProjectListItemDTO
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    Products = p.ProductProjects
                .Select(pp => new ProjectProductDto
                {
                    ProductId = pp.ProductId,
                    ProductTitle = pp.Product.ProductTitle,
                    Quantity = pp.Quantity,
                    UnitPrice = pp.Product.Price

                })
                .ToList()
                })
            .ToListAsync();

        }
        //Get Project by Id
        public async Task<ProjectDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Projects
            .AsNoTracking()
            .Where(p => p.ProjectId == id)
            .Select(p => new ProjectDetailDto
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                Description = p.Description,
                Products = p.ProductProjects
                .Select(pp => new ProjectProductDto
                {
                    ProductId = pp.ProductId,
                    ProductTitle = pp.Product.ProductTitle,
                    Quantity = pp.Quantity,
                    UnitPrice = pp.Product.Price
                })
                .ToList()
            })
            .FirstOrDefaultAsync();
        }
        //Create Project
        public async Task<int> CreateAsync(CreateProjectDto dto)
        {
            if (dto.Products == null || dto.Products.Count == 0) throw new ArgumentException("At least one product must be specified for the project.");
            // Map DTO to Entity
            var entity = new Entities.Project
            {
                ProjectName = dto.ProjectName,
                Description = dto.Description,
                ProductProjects = dto.Products.Select(p => new Entities.ProductProject
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList()
            };

            // Save to Database
            _context.Projects.Add(entity);
            await _context.SaveChangesAsync();
            return entity.ProjectId;
        }

        public async Task<bool> UpdateAsync(int id, UpdateProjectDto dto)
        {
            // Find existing entity
            var entity = await _context.Projects
            .Include(p => p.ProductProjects)
            .FirstOrDefaultAsync(p => p.ProjectId == id);

            //Check if entity exists
            if (entity is null) return false;

            // Update fields
            entity.ProjectName = dto.ProjectName;
            entity.Description = dto.Description;

            // If Products are provided, update the product list
            // Produkte updaten (nur wenn mitgegeben)
            if (dto.Products != null)
            {
                // alte Zuordnungen löschen
                entity.ProductProjects.Clear();

                // neue Zuordnungen setzen
                entity.ProductProjects = dto.Products.Select(p => new Entities.ProductProject
                {
                    ProjectId = entity.ProjectId,   // optional, EF setzt das oft selbst
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList();
            }
            // Save changes
            await _context.SaveChangesAsync();
            return true;
        }

        //Delete Project
        public async Task<bool> DeleteAsync(int id)
        {
            // Find existing entity
            var entity = await _context.Projects.FindAsync(id);

            // If not found, return false
            if (entity is null) return false;

            // Remove entity
            _context.Projects.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }



    }

}