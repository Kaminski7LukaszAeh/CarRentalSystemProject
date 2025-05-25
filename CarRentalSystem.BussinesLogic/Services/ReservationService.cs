using CarRentalSystem.BusinessLogic.Common;
using CarRentalSystem.BusinessLogic.DataTransferObjects;
using CarRentalSystem.BusinessLogic.Interfaces;
using CarRentalSystem.BusinessLogic.Mapping;
using CarRentalSystem.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace CarRentalSystem.BusinessLogic.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(IReservationRepository reservationRepository, IVehicleRepository vehicleRepository, ILogger<ReservationService> logger)
        {
            _reservationRepository = reservationRepository;
            _vehicleRepository = vehicleRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<ReservationDto>>> GetReservationsByUserAsync(string userId)
        {
            try
            {
                var reservations = await _reservationRepository.GetReservationsByUserIdAsync(userId);
                var dtos = EntityToDtoMapper.MapList(reservations);
                return Result<IEnumerable<ReservationDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve reservations for user {userId}.");
                return Result<IEnumerable<ReservationDto>>.Failure("Failed to retrieve reservations.");
            }
        }

        public async Task<Result<IEnumerable<ReservationDto>>> GetReservationsByVehicleAsync(int vehicleId)
        {
            try
            {
                var reservations = await _reservationRepository.GetReservationsByVehicleIdAsync(vehicleId);
                var dtos = EntityToDtoMapper.MapList(reservations);
                return Result<IEnumerable<ReservationDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve reservations for vehicle {vehicleId}.");
                return Result<IEnumerable<ReservationDto>>.Failure("Failed to retrieve reservations.");
            }
        }


        public async Task<Result> CreateReservationAsync(CreateReservationDto dto)
        {
            try
            {
                if (dto.EndDate < dto.StartDate)
                {
                    return Result.Failure("End date must be after start date.");
                }

                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(dto.VehicleId);
                if (vehicle == null)
                {
                    return Result.Failure("Vehicle not found.");
                }

                var isAvailable = await _reservationRepository.IsVehicleAvailable(dto.VehicleId, dto.StartDate, dto.EndDate);
                if (!isAvailable)
                {
                    return Result.Failure("Vehicle is not available for the selected dates.");
                }

                dto.TotalCost = CalculateTotalCost(dto.StartDate, dto.EndDate, vehicle.DailyRate);
                dto.StartDate = dto.StartDate.ToUniversalTime();
                dto.EndDate = dto.EndDate.ToUniversalTime();

                var reservation = DtoToEntityMapper.Map(dto);

                await _reservationRepository.AddReservationAsync(reservation);
                await _reservationRepository.SaveChangesAsync();

                return Result.Success("Reservation created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create reservation.");
                return Result.Failure("An unexpected error occurred.");
            }
        }

        public decimal CalculateTotalCost(DateTime startDate, DateTime endDate, decimal dailyRate)
        {
            int days = Math.Max((endDate - startDate).Days, 1);
            return days * dailyRate;
        }
    }
}
