using CarRentalSystem.DataAccess.Entities.Enums;

namespace CarRentalSystem.BusinessLogic.DataTransferObjects
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public ReservationStatus Status { get; set; }
        public decimal TotalCost { get; set; }
        public decimal DailyRate { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string VehicleTypeName { get; set; }
    }
}
