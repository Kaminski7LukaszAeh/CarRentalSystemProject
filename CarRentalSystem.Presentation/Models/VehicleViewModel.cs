namespace CarRentalSystem.Presentation.Models
{
    public class VehicleViewModel
    {
        public int Id { get; set; }
        public decimal DailyRate { get; set; }
        public bool IsAvailable { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;

    }
}
