namespace CarRentalSystem.BusinessLogic.DataTransferObjects
{
    public class VehicleModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int VehicleTypeId { get; set; }
        public string VehicleTypeName { get; set; }
    }
}
