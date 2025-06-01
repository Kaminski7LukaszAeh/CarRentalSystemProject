using CarRentalSystem.BusinessLogic.Common;
using CarRentalSystem.BusinessLogic.DataTransferObjects;
using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.DataAccess.Entities.Enums;

namespace CarRentalSystem.BusinessLogic.Interfaces
{
    public interface IReservationService
    {
        Task<Result<Reservation>> GetReservationByIdAsync(int id);
        Task<Result<IEnumerable<ReservationDto>>> GetReservationsByUserAsync(string userId);
        Task<Result<IEnumerable<ReservationDto>>> GetReservationsByVehicleAsync(int vehicleId);
        Task<Result<IEnumerable<ReservationDto>>> GetHistoricalReservations(string userId);
        Task<Result<IEnumerable<ReservationDto>>> GetActiveReservationsByUserAsync(string userId);
        Task<Result<IEnumerable<DateTime>>> GetReservedDatesAsync(int vehicleId);
        Task<Result<Reservation>> CreateReservationAsync(CreateReservationDto dto);
        Task<Result> CancelReservationAsync(int reservationId, string userId);
        Task<Result> UpdateReservationStatusAsync(int reservationId, ReservationStatus status);
        decimal CalculateTotalCost(DateTime startDate, DateTime endDate, decimal dailyRate);
        
    }
}
 