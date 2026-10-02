using CarRental.Controllers;
using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Tests;

public class RequirementTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Vehicle_WithDuplicateLicensePlate_IsRejected()
    {
        await using var context = CreateContext();

        context.Vehicles.Add(new Vehicle
        {
            Brand = "BMW",
            Model = "Série 1",
            LicensePlate = "AA-00-AA",
            ManufacturingYear = 2024,
            FuelType = "Gasolina"
        });

        await context.SaveChangesAsync();

        var controller = new VehiclesController(context);

        var duplicateVehicle = new Vehicle
        {
            Brand = "Audi",
            Model = "A3",
            LicensePlate = "AA-00-AA",
            ManufacturingYear = 2024,
            FuelType = "Gasolina"
        };

        var result = await controller.Create(duplicateVehicle);

        Assert.False(controller.ModelState.IsValid);
        Assert.True(
            controller.ModelState.ContainsKey(nameof(Vehicle.LicensePlate))
        );
        Assert.IsType<ViewResult>(result);

        Assert.Equal(1, await context.Vehicles.CountAsync());
    }

    [Fact]
    public async Task Client_WithDuplicateEmail_IsRejected()
    {
        await using var context = CreateContext();

        context.Clients.Add(new Client
        {
            FullName = "Cliente Existente",
            Email = "cliente@example.com",
            Phone = "912345678",
            DrivingLicense = "P-123456"
        });

        await context.SaveChangesAsync();

        var controller = new ClientsController(context);

        var duplicateClient = new Client
        {
            FullName = "Outro Cliente",
            Email = "cliente@example.com",
            Phone = "913456789",
            DrivingLicense = "P-654321"
        };

        var result = await controller.Create(duplicateClient);

        Assert.False(controller.ModelState.IsValid);
        Assert.True(
            controller.ModelState.ContainsKey(nameof(Client.Email))
        );
        Assert.IsType<ViewResult>(result);

        Assert.Equal(1, await context.Clients.CountAsync());
    }

    [Fact]
    public async Task RentalContract_WithPastStartDate_IsRejected()
    {
        await using var context = CreateContext();

        var client = new Client
        {
            FullName = "Mariana Pinheiro",
            Email = "mariana@example.com",
            Phone = "912345678",
            DrivingLicense = "P-123456"
        };

        var vehicle = new Vehicle
        {
            Brand = "BMW",
            Model = "Série 1",
            LicensePlate = "AA-00-AA",
            ManufacturingYear = 2024,
            FuelType = "Gasolina"
        };

        context.Clients.Add(client);
        context.Vehicles.Add(vehicle);

        await context.SaveChangesAsync();

        var controller = new RentalContractsController(context);

        var contract = new RentalContract
        {
            ClientId = client.Id,
            VehicleId = vehicle.Id,
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(2),
            InitialMileage = 10000
        };

        var result = await controller.Create(contract);

        Assert.False(controller.ModelState.IsValid);
        Assert.True(
            controller.ModelState.ContainsKey(nameof(RentalContract.StartDate))
        );
        Assert.IsType<ViewResult>(result);

        Assert.Empty(context.RentalContracts);
    }

    [Fact]
    public async Task Vehicle_WithActiveRentalContract_IsMarkedAsRented()
    {
        await using var context = CreateContext();

        var client = new Client
        {
            FullName = "Mariana Pinheiro",
            Email = "mariana@example.com",
            Phone = "912345678",
            DrivingLicense = "P-123456"
        };

        var vehicle = new Vehicle
        {
            Brand = "BMW",
            Model = "Série 1",
            LicensePlate = "AA-00-AA",
            ManufacturingYear = 2024,
            FuelType = "Gasolina"
        };

        context.Clients.Add(client);
        context.Vehicles.Add(vehicle);

        await context.SaveChangesAsync();

        context.RentalContracts.Add(new RentalContract
        {
            ClientId = client.Id,
            VehicleId = vehicle.Id,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(3),
            InitialMileage = 10000
        });

        await context.SaveChangesAsync();

        var controller = new VehiclesController(context);

        var result = await controller.Details(vehicle.Id);

        Assert.IsType<ViewResult>(result);
        Assert.True((bool)controller.ViewBag.IsRented);
    }

    [Fact]
    public async Task RentalContract_WithoutClientAndVehicle_IsRejected()
    {
        await using var context = CreateContext();

        var controller = new RentalContractsController(context);

        var contract = new RentalContract
        {
            ClientId = 0,
            VehicleId = 0,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            InitialMileage = 10000
        };

        controller.ModelState.AddModelError(
            nameof(RentalContract.ClientId),
            "O cliente é obrigatório."
        );

        controller.ModelState.AddModelError(
            nameof(RentalContract.VehicleId),
            "O veículo é obrigatório."
        );

        var result = await controller.Create(contract);

        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(nameof(RentalContract.ClientId)));
        Assert.True(controller.ModelState.ContainsKey(nameof(RentalContract.VehicleId)));
        Assert.IsType<ViewResult>(result);
        Assert.Empty(context.RentalContracts);
    }

    [Fact]
    public async Task Vehicle_WithoutActiveRentalContract_IsMarkedAsAvailable()
    {
        await using var context = CreateContext();

        var vehicle = new Vehicle
        {
            Brand = "BMW",
            Model = "Série 1",
            LicensePlate = "AA-00-AA",
            ManufacturingYear = 2024,
            FuelType = "Gasolina"
        };

        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        var controller = new VehiclesController(context);

        var result = await controller.Details(vehicle.Id);

        Assert.IsType<ViewResult>(result);
        Assert.False((bool)controller.ViewBag.IsRented);
    }
}