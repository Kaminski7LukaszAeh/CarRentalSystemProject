namespace CarRentalSystem.Presentation.Models
{
    public class ManageVehiclesViewModel
    {
        public CreateVehicleViewModel NewVehicle { get; set; }
        public List<VehicleViewModel> Vehicles { get; set; } = [];
    }
}
