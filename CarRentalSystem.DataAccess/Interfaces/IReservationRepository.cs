using CarRentalSystem.DataAccess.Entities;

namespace CarRentalSystem.DataAccess.Interfaces
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetReservationsByUserIdAsync(string userId);
        Task<IEnumerable<Reservation>> GetReservationsByVehicleIdAsync(int vehicleId);
        Task AddReservationAsync(Reservation reservation);
        Task<bool> IsVehicleAvailable(int vehicleId, DateTime startDate, DateTime endDate);
        Task SaveChangesAsync();
    }
}
