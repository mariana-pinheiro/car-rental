using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public class Vehicle : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A marca é obrigatória.")]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "O modelo é obrigatório.")]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "A matrícula é obrigatória.")]
    public string LicensePlate { get; set; } = string.Empty;

    [Required(ErrorMessage = "O ano de fabrico é obrigatório.")]
    [Range(1900, int.MaxValue, ErrorMessage = "Introduza um ano de fabrico válido.")]
    public int ManufacturingYear { get; set; }

    [Required(ErrorMessage = "O tipo de combustível é obrigatório.")]
    public string FuelType { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ManufacturingYear > DateTime.Now.Year)
        {
            yield return new ValidationResult(
                "O ano de fabrico não pode ser posterior ao ano atual.",
                new[] { nameof(ManufacturingYear) }
            );
        }
    }
}