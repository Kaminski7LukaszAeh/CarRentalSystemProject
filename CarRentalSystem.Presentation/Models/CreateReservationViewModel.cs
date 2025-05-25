using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Presentation.Models
{
    public class CreateReservationViewModel
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data wypożyczenia")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data zwrotu")]
        public DateTime EndDate { get; set; }

        [ValidateNever]
        [BindNever]
        public decimal TotalCost { get; set; }

        public decimal DailyRate { get; set; }
    }
}
