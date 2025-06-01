using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.DataAccess.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment> GetByReservationIdAsync(int reservationId)
        {
            return await _context.Payments
                 .FirstAsync(p => p.ReservationId == reservationId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
