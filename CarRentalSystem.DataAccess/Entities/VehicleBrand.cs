namespace CarRentalSystem.DataAccess.Entities
{
    public class VehicleBrand
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<VehicleModel> Models { get; set; } = [];
    }
}
