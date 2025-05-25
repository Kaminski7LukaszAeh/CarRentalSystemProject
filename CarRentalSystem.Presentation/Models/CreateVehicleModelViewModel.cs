using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Presentation.Models
{
    public class CreateVehicleModelViewModel
    {
        [Required]
        [Display(Name = "Model")]
        public string Name { get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required]
        public int VehicleTypeId { get; set; }
        [ValidateNever]
        public List<SelectListItem> AvailableBrands { get; set; }
        [ValidateNever]
        public List<SelectListItem> AvailableVehicleTypes { get; set; } = [];
    }
}
