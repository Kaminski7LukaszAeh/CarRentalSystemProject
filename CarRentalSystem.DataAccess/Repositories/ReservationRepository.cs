using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.DataAccess.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.DataAccess.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Reservation>> GetReservationsByUserIdAsync(string userId)
        {
            return await _context.Reservations
                .Include(r => r.Vehicle)
                    .ThenInclude(vm => vm.VehicleModel)
                        .ThenInclude(m => m.Brand)
                .Include(r => r.Vehicle)
                    .ThenInclude(vm => vm.VehicleModel)
                        .ThenInclude(m => m.VehicleType)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Reservation>> GetReservationsByVehicleIdAsync(int vehicleId)
        {
            return await _context.Reservations
                .Where(r => r.VehicleId == vehicleId)
                .ToListAsync();
        }
        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsVehicleAvailable(int vehicleId, DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToUniversalTime();
            endDate = endDate.ToUniversalTime();

            return !await _context.Reservations
                .AnyAsync(r =>
                    r.VehicleId == vehicleId &&
                    r.StartDate < endDate &&
                    r.EndDate > startDate
                );
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
