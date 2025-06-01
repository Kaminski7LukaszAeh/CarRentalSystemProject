using System.ComponentModel.DataAnnotations;

public class VehicleFilter : IValidatableObject
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public List<int> SelectedVehicleTypeIds { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && StartDate > EndDate)
        {
            yield return new ValidationResult(
                "Start date must be before or equal to end date.",
                [nameof(StartDate), nameof(EndDate)]);
        }

        if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
        {
            yield return new ValidationResult(
                "Minimum price must be less than or equal to maximum price.",
                [nameof(MinPrice), nameof(MaxPrice)]);
        }
    }
}
