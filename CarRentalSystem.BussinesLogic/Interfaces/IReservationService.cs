using CarRentalSystem.BusinessLogic.Common;
using CarRentalSystem.BusinessLogic.DataTransferObjects;

namespace CarRentalSystem.BusinessLogic.Interfaces
{
    public interface IReservationService
    {
        Task<Result<IEnumerable<ReservationDto>>> GetReservationsByUserAsync(string userId);
        Task<Result<IEnumerable<ReservationDto>>> GetReservationsByVehicleAsync(int vehicleId);
        Task<Result> CreateReservationAsync(CreateReservationDto dto);
        decimal CalculateTotalCost(DateTime startDate, DateTime endDate, decimal dailyRate);
    }
}
 