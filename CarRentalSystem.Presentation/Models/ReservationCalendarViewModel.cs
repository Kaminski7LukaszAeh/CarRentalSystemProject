namespace CarRentalSystem.Presentation.Models
{
    public class ReservationCalendarViewModel
    {
        public int VehicleId { get; set; }
        public decimal DailyRate { get; set; }
        public List<DateTime> ReservedDates { get; set; } = new();
    }
}
