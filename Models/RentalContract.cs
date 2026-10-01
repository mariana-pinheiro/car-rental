using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public class RentalContract : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O cliente é obrigatório.")]
    public int ClientId { get; set; }

    public Client? Client { get; set; }

    [Required(ErrorMessage = "O veículo é obrigatório.")]
    public int VehicleId { get; set; }

    public Vehicle? Vehicle { get; set; }

    [Required(ErrorMessage = "A data de início é obrigatória.")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "A data de fim é obrigatória.")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "A quilometragem inicial é obrigatória.")]
    [Range(0, int.MaxValue,
        ErrorMessage = "A quilometragem inicial não pode ser negativa.")]
    public int InitialMileage { get; set; }

    [Range(0, int.MaxValue,
        ErrorMessage = "A quilometragem final não pode ser negativa.")]
    public int? FinalMileage { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {


        if (EndDate <= StartDate)
        {
            yield return new ValidationResult(
                "A data de fim deve ser posterior à data de início.",
                new[] { nameof(EndDate) }
            );
        }

        if (FinalMileage.HasValue &&
            FinalMileage.Value < InitialMileage)
        {
            yield return new ValidationResult(
                "A quilometragem final não pode ser inferior à quilometragem inicial.",
                new[] { nameof(FinalMileage) }
            );
        }
    }
}