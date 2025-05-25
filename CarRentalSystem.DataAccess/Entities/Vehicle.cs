namespace CarRentalSystem.DataAccess.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public decimal DailyRate { get; set; }
        public bool IsAvailable { get; set; }
        public int VehicleModelId { get; set; }
        public VehicleModel VehicleModel { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}
