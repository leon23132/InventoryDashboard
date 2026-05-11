using InventoryDashboard.Api.Data;
using InventoryDashboard.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Api.Services
{
    public class CategoryService
    {
        private readonly InventoryDbContext _context;

        public CategoryService(InventoryDbContext context)
        {
            _context = context;
        }

        //Get Categories with Filters
        public async Task<List<CategoryListItemDto>> GetAllAsync(
            string? q, int page, int pageSize)
        {
            var query = _context.Categories
            .AsNoTracking()
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                //Trim input
                var s = q.Trim();
                //Filter for Category Name
                query = query.Where(c => c.Name.Contains(s));
            }

            // Pagination validation
            if (page <= 0) page = 1;

            if (pageSize <= 0) pageSize = 10;

            if (pageSize > 100) pageSize = 100;

            return await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CategoryListItemDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();
        }

        //Get Category by Id
        public async Task<CategoryDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.CategoryId == id)
                .Select(c => new CategoryDetailDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .FirstOrDefaultAsync();
        }

        //Create Category
        public async Task<int> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category.CategoryId;
        }

        //Update Category
        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            //Find Category
            var category = await _context.Categories.FindAsync(id);
            //Check if Category exists
            if (category is null) return false;


            category.Name = dto.Name;

            await _context.SaveChangesAsync();
            return true;
        }
        //Delete Category
        public async Task<bool> DeleteAsync(int id)
        {
            //Find Category
            var category = await _context.Categories.FindAsync(id);
            //Check if Category exists
            if (category is null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}