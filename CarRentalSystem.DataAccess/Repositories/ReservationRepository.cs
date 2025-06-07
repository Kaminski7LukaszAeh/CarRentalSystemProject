using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.DataAccess.Entities.Enums;
using CarRentalSystem.DataAccess.Interfaces;
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

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            return await _context.Reservations
                                 .Include(r => r.Vehicle)
                                    .ThenInclude(vm => vm.VehicleModel)
                                        .ThenInclude(m => m.Brand)
                                .Include(r => r.Vehicle)
                                    .ThenInclude(vm => vm.VehicleModel)
                                        .ThenInclude(m => m.VehicleType)
                                 .Include(r => r.User)
                                 .Include(r => r.Payment)
                                 .ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int reservationId)
        {
            return await _context.Reservations
                .Include(r => r.Vehicle)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.Id == reservationId);
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
                .Include(r => r.Payment)
                .ToListAsync();
        }
        public async Task<IEnumerable<Reservation>> GetActiveReservationsByUserIdAsync(string userId)
        {
            return await _context.Reservations
                .Include(r => r.Vehicle)
                    .ThenInclude(vm => vm.VehicleModel)
                        .ThenInclude(m => m.Brand)
                .Include(r => r.Vehicle)
                    .ThenInclude(vm => vm.VehicleModel)
                        .ThenInclude(m => m.VehicleType)
                .Include(r => r.Payment)
                .Where(r => r.UserId == userId &&
                            r.Status != ReservationStatus.Cancelled &&
                            (r.Status != ReservationStatus.Completed))
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetHistoricalReservations(string userId)
        {
            return await _context.Reservations
                .Include(r => r.Vehicle)
                    .ThenInclude(vm => vm.VehicleModel)
                        .ThenInclude(m => m.Brand)
                .Include(r => r.Vehicle)
                    .ThenInclude(vm => vm.VehicleModel)
                        .ThenInclude(m => m.VehicleType)
                .Where(r => r.UserId == userId &&
                            (r.Status == ReservationStatus.Cancelled || r.Status == ReservationStatus.Completed))
                .ToListAsync();
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            reservation.StartDate = reservation.StartDate.AddHours(2);
            reservation.EndDate = reservation.EndDate.AddHours(2);

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
                    r.Status != ReservationStatus.Cancelled && 
                    r.Status != ReservationStatus.Completed &&
                    r.StartDate < endDate &&
                    r.EndDate > startDate
                );
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
