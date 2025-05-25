using CarRentalSystem.DataAccess.Entities;
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

        public async Task<VehicleType> GetByIdAsync(int id)
        {
            return await _context.VehicleTypes.FindAsync(id);
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
            return await _context.VehicleBrands.FindAsync(id);
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
            return await _context.VehicleModels.FindAsync(id);
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

        public async Task<Vehicle?> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
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
