namespace CarRental.Models;

public class Vehicle
{
    public int Id { get; set; }

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string LicensePlate { get; set; } = string.Empty;

    public int ManufacturingYear { get; set; }

    public string FuelType { get; set; } = string.Empty;
}