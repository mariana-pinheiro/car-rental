using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public class RentalContract
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
    [Range(0, int.MaxValue, ErrorMessage = "A quilometragem não pode ser negativa.")]
    public int InitialMileage { get; set; }

    [Required(ErrorMessage = "A quilometragem final é obrigatória.")]
    [Range(0, int.MaxValue, ErrorMessage = "A quilometragem não pode ser negativa.")]
    public int FinalMileage { get; set; }
}