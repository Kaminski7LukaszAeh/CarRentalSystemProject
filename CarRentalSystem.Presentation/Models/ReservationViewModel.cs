using CarRentalSystem.DataAccess.Entities.Enums;

namespace CarRentalSystem.Presentation.Models
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public int VehicleId { get; set; }
        public decimal DailyRate { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string VehicleTypeName { get; set; }
        public DateTime CreatedAt { get; internal set; }
        public ReservationStatus Status { get; internal set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string UserId { get; set; }
        public bool IsReturned { get; set; }
    }
}
