using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers;

public class VehiclesController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehiclesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Vehicles
    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        var vehicles = await _context.Vehicles
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .ToListAsync();
        var rentedVehicleIds = await _context.RentalContracts
            .Where(c =>
                !c.CompletedAt.HasValue &&
                !c.CancelledAt.HasValue &&
                c.StartDate <= today &&
                c.EndDate >= today)
            .Select(c => c.VehicleId)
            .Distinct()
            .ToListAsync();

        ViewBag.RentedVehicleIds = rentedVehicleIds;

        return View(vehicles);
    }

    // GET: /Vehicles/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Vehicles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vehicle vehicle)
    {
        vehicle.Brand = vehicle.Brand.Trim();
        vehicle.Model = vehicle.Model.Trim();
        vehicle.LicensePlate = vehicle.LicensePlate.Trim().ToUpperInvariant();
        vehicle.FuelType = vehicle.FuelType.Trim();

        bool licensePlateExists = await _context.Vehicles
            .AnyAsync(v => v.LicensePlate == vehicle.LicensePlate);

        if (licensePlateExists)
        {
            ModelState.AddModelError(
                nameof(vehicle.LicensePlate),
                "Já existe um veículo com esta matrícula."
            );
        }

        if (!ModelState.IsValid)
        {
            return View(vehicle);
        }

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Vehicles/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound();
        }

        var today = DateTime.Today;

        var isRented = await _context.RentalContracts
            .AnyAsync(c =>
                c.VehicleId == vehicle.Id &&
                !c.CompletedAt.HasValue &&
                !c.CancelledAt.HasValue &&
                c.StartDate <= today &&
                c.EndDate >= today);

        ViewBag.IsRented = isRented;

        return View(vehicle);
    }

    // GET: /Vehicles/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    // POST: /Vehicles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return NotFound();
        }

        vehicle.Brand = vehicle.Brand.Trim();
        vehicle.Model = vehicle.Model.Trim();
        vehicle.LicensePlate = vehicle.LicensePlate.Trim().ToUpperInvariant();
        vehicle.FuelType = vehicle.FuelType.Trim();

        bool licensePlateExists = await _context.Vehicles
            .AnyAsync(v =>
                v.LicensePlate == vehicle.LicensePlate &&
                v.Id != vehicle.Id);

        if (licensePlateExists)
        {
            ModelState.AddModelError(
                nameof(vehicle.LicensePlate),
                "Já existe outro veículo com esta matrícula."
            );
        }

        if (!ModelState.IsValid)
        {
            return View(vehicle);
        }

        _context.Update(vehicle);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Vehicles/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    // POST: /Vehicles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        var hasContracts = await _context.RentalContracts
            .AnyAsync(c => c.VehicleId == id);

        if (hasContracts)
        {
            TempData["ErrorMessage"] =
                "Não é possível eliminar este veículo porque existem contratos associados.";

            return RedirectToAction(nameof(Index));
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Veículo eliminado com sucesso.";

        return RedirectToAction(nameof(Index));
    }
}