using InventoryDashboard.Api.Dtos.Suppliers;
using Microsoft.EntityFrameworkCore;
using InventoryDashboard.Api.Data;

namespace InventoryDashboard.Api.Services
{
    public class SuppliersService
    {
        private readonly InventoryDbContext _context;

        public SuppliersService(InventoryDbContext context)
        {
            _context = context;
        }


        //GetSuppliers with Filters
        public async Task<List<SupplierListItemDto>> GetAllAsync(
            string? q,
            string? ContactPerson,
            string? City,
            int page,
            int pageSize)
        {
            //Query Start
            var query = _context.Suppliers
            .AsNoTracking()
            .Include(s => s.BillingAddress)
            .Include(s => s.ShippingAddress)
            .AsQueryable();

            //Filters for Company Name
            if (!string.IsNullOrWhiteSpace(q))
            {
                //Trim input
                var s = q.Trim();

                //Filter for Company Name and Contact Person
                query = query.Where(p =>
                    p.CompanyName.Contains(s) ||
                    (p.ContactPerson != null && p.ContactPerson.Contains(s))
                    );
            }

            //Filter for Contact Person
            if (!string.IsNullOrWhiteSpace(ContactPerson))
            {
                var c = ContactPerson.Trim();
                query = query.Where(p => p.ContactPerson != null && p.ContactPerson.Contains(c));
            }
            //Filter for City (Billing or Shipping)
            if (!string.IsNullOrWhiteSpace(City))
            {
                var c = City.Trim();
                query = query.Where(p =>
                    (p.BillingAddress != null && p.BillingAddress.City.Contains(c)) ||
                    (p.ShippingAddress != null && p.ShippingAddress.City.Contains(c))
                    );
            }

            //Pagination
            if (page <= 0) page = 1;
            //Default page size to 10, max 100
            if (pageSize <= 0) pageSize = 10;
            //Skip and Take for pagination
            if (pageSize > 100) pageSize = 100;

            //Execute Query and Return Results
            return await query
                .OrderBy(p => p.CompanyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new SupplierListItemDto
                {
                    SupplierId = p.SupplierId,
                    CompanyName = p.CompanyName,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    Website = p.Website,
                    ContactPerson = p.ContactPerson,
                    BillingAddress = new Dtos.Addresses.AddressDto
                    {
                        StreetAddress = p.BillingAddress.StreetAddress,
                        City = p.BillingAddress.City,
                        PostalCode = p.BillingAddress.PostalCode,
                        Country = p.BillingAddress.Country
                    },
                    ShippingAddress = p.ShippingAddress == null ? null : new Dtos.Addresses.AddressDto
                    {
                        StreetAddress = p.ShippingAddress.StreetAddress,
                        City = p.ShippingAddress.City,
                        PostalCode = p.ShippingAddress.PostalCode,
                        Country = p.ShippingAddress.Country
                    }
                }).ToListAsync();
        }

        //Get Supplier by Id
        public async Task<SupplierDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Suppliers.AsNoTracking()
             .Where(s => s.SupplierId == id)
             .Select(s => new SupplierDetailDto
             {
                 SupplierId = s.SupplierId,
                 CompanyName = s.CompanyName,
                 Email = s.Email,
                 PhoneNumber = s.PhoneNumber,
                 Website = s.Website,
                 ContactPerson = s.ContactPerson,
                 BillingAddress = new Dtos.Addresses.AddressDto
                 {
                     StreetAddress = s.BillingAddress.StreetAddress,
                     City = s.BillingAddress.City,
                     PostalCode = s.BillingAddress.PostalCode,
                     Country = s.BillingAddress.Country
                 },
                 ShippingAddress = s.ShippingAddress == null ? null : new Dtos.Addresses.AddressDto
                 {
                     StreetAddress = s.ShippingAddress.StreetAddress,
                     City = s.ShippingAddress.City,
                     PostalCode = s.ShippingAddress.PostalCode,
                     Country = s.ShippingAddress.Country
                 }
             })
             .FirstOrDefaultAsync();
        }

        //Create Supplier
        public async Task<int> CreateAsync(CreateSupplierDto dto)
        {
            //Map DTO to Entity
            var entity = new Entities.Supplier
            {
                CompanyName = dto.CompanyName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Website = dto.Website,
                ContactPerson = dto.ContactPerson,
                BillingAddress = new Entities.Address
                {
                    StreetAddress = dto.BillingAddress.StreetAddress,
                    City = dto.BillingAddress.City,
                    PostalCode = dto.BillingAddress.PostalCode,
                    Country = dto.BillingAddress.Country
                },
                ShippingAddress = dto.ShippingAddress == null ? null : new Entities.Address
                {
                    StreetAddress = dto.ShippingAddress.StreetAddress,
                    City = dto.ShippingAddress.City,
                    PostalCode = dto.ShippingAddress.PostalCode,
                    Country = dto.ShippingAddress.Country
                }
            };

            //Save to Database
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return entity.SupplierId;
        }

        //Uodate Supplier
        public async Task<bool> UpdateAsync(int id, UpdateSupplierDto dto)
        {
            // Find existing entity
            var entity = await _context.Suppliers
                .Include(s => s.BillingAddress)
                .Include(s => s.ShippingAddress)
                .FirstOrDefaultAsync(s => s.SupplierId == id);

            // If not found, return false
            if (entity is null) return false;

            // Update supplier properties
            entity.CompanyName = dto.CompanyName;
            entity.Email = dto.Email;
            entity.PhoneNumber = dto.PhoneNumber;
            entity.Website = dto.Website;
            entity.ContactPerson = dto.ContactPerson;

            // Update billing address
            if (entity.BillingAddress == null)
            {
                entity.BillingAddress = new Entities.Address();
            }

            entity.BillingAddress.StreetAddress = dto.BillingAddress.StreetAddress;
            entity.BillingAddress.City = dto.BillingAddress.City;
            entity.BillingAddress.PostalCode = dto.BillingAddress.PostalCode;
            entity.BillingAddress.Country = dto.BillingAddress.Country;

            // Update shipping address
            if (dto.ShippingAddress != null)
            {
                if (entity.ShippingAddress == null)
                {
                    entity.ShippingAddress = new Entities.Address();
                }

                entity.ShippingAddress.StreetAddress = dto.ShippingAddress.StreetAddress;
                entity.ShippingAddress.City = dto.ShippingAddress.City;
                entity.ShippingAddress.PostalCode = dto.ShippingAddress.PostalCode;
                entity.ShippingAddress.Country = dto.ShippingAddress.Country;
            }
            else
            {
                entity.ShippingAddress = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        //Delete Supplier
        public async Task<bool> DeleteAsync(int id)
        {
            //Find existing entity
            var entity = await _context.Suppliers.FindAsync(id);
            //If not found, return false
            if (entity is null) return false;

            //Remove entity from database
            _context.Suppliers.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}