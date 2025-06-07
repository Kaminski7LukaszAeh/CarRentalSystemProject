using CarRentalSystem.BusinessLogic.Common;
using CarRentalSystem.BusinessLogic.DataTransferObjects;
using CarRentalSystem.BusinessLogic.Interfaces;
using CarRentalSystem.BusinessLogic.Mapping;
using CarRentalSystem.DataAccess.Entities.Enums;
using CarRentalSystem.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace CarRentalSystem.BusinessLogic.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(IVehicleRepository repository, ILogger<VehicleService> logger)
        {
            _vehicleRepository = repository;
            _logger = logger;
        }

        public async Task<Result> AddVehicleAsync(CreateVehicleDto dto)
        {
            try
            {
                var vehicle = DtoToEntityMapper.Map(dto);
                await _vehicleRepository.AddVehicleAsync(vehicle);
                return Result.Success("Vehicle added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add vehicle.");
                return Result.Failure("An unexpected error occurred.");
            }
        }

        public async Task<Result<IEnumerable<VehicleDto>>> GetAllVehiclesAsync()
        {
            try
            {
                var vehicles = await _vehicleRepository.GetAllVehiclesAsync();
                var dtos = EntityToDtoMapper.MapList(vehicles);
                return Result<IEnumerable<VehicleDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve vehicles.");
                return Result<IEnumerable<VehicleDto>>.Failure("Failed to retrieve vehicles.");
            }
        }
        public async Task<Result<IEnumerable<VehicleDto>>> GetFilteredVehiclesAsync(VehicleFilterDto filterDto)
        {
            try
            {
                var dalFilter = DtoToEntityMapper.Map(filterDto);
                var vehicles = await _vehicleRepository.GetFilteredVehiclesAsync(dalFilter);
                var dtos = EntityToDtoMapper.MapList(vehicles);
                return Result<IEnumerable<VehicleDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve filtered vehicles.");
                return Result<IEnumerable<VehicleDto>>.Failure("Failed to retrieve vehicles.");
            }
        }

        public async Task<Result<VehicleDto>> GetVehicleByIdAsync(int vehicleId)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(vehicleId);
                if (vehicle == null)
                    return Result<VehicleDto>.Failure("Vehicle not found.");

                var dto = EntityToDtoMapper.Map(vehicle);
                return Result<VehicleDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get vehicle by ID.");
                return Result<VehicleDto>.Failure("An error occurred retrieving the vehicle.");
            }
        }

        public async Task<Result> UpdateVehicleAsync(VehicleDto dto)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(dto.Id);
                if (vehicle == null)
                    return Result.Failure("Vehicle not found.");

                vehicle.DailyRate = dto.DailyRate;
                vehicle.IsAvailable = dto.IsAvailable;

                await _vehicleRepository.UpdateVehicleAsync(vehicle);
                return Result.Success("Vehicle updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update vehicle.");
                return Result.Failure("An unexpected error occurred.");
            }
        }

        public async Task<Result> DeleteVehicleAsync(int id)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id);
                if (vehicle == null)
                    return Result.Failure("Vehicle not found.");

                var hasPendingReservations = vehicle.Reservations.Any(r =>
                    r.Status != ReservationStatus.Cancelled &&
                    r.Status != ReservationStatus.Completed &&
                    r.Status != ReservationStatus.Active &&
                    r.EndDate >= DateTime.UtcNow);

                if (hasPendingReservations)
                {
                    return Result.Failure("Vehicle cannot be deleted because it is associated with active or upcoming reservations.");
                }

                await _vehicleRepository.DeleteVehicleAsync(vehicle);
                return Result.Success("Vehicle deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete vehicle.");
                return Result.Failure("An unexpected error occurred.");
            }
        }


        public async Task<Result> AddVehicleTypeAsync(CreateVehicleTypeDto dto)
        {
            try
            {
                if (await _vehicleRepository.TypeExistsAsync(dto.TypeName))
                    return Result.Failure("Vehicle type already exists.");

                var entity = DtoToEntityMapper.Map(dto);
                await _vehicleRepository.AddVehicleTypeAsync(entity);
                await _vehicleRepository.SaveChangesAsync();

                return Result.Success("Vehicle type added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add vehicle type.");
                return Result.Failure("An unexpected error occurred.");
            }
        }
        public async Task<Result<IEnumerable<VehicleTypeDto>>> GetAllVehicleTypes()
        {
            try
            {
                var types = await _vehicleRepository.GetAllTypesAsync();
                var dtos = EntityToDtoMapper.MapList(types);
                return Result<IEnumerable<VehicleTypeDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve vehicle types.");
                return Result<IEnumerable<VehicleTypeDto>>.Failure("Failed to retrieve vehicle types.");
            }
        }
        public async Task<Result> DeleteVehicleTypeAsync(int id)
        {
            try
            {
                var type = await _vehicleRepository.GetTypeByIdAsync(id);
                if (type == null)
                {
                    return Result.Failure("Vehicle type not found.");
                }

                if (type.VehicleModels != null && type.VehicleModels.Any())
                {
                    return Result.Failure("Cannot delete vehicle type because it has associated vehicle models.");
                }

                await _vehicleRepository.DeleteVehicleTypeAsync(type);
                await _vehicleRepository.SaveChangesAsync();

                return Result.Success("Vehicle type deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete vehicle type.");
                return Result.Failure("An unexpected error occurred.");
            }
        }

        public async Task<Result> AddVehicleBrandAsync(CreateVehicleBrandDto dto)
        {
            try
            {
                if (await _vehicleRepository.BrandExistsAsync(dto.Name))
                {
                    return Result.Failure("Vehicle brand already exists.");
                }

                var entity = DtoToEntityMapper.Map(dto);
                await _vehicleRepository.AddVehicleBrandAsync(entity);
                await _vehicleRepository.SaveChangesAsync();

                return Result.Success("Vehicle brand added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add vehicle brand.");
                return Result.Failure("An unexpected error occurred.");
            }
        }

        public async Task<Result<IEnumerable<VehicleBrandDto>>> GetAllBrandsAsync()
        {
            try
            {
                var brands = await _vehicleRepository.GetAllBrandsAsync();
                var dtos = EntityToDtoMapper.MapList(brands);
                return Result<IEnumerable<VehicleBrandDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve vehicle brands.");
                return Result<IEnumerable<VehicleBrandDto>>.Failure("Failed to retrieve vehicle brands.");
            }
        }

        public async Task<Result> DeleteVehicleBrandAsync(int id)
        {
            try
            {
                var brand = await _vehicleRepository.GetBrandByIdAsync(id);
                if (brand == null)
                {
                    return Result.Failure("Vehicle brand not found.");
                }

                if (brand.Models != null && brand.Models.Any())
                {
                    return Result.Failure("Cannot delete vehicle brand because it has associated vehicle models.");
                }

                await _vehicleRepository.DeleteVehicleBrandAsync(brand);
                await _vehicleRepository.SaveChangesAsync();

                return Result.Success("Vehicle brand deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete vehicle brand.");
                return Result.Failure("An unexpected error occurred.");
            }
        }

        public async Task<Result> AddVehicleModelAsync(CreateVehicleModelDto dto)
        {
            try
            {
                var brand = await _vehicleRepository.GetBrandByIdAsync(dto.BrandId);
                if (brand == null)
                {
                    return Result.Failure("Selected brand does not exist.");
                }

                if (await _vehicleRepository.VehicleModelExistsAsync(dto.Name, dto.BrandId))
                {
                    return Result.Failure("Vehicle model already exists.");
                }

                var model = DtoToEntityMapper.Map(dto);

                await _vehicleRepository.AddVehicleModelAsync(model);
                await _vehicleRepository.SaveChangesAsync();

                return Result.Success("Vehicle model added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add vehicle model.");
                return Result.Failure("An unexpected error occurred.");
            }
        }

        public async Task<Result<IEnumerable<VehicleModelDto>>> GetAllVehicleModelsAsync()
        {
            try
            {
                var models = await _vehicleRepository.GetAllVehicleModelsAsync();
                var dtos = EntityToDtoMapper.MapList(models);
                return Result<IEnumerable<VehicleModelDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve vehicle models.");
                return Result<IEnumerable<VehicleModelDto>>.Failure("Failed to retrieve vehicle models.");
            }
        }
        public async Task<Result> DeleteVehicleModelAsync(int id)
        {
            try
            {
                var model = await _vehicleRepository.GetVehicleModelByIdAsync(id);
                if (model == null)
                {
                    return Result.Failure("Vehicle model not found.");
                }

                if (model.Vehicles != null && model.Vehicles.Any())
                {
                    return Result.Failure("Cannot delete vehicle model because it has associated vehicles.");
                }

                await _vehicleRepository.DeleteVehicleModelAsync(model);
                await _vehicleRepository.SaveChangesAsync();

                return Result.Success("Vehicle model deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete vehicle model.");
                return Result.Failure("An unexpected error occurred.");
            }
        }

        public async Task<Result<IEnumerable<VehicleBrandDto>>> GetBrandsByTypeAsync(int typeId)
        {
            var brands = await _vehicleRepository.GetBrandsByTypeAsync(typeId);
            var dtos = EntityToDtoMapper.MapList(brands);
            return Result<IEnumerable<VehicleBrandDto>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<VehicleModelDto>>> GetModelsByBrandAndTypeAsync(int brandId, int typeId)
        {
            var models = await _vehicleRepository.GetModelsByBrandAndTypeAsync(brandId, typeId);
            var dtos = EntityToDtoMapper.MapList(models);
            return Result<IEnumerable<VehicleModelDto>>.Success(dtos);
        }

    }
}


