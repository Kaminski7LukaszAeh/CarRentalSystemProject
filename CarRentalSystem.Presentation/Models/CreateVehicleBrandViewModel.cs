using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Presentation.Models
{
    public class CreateVehicleBrandViewModel
    {
        [Required]
        [Display(Name = "Marka")]
        public string Name { get; set; }
    }
}

