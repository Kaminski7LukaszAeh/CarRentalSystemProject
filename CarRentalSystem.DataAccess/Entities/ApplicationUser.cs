using Microsoft.AspNetCore.Identity;

namespace CarRentalSystem.DataAccess.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}
