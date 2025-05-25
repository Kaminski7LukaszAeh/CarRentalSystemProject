using CarRentalSystem.DataAccess.Entities;

namespace CarRentalSystem.DataAccess.Interfaces
{
    public interface IVehicleRepository
    {
        //Vehicle
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle?> GetVehicleByIdAsync(int id);
        Task AddVehicleAsync(Vehicle vehicle);
        Task UpdateVehicleAsync(Vehicle vehicle);
        Task DeleteVehicleAsync(Vehicle vehicle);

        //Vehicle Type
        Task<IEnumerable<VehicleType>> GetAllTypesAsync();
        Task <VehicleType> GetByIdAsync(int id);
        Task AddVehicleTypeAsync (VehicleType type);
        Task DeleteVehicleTypeAsync(VehicleType type);
        Task<bool> TypeExistsAsync(string typeName);

        // Vehicle Brand
        Task<IEnumerable<VehicleBrand>> GetAllBrandsAsync();
        Task<VehicleBrand> GetBrandByIdAsync(int id);
        Task<IEnumerable<VehicleBrand>> GetBrandsByTypeAsync(int typeId);
        Task AddVehicleBrandAsync(VehicleBrand brand);
        Task DeleteVehicleBrandAsync(VehicleBrand brand);
        Task<bool> BrandExistsAsync(string name);

        // Vehicle Model
        Task<IEnumerable<VehicleModel>> GetAllVehicleModelsAsync();
        Task<IEnumerable<VehicleModel>> GetModelsByBrandAndTypeAsync(int brandId, int typeId);
        Task<VehicleModel?> GetVehicleModelByIdAsync(int id);
        Task AddVehicleModelAsync(VehicleModel model);
        Task DeleteVehicleModelAsync(VehicleModel brand);
        Task<bool> VehicleModelExistsAsync(string name, int brandId);

        Task SaveChangesAsync();
    }

}
