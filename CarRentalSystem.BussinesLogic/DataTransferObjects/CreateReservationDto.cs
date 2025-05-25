namespace CarRentalSystem.BusinessLogic.DataTransferObjects
{
    public class CreateReservationDto
    {
        public int VehicleId { get; set; }
        public string UserId { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
    }
}
