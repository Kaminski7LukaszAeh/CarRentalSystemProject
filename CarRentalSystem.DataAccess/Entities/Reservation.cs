using CarRentalSystem.DataAccess.Entities.Enums;
using System;

namespace CarRentalSystem.DataAccess.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalCost { get; set; }
        public ReservationStatus Status { get; set; }
        public Payment Payment { get; set; }
        public bool IsReturned { get; set; } = false;
    }
}
