using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.DataAccess.Entities.Enums;
using CarRentalSystem.DataAccess.Filters;
using CarRentalSystem.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.DataAccess.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleType>> GetAllTypesAsync()
        {
            return await _context.VehicleTypes.ToListAsync();
        }

        public async Task<VehicleType> GetTypeByIdAsync(int id)
        {
            return await _context.VehicleTypes
                .Include(vt => vt.VehicleModels)
                .FirstAsync(vt => vt.Id == id);
        }

        public async Task AddVehicleTypeAsync(VehicleType type)
        {
            await _context.VehicleTypes.AddAsync(type);
            await SaveChangesAsync();
        }

        public async Task DeleteVehicleTypeAsync(VehicleType type)
        {
            _context.VehicleTypes.Remove(type);
            await SaveChangesAsync();
        }

        public async Task<IEnumerable<VehicleBrand>> GetAllBrandsAsync()
        {
            return await _context.VehicleBrands.ToListAsync();
        }

        public async Task<IEnumerable<VehicleBrand>> GetBrandsByTypeAsync(int typeId)
        {
            return await _context.VehicleModels
                .Where(m => m.VehicleTypeId == typeId)
                .Select(m => m.Brand)
                .Distinct()
                .ToListAsync();
        }

        public async Task<VehicleBrand> GetBrandByIdAsync(int id)
        {
            return await _context.VehicleBrands
                      .Include(b => b.Models)
                      .FirstAsync(b => b.Id == id);
        }

        public async Task AddVehicleBrandAsync(VehicleBrand brand)
        {
            await _context.VehicleBrands.AddAsync(brand);
            await SaveChangesAsync();
        }


        public async Task DeleteVehicleBrandAsync(VehicleBrand brand)
        {
            _context.VehicleBrands.Remove(brand);
            await SaveChangesAsync();
        }
        public async Task<IEnumerable<VehicleModel>> GetModelsByBrandAndTypeAsync(int brandId, int typeId)
        {
            return await _context.VehicleModels
                .Where(m => m.BrandId == brandId && m.VehicleTypeId == typeId)
                .ToListAsync();
        }
        public async Task<IEnumerable<VehicleModel>> GetAllVehicleModelsAsync()
        {
            return await _context.VehicleModels
                .Include(vm => vm.Brand)
                .Include(v => v.VehicleType)
                .ToListAsync();
        }

        public async Task<VehicleModel?> GetVehicleModelByIdAsync(int id)
        {
            return await _context.VehicleModels
                     .Include(vm => vm.Vehicles)
                     .FirstOrDefaultAsync(vm => vm.Id == id);
        }

        public async Task AddVehicleModelAsync(VehicleModel model)
        {
            await _context.VehicleModels.AddAsync(model);
            await SaveChangesAsync();
        }

        public async Task<bool> VehicleModelExistsAsync(string name, int brandId)
        {
            return await _context.VehicleModels.AnyAsync(vm => vm.Name == name && vm.BrandId == brandId);
        }
        public async Task<bool> TypeExistsAsync(string typeName)
        {
            return await _context.VehicleTypes.AnyAsync(v => v.TypeName.ToLower() == typeName.ToLower());
        }

        public async Task<bool> BrandExistsAsync(string name)
        {
            return await _context.VehicleBrands.AnyAsync(v => v.Name.ToLower() == name.ToLower());
        }

        public async Task DeleteVehicleModelAsync(VehicleModel model)
        {
            _context.VehicleModels.Remove(model);
            await SaveChangesAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.Vehicles
                .Include(v => v.VehicleModel)
                    .ThenInclude(vm => vm.Brand)
                .Include(v => v.VehicleModel)
                    .ThenInclude(vm => vm.VehicleType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetFilteredVehiclesAsync(VehicleFilterDal filter)
        {
            var query = _context.Vehicles
                .Include(v => v.VehicleModel)
                    .ThenInclude(vm => vm.VehicleType)
                .Include(v => v.VehicleModel)
                    .ThenInclude(vm => vm.Brand)
                .AsQueryable();

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                var startDateWithAddedHours = filter.StartDate.Value.AddHours(2);
                var endDateWithAddedHours = filter.EndDate.Value.AddHours(2);

                var startDateUtc = startDateWithAddedHours.ToUniversalTime();
                var endDateUtc = endDateWithAddedHours.ToUniversalTime();

                query = query.Where(vehicle => !vehicle.Reservations.Any(r =>
                    r.Status != ReservationStatus.Completed &&
                    r.Status != ReservationStatus.Cancelled &&
                    r.StartDate < endDateUtc &&
                    r.EndDate > startDateUtc));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(v => v.DailyRate >= filter.MinPrice);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(v => v.DailyRate <= filter.MaxPrice);
            }

            if (filter.SelectedVehicleTypeIds != null && filter.SelectedVehicleTypeIds.Any())
            {
                query = query.Where(v => filter.SelectedVehicleTypeIds.Contains(v.VehicleModel.VehicleTypeId));
            }

            return await query.ToListAsync();
        }



        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles
                .Include(v => v.Reservations)
                  .Include(v => v.VehicleModel)
                    .ThenInclude(vm => vm.Brand)
                .Include(v => v.VehicleModel)
                    .ThenInclude(vm => vm.VehicleType)
                .FirstAsync(v => v.Id == id);
        }

        public async Task AddVehicleAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
            await SaveChangesAsync();
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await SaveChangesAsync();
        }

        public async Task DeleteVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
