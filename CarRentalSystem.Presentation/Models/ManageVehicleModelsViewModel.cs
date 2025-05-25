namespace CarRentalSystem.Presentation.Models
{
    public class ManageVehicleModelsViewModel
    {
        public CreateVehicleModelViewModel NewVehicleModel { get; set; }
        public List<VehicleModelViewModel> VehicleModels { get; set; } = [];
    }
}
