namespace CarRentalSystem.BusinessLogic.DataTransferObjects
{
    public class CreateVehicleModelDto
    {
        public string Name { get; set; }
        public int BrandId { get; set; }
        public int VehicleTypeId { get; set; }
    }
}
