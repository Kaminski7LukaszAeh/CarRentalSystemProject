using CarRentalSystem.BusinessLogic.Common;
using CarRentalSystem.BusinessLogic.DataTransferObjects;
using CarRentalSystem.BusinessLogic.Interfaces;
using CarRentalSystem.BusinessLogic.Mapping;
using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.DataAccess.Entities.Enums;
using CarRentalSystem.DataAccess.Interfaces;
using CarRentalSystem.DataAccess.Repositories;
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

        public async Task<Result<Reservation>> GetReservationByIdAsync(int id)
        {
            try
            {
                var reservation = await _reservationRepository.GetByIdAsync(id);
                if (reservation == null)
                    return Result<Reservation>.Failure("Reservation not found.");
                return Result<Reservation>.Success(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve reservation {id}.");
                return Result<Reservation>.Failure("An error occurred while retrieving reservation.");
            }
        }

        public async Task<Result<IEnumerable<ReservationDto>>> GetReservationsByUserAsync(string userId)
        {
            try
            {
                var reservations = await _reservationRepository.GetReservationsByUserIdAsync(userId);

                await UpdateOverdueReservationsAsync(reservations);
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

                await UpdateOverdueReservationsAsync(reservations);

                var dtos = EntityToDtoMapper.MapList(reservations);
                return Result<IEnumerable<ReservationDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve reservations for vehicle {vehicleId}.");
                return Result<IEnumerable<ReservationDto>>.Failure("Failed to retrieve reservations.");
            }
        }

        public async Task<Result<IEnumerable<ReservationDto>>> GetHistoricalReservations(string userId)
        {
            try
            {
                var reservations = await _reservationRepository.GetHistoricalReservations(userId);
                var dtos = EntityToDtoMapper.MapList(reservations);
                return Result<IEnumerable<ReservationDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve historical reservations for user {userId}.");
                return Result<IEnumerable<ReservationDto>>.Failure("Failed to retrieve historical reservations.");
            }
        }

        public async Task<Result<IEnumerable<ReservationDto>>> GetActiveReservationsByUserAsync(string userId)
        {
            try
            {
                var reservations = await _reservationRepository.GetActiveReservationsByUserIdAsync(userId);
                var dtos = EntityToDtoMapper.MapList(reservations);
                return Result<IEnumerable<ReservationDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve active reservations for user {userId}.");
                return Result<IEnumerable<ReservationDto>>.Failure("Failed to retrieve active reservations.");
            }
        }




        public async Task<Result<Reservation>> CreateReservationAsync(CreateReservationDto dto)
        {
            try
            {
                if (dto.EndDate < dto.StartDate)
                    return Result<Reservation>.Failure("End date must be after start date.");

                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(dto.VehicleId);
                if (vehicle == null)
                    return Result<Reservation>.Failure("Vehicle not found.");

                var isAvailable = await _reservationRepository.IsVehicleAvailable(dto.VehicleId, dto.StartDate, dto.EndDate);
                if (!isAvailable)
                    return Result<Reservation>.Failure("Vehicle is not available for the selected dates.");

                dto.TotalCost = CalculateTotalCost(dto.StartDate, dto.EndDate, vehicle.DailyRate);
                dto.StartDate = dto.StartDate.ToUniversalTime();
                dto.EndDate = dto.EndDate.ToUniversalTime();

                var reservation = DtoToEntityMapper.Map(dto);
                await _reservationRepository.AddReservationAsync(reservation);
                await _reservationRepository.SaveChangesAsync();

                return Result<Reservation>.Success(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create reservation.");
                return Result<Reservation>.Failure("An unexpected error occurred.");
            }
        }

        public async Task<Result> CancelReservationAsync(int reservationId, string userId)
        {
            try
            {
                var reservation = await _reservationRepository.GetByIdAsync(reservationId);
                if (reservation == null || reservation.UserId != userId)
                {
                    return Result.Failure("Reservation not found.");
                }

                if (reservation.Status == ReservationStatus.Cancelled)
                {
                    return Result.Failure("Reservation is already canceled.");
                }

                reservation.Status = ReservationStatus.Cancelled;
                await _reservationRepository.SaveChangesAsync();

                return Result.Success("Reservation canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel reservation.");
                return Result.Failure("An error occurred while canceling the reservation.");
            }
        }
        public async Task<Result> UpdateReservationStatusAsync(int reservationId, ReservationStatus status)
        {
            try
            {
                var reservation = await _reservationRepository.GetByIdAsync(reservationId);
                if (reservation == null)
                    return Result.Failure("Reservation not found.");

                reservation.Status = status;
                await _reservationRepository.SaveChangesAsync();

                return Result.Success("Reservation status updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update status for reservation {reservationId}.");
                return Result.Failure("Failed to update reservation status.");
            }
        }


        public async Task<Result<IEnumerable<DateTime>>> GetReservedDatesAsync(int vehicleId)
        {
            var result = await GetReservationsByVehicleAsync(vehicleId);

            if (!result.IsSuccess || result.Value == null)
                return Result<IEnumerable<DateTime>>.Failure(result.Error ?? "No reservations found.");

            var activeReservations = result.Value
                .Where(r => r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.Completed);

            var dates = activeReservations
                .SelectMany(r => Enumerable.Range(0, (r.EndDate.Date - r.StartDate.Date).Days + 1)
                    .Select(d => r.StartDate.Date.AddDays(d)))
                .Distinct()
                .ToList();

            return Result<IEnumerable<DateTime>>.Success(dates);
        }

        public decimal CalculateTotalCost(DateTime startDate, DateTime endDate, decimal dailyRate)
        {
            int days = Math.Max((endDate - startDate).Days, 1);
            return days * dailyRate;
        }

        private async Task UpdateOverdueReservationsAsync(IEnumerable<Reservation> reservations)
        {
            foreach (var reservation in reservations)
            {
                if (reservation.Payment == null || reservation.Payment.Status != PaymentStatus.Completed)
                {
                    if (DateTime.UtcNow.Date > reservation.StartDate.Date)
                    {
                        var overdueDays = (DateTime.UtcNow.Date - reservation.StartDate.Date).Days;

                        if (overdueDays > 0)
                        {
                            reservation.Status = ReservationStatus.Overdue;

                            var dailyRate = reservation.Vehicle?.DailyRate ?? 0m;

                            var additionalCost = overdueDays * dailyRate * 0.5m;

                            reservation.TotalCost += additionalCost;

                            await _reservationRepository.UpdateAsync(reservation);
                        }
                    }
                }
            }
        }


    }
}
