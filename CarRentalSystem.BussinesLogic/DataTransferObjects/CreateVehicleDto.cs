namespace CarRentalSystem.BusinessLogic.DataTransferObjects
{
    public class CreateVehicleDto
    {
        public decimal DailyRate { get; set; }
        public int VehicleModelId { get; set; }
        public int VehicleTypeId { get; set; }
    }
}
