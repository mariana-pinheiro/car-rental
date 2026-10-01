using CarRental.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        var totalVehicles = await _context.Vehicles.CountAsync();
        var totalClients = await _context.Clients.CountAsync();

        var rentedVehicleIds = await _context.RentalContracts
            .Where(c =>
                !c.CompletedAt.HasValue &&
                !c.CancelledAt.HasValue &&
                c.StartDate <= today &&
                c.EndDate >= today)
            .Select(c => c.VehicleId)
            .Distinct()
            .ToListAsync();

        var rentedVehicles = rentedVehicleIds.Count;
        var availableVehicles = totalVehicles - rentedVehicles;

        var activeContracts = await _context.RentalContracts
            .Include(c => c.Client)
            .Include(c => c.Vehicle)
            .Where(c =>
                !c.CompletedAt.HasValue &&
                c.StartDate <= today &&
                c.EndDate >= today)
            .OrderBy(c => c.EndDate)
            .Take(5)
            .ToListAsync();

        var upcomingContracts = await _context.RentalContracts
            .Include(c => c.Client)
            .Include(c => c.Vehicle)
            .Where(c =>
                !c.CompletedAt.HasValue &&
                c.StartDate > today)
            .OrderBy(c => c.StartDate)
            .Take(5)
            .ToListAsync();

        ViewBag.TotalVehicles = totalVehicles;
        ViewBag.AvailableVehicles = availableVehicles;
        ViewBag.RentedVehicles = rentedVehicles;
        ViewBag.TotalClients = totalClients;
        ViewBag.ActiveContracts = activeContracts;
        ViewBag.UpcomingContracts = upcomingContracts;

        return View();
    }
}