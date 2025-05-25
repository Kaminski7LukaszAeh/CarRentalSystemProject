namespace CarRentalSystem.Presentation.Models
{
    public class ManageVehicleTypesViewModel
    {
        public List<VehicleTypeViewModel> VehicleTypes { get; set; } = [];
        public CreateVehicleTypeViewModel NewVehicleType { get; set; } = new();
    }
}
