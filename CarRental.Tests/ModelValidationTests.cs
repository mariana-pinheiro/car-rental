using System.ComponentModel.DataAnnotations;
using CarRental.Models;

namespace CarRental.Tests;

public class ModelValidationTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(
            model,
            context,
            results,
            validateAllProperties: true
        );

        return results;
    }

    [Fact]
    public void Vehicle_WithFutureManufacturingYear_IsInvalid()
    {
        var vehicle = new Vehicle
        {
            Brand = "BMW",
            Model = "Série 1",
            LicensePlate = "AA-00-AA",
            ManufacturingYear = DateTime.Now.Year + 1,
            FuelType = "Gasolina"
        };

        var results = ValidateModel(vehicle);

        Assert.Contains(
            results,
            r => r.MemberNames.Contains(nameof(Vehicle.ManufacturingYear))
        );
    }

    [Fact]
    public void Vehicle_WithValidData_IsValid()
    {
        var vehicle = new Vehicle
        {
            Brand = "BMW",
            Model = "Série 1",
            LicensePlate = "AA-00-AA",
            ManufacturingYear = DateTime.Now.Year,
            FuelType = "Gasolina"
        };

        var results = ValidateModel(vehicle);

        Assert.Empty(results);
    }

    [Fact]
    public void Vehicle_WithMissingRequiredFields_IsInvalid()
    {
        var vehicle = new Vehicle
        {
            Brand = "",
            Model = "",
            LicensePlate = "",
            ManufacturingYear = DateTime.Now.Year,
            FuelType = ""
        };

        var results = ValidateModel(vehicle);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Vehicle.Brand)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Vehicle.Model)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Vehicle.LicensePlate)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Vehicle.FuelType)));
    }

    [Fact]
    public void Client_WithPhoneContainingLessThanNineDigits_IsInvalid()
    {
        var client = new Client
        {
            FullName = "Mariana Pinheiro",
            Email = "mariana@example.com",
            Phone = "12345",
            DrivingLicense = "P-123456"
        };

        var results = ValidateModel(client);

        Assert.Contains(
            results,
            r => r.MemberNames.Contains(nameof(Client.Phone))
        );
    }

    [Fact]
    public void Client_WithNineDigitPhone_IsValid()
    {
        var client = new Client
        {
            FullName = "Mariana Pinheiro",
            Email = "mariana@example.com",
            Phone = "912345678",
            DrivingLicense = "P-123456"
        };

        var results = ValidateModel(client);

        Assert.Empty(results);
    }

    [Fact]
    public void Client_WithMissingRequiredFields_IsInvalid()
    {
        var client = new Client
        {
            FullName = "",
            Email = "",
            Phone = "",
            DrivingLicense = ""
        };

        var results = ValidateModel(client);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Client.FullName)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Client.Email)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Client.Phone)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Client.DrivingLicense)));
    }

    [Fact]
    public void Client_WithInvalidEmail_IsInvalid()
    {
        var client = new Client
        {
            FullName = "Mariana Pinheiro",
            Email = "email-invalido",
            Phone = "912345678",
            DrivingLicense = "P-123456"
        };

        var results = ValidateModel(client);

        Assert.Contains(
            results,
            r => r.MemberNames.Contains(nameof(Client.Email))
        );
    }

    [Fact]
    public void Client_WithPhoneContainingLetters_IsInvalid()
    {
        var client = new Client
        {
            FullName = "Mariana Pinheiro",
            Email = "mariana@example.com",
            Phone = "91234ABCD",
            DrivingLicense = "P-123456"
        };

        var results = ValidateModel(client);

        Assert.Contains(
            results,
            r => r.MemberNames.Contains(nameof(Client.Phone))
        );
    }

    [Fact]
    public void RentalContract_WithEndDateBeforeStartDate_IsInvalid()
    {
        var contract = new RentalContract
        {
            ClientId = 1,
            VehicleId = 1,
            StartDate = DateTime.Today.AddDays(5),
            EndDate = DateTime.Today.AddDays(4),
            InitialMileage = 10000
        };

        var results = ValidateModel(contract);

        Assert.Contains(
            results,
            r => r.MemberNames.Contains(nameof(RentalContract.EndDate))
        );
    }

    [Fact]
    public void RentalContract_WithEndDateEqualToStartDate_IsInvalid()
    {
        var date = DateTime.Today.AddDays(1);

        var contract = new RentalContract
        {
            ClientId = 1,
            VehicleId = 1,
            StartDate = date,
            EndDate = date,
            InitialMileage = 10000
        };

        var results = ValidateModel(contract);

        Assert.Contains(
            results,
            r => r.MemberNames.Contains(nameof(RentalContract.EndDate))
        );
    }

    [Fact]
    public void RentalContract_WithFinalMileageLowerThanInitialMileage_IsInvalid()
    {
        var contract = new RentalContract
        {
            ClientId = 1,
            VehicleId = 1,
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2),
            InitialMileage = 10000,
            FinalMileage = 9000
        };

        var results = ValidateModel(contract);

        Assert.Contains(
            results,
            r => r.MemberNames.Contains(nameof(RentalContract.FinalMileage))
        );
    }

    [Fact]
    public void RentalContract_WithValidData_IsValid()
    {
        var contract = new RentalContract
        {
            ClientId = 1,
            VehicleId = 1,
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(5),
            InitialMileage = 10000
        };

        var results = ValidateModel(contract);

        Assert.Empty(results);
    }
}