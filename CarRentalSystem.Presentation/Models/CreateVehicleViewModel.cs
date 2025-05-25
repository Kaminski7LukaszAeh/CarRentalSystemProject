using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Presentation.Models
{
    public class CreateVehicleViewModel
    {
        [Display(Name = "Typ")]
        [Required]
        public int VehicleTypeId { get; set; }

        [Display(Name = "Marka")]
        [Required]
        public int VehicleBrandId { get; set; }

        [Display(Name = "Model")]
        [Required]
        public int VehicleModelId { get; set; }

        [Display(Name = "Dzienna stawka")]
        [Range(0, 100000)]
        [Required]
        public decimal DailyRate { get; set; }

        public List<SelectListItem> AvailableTypes { get; set; } = new();
        public List<SelectListItem> AvailableBrands { get; set; } = new();
        public List<SelectListItem> AvailableModels { get; set; } = new();
    }
}
