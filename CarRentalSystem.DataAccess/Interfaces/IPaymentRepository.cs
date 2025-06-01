using CarRentalSystem.DataAccess.Entities;

namespace CarRentalSystem.DataAccess.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment> GetByReservationIdAsync(int reservationId);
        Task SaveChangesAsync();
    }
}
