using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public class Client
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Introduza um email válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "O telefone deve conter exatamente 9 números.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "A carta de condução é obrigatória.")]
    public string DrivingLicense { get; set; } = string.Empty;
}