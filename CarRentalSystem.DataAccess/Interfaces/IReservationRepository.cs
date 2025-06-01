using CarRentalSystem.DataAccess.Entities;

namespace CarRentalSystem.DataAccess.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(int reservationId);
        Task<List<Reservation>> GetReservationsByUserIdAsync(string userId);
        Task<IEnumerable<Reservation>> GetReservationsByVehicleIdAsync(int vehicleId);
        Task<IEnumerable<Reservation>> GetHistoricalReservations(string userId);
        Task<IEnumerable<Reservation>> GetActiveReservationsByUserIdAsync(string userId);
        Task AddReservationAsync(Reservation reservation);
        Task<bool> IsVehicleAvailable(int vehicleId, DateTime startDate, DateTime endDate);
        Task SaveChangesAsync();
        Task UpdateAsync(Reservation reservation);
    }
}
