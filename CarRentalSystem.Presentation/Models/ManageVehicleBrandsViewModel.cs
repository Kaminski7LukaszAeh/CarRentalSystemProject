namespace CarRentalSystem.Presentation.Models
{
    public class ManageVehicleBrandsViewModel
    {
        public CreateVehicleBrandViewModel NewVehicleBrand { get; set; } = new();
        public List<VehicleBrandViewModel> VehicleBrands { get; set; } = new();
    }
}
