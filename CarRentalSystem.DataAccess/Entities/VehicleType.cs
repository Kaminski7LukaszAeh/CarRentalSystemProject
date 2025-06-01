namespace CarRentalSystem.DataAccess.Entities
{
    public class VehicleType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }

        public ICollection<VehicleModel> VehicleModels { get; set; } = [];
    }
}
