using CarRentalSystem.BusinessLogic.Common;
using CarRentalSystem.BusinessLogic.DataTransferObjects;

namespace CarRentalSystem.BusinessLogic.Interfaces
{
    public interface IVehicleService
    {
        Task<Result<IEnumerable<VehicleModelDto>>> GetAllVehicleModelsAsync();
        Task<Result<IEnumerable<VehicleModelDto>>> GetModelsByBrandAndTypeAsync(int brandId, int typeId);
        Task<Result> AddVehicleModelAsync(CreateVehicleModelDto dto);
        Task<Result> UpdateVehicleAsync(VehicleDto dto);
        Task<Result> DeleteVehicleModelAsync(int id);

        Task<Result<IEnumerable<VehicleDto>>> GetAllVehiclesAsync();
        Task<Result> AddVehicleAsync(CreateVehicleDto dto);
        Task<Result> DeleteVehicleAsync(int id);

        Task<Result<IEnumerable<VehicleTypeDto>>> GetAllVehicleTypes();
        Task<Result> AddVehicleTypeAsync(CreateVehicleTypeDto dto);
        Task<Result> DeleteVehicleTypeAsync(int id);

        Task<Result<IEnumerable<VehicleBrandDto>>> GetAllBrandsAsync();
        Task<Result<IEnumerable<VehicleBrandDto>>> GetBrandsByTypeAsync(int typeId);
        Task<Result> AddVehicleBrandAsync(CreateVehicleBrandDto dto);
        Task<Result> DeleteVehicleBrandAsync(int id);
    }
}
