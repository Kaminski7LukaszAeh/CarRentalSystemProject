namespace CarRentalSystem.BusinessLogic.DataTransferObjects
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public decimal DailyRate { get; set; }
        public bool IsAvailable { get; set; }

        public int VehicleModelId { get; set; }
        public string VehicleModelName { get; set; }

        public int VehicleTypeId { get; set; }
        public string VehicleTypeName { get; set; }

        public int VehicleBrandId { get; set; }
        public string VehicleBrandName { get; set; }
    }
}
