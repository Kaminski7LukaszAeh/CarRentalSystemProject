using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Presentation.Models
{
    public class VehicleViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Dzienna Stawka")]
        public decimal DailyRate { get; set; }
        public bool IsAvailable { get; set; }
        [Display(Name = "Dostępność")]
        public string BrandName { get; set; } = string.Empty;
        [Display(Name = "Marka")]
        public string ModelName { get; set; } = string.Empty;
        [Display(Name = "Typ")]
        public string TypeName { get; set; } = string.Empty;

    }
}
