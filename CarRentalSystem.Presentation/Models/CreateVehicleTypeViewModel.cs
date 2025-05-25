using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Presentation.Models
{
    public class CreateVehicleTypeViewModel
    {
        [Required]
        [Display (Name = "Typ")]
        public string TypeName { get; set; }
    }
}
