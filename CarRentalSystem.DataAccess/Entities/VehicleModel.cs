namespace CarRentalSystem.DataAccess.Entities
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public VehicleBrand Brand { get; set; }
        public int VehicleTypeId { get; set; }
        public VehicleType VehicleType { get; set; }
    }
}
