using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers;

public class RentalContractsController : Controller
{
    private readonly ApplicationDbContext _context;

    public RentalContractsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /RentalContracts
    public async Task<IActionResult> Index()
    {
        var contracts = await _context.RentalContracts
            .Include(c => c.Client)
            .Include(c => c.Vehicle)
            .OrderByDescending(c => c.StartDate)
            .ToListAsync();

        return View(contracts);
    }

    // GET: /RentalContracts/Create
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();

        return View(new RentalContract
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1)
        });
    }

    // POST: /RentalContracts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RentalContract contract)
    {
        if (contract.StartDate.Date < DateTime.Today)
        {
            ModelState.AddModelError(
                nameof(contract.StartDate),
                "A data de início não pode ser anterior à data atual."
            );
        }

        bool overlappingContract = await _context.RentalContracts
    .AnyAsync(c =>
        c.VehicleId == contract.VehicleId &&
        !c.CompletedAt.HasValue &&
        !c.CancelledAt.HasValue &&
        contract.StartDate <= c.EndDate &&
        contract.EndDate >= c.StartDate);

        if (overlappingContract)
        {
            ModelState.AddModelError(
                nameof(contract.VehicleId),
                "O veículo já possui um contrato neste período."
            );
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdowns(contract.ClientId, contract.VehicleId);
            return View(contract);
        }

        _context.RentalContracts.Add(contract);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns(
        int? selectedClientId = null,
        int? selectedVehicleId = null)
    {
        var clients = await _context.Clients
            .OrderBy(c => c.FullName)
            .ToListAsync();

        var vehicles = await _context.Vehicles
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .ToListAsync();

        ViewBag.Clients = new SelectList(
            clients,
            "Id",
            "FullName",
            selectedClientId
        );

        ViewBag.Vehicles = new SelectList(
            vehicles.Select(v => new
            {
                v.Id,
                Description = $"{v.Brand} {v.Model} - {v.LicensePlate}"
            }),
            "Id",
            "Description",
            selectedVehicleId
        );
    }

    [HttpGet]
    public async Task<IActionResult> SearchClients(string? term)
    {
        term = term?.Trim() ?? string.Empty;

        var clients = await _context.Clients
            .Where(c =>
                term == string.Empty ||
                c.FullName.Contains(term) ||
                c.Email.Contains(term))
            .OrderBy(c => c.FullName)
            .Take(20)
            .Select(c => new
            {
                id = c.Id,
                text = $"{c.FullName} - {c.Email}"
            })
            .ToListAsync();

        return Json(clients);
    }

    [HttpGet]
    public async Task<IActionResult> SearchVehicles(string? term)
    {
        term = term?.Trim() ?? string.Empty;

        var vehicles = await _context.Vehicles
            .Where(v =>
                term == string.Empty ||
                v.Brand.Contains(term) ||
                v.Model.Contains(term) ||
                v.LicensePlate.Contains(term))
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .Take(20)
            .Select(v => new
            {
                id = v.Id,
                text = $"{v.Brand} {v.Model} - {v.LicensePlate}"
            })
            .ToListAsync();

        return Json(vehicles);
    }

    // GET: /RentalContracts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var contract = await _context.RentalContracts
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
        {
            return NotFound();
        }

        var today = DateTime.Today;

        if (contract.CompletedAt.HasValue ||
            contract.CancelledAt.HasValue)
        {
            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }

        if (contract.EndDate.Date < today)
        {
            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }

        ViewBag.IsActive = contract.StartDate.Date <= today;

        await LoadDropdowns(contract.ClientId, contract.VehicleId);
        var client = await _context.Clients.FindAsync(contract.ClientId);
        var vehicle = await _context.Vehicles.FindAsync(contract.VehicleId);

        ViewBag.ClientName = client?.FullName;

        ViewBag.VehicleDescription = vehicle == null
            ? string.Empty
            : $"{vehicle.Brand} {vehicle.Model} - {vehicle.LicensePlate}";

        return View(contract);
    }


    // POST: /RentalContracts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RentalContract contract)
    {
        if (id != contract.Id)
        {
            return NotFound();
        }

        var existingContract = await _context.RentalContracts
            .FirstOrDefaultAsync(c => c.Id == id);

        if (existingContract == null)
        {
            return NotFound();
        }

        var today = DateTime.Today;

        if (existingContract.CompletedAt.HasValue ||
            existingContract.CancelledAt.HasValue ||
            existingContract.EndDate.Date < today)
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        var isActive = existingContract.StartDate.Date <= today;

        if (isActive)
        {
            // Num contrato ativo, apenas permitimos alterar a data final
            if (contract.EndDate <= existingContract.StartDate)
            {
                ModelState.AddModelError(
                    nameof(contract.EndDate),
                    "A data de fim deve ser posterior à data de início."
                );
            }

            bool overlappingContract = await _context.RentalContracts
                .AnyAsync(c =>
                    c.Id != existingContract.Id &&
                    c.VehicleId == existingContract.VehicleId &&
                    !c.CompletedAt.HasValue &&
                    !c.CancelledAt.HasValue &&
                    existingContract.StartDate <= c.EndDate &&
                    contract.EndDate >= c.StartDate);

            if (overlappingContract)
            {
                ModelState.AddModelError(
                    nameof(contract.EndDate),
                    "O veículo já possui outro contrato neste período."
                );
            }

            if (!ModelState.IsValid)
            {
                // Repomos os dados que não podem ser alterados
                contract.ClientId = existingContract.ClientId;
                contract.VehicleId = existingContract.VehicleId;
                contract.StartDate = existingContract.StartDate;
                contract.InitialMileage = existingContract.InitialMileage;

                ViewBag.IsActive = true;

                await LoadDropdowns(
                    existingContract.ClientId,
                    existingContract.VehicleId
                );

                return View(contract);
            }

            existingContract.EndDate = contract.EndDate;
        }
        else
        {
            // Contrato agendado: edição completa

            bool overlappingContract = await _context.RentalContracts
                .AnyAsync(c =>
                    c.Id != existingContract.Id &&
                    c.VehicleId == contract.VehicleId &&
                    !c.CompletedAt.HasValue &&
                    !c.CancelledAt.HasValue &&
                    contract.StartDate <= c.EndDate &&
                    contract.EndDate >= c.StartDate);

            if (overlappingContract)
            {
                ModelState.AddModelError(
                    nameof(contract.VehicleId),
                    "O veículo já possui um contrato neste período."
                );
            }

            if (!ModelState.IsValid)
            {
                ViewBag.IsActive = false;

                await LoadDropdowns(
                    contract.ClientId,
                    contract.VehicleId
                );

                return View(contract);
            }

            existingContract.ClientId = contract.ClientId;
            existingContract.VehicleId = contract.VehicleId;
            existingContract.StartDate = contract.StartDate;
            existingContract.EndDate = contract.EndDate;
            existingContract.InitialMileage = contract.InitialMileage;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /RentalContracts/Finish/5
    public async Task<IActionResult> Finish(int? id)
    {
        if (id == null)
            return NotFound();

        var contract = await _context.RentalContracts
            .Include(c => c.Client)
            .Include(c => c.Vehicle)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
            return NotFound();

        if (contract.CompletedAt.HasValue ||
          contract.CancelledAt.HasValue ||
          contract.StartDate.Date > DateTime.Today)
        {
            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }

        return View(contract);
    }

    // POST: /RentalContracts/Finish/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Finish(int id, int? finalMileage)
    {
        var contract = await _context.RentalContracts
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
            return NotFound();

        if (contract.CompletedAt.HasValue ||
           contract.CancelledAt.HasValue ||
           contract.StartDate.Date > DateTime.Today)
        {
            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }

        if (!finalMileage.HasValue)
        {
            ModelState.AddModelError(
                "FinalMileage",
                "A quilometragem final é obrigatória."
            );
        }
        else if (finalMileage.Value < contract.InitialMileage)
        {
            ModelState.AddModelError(
                "FinalMileage",
                "A quilometragem final não pode ser inferior à quilometragem inicial."
            );
        }

        if (!ModelState.IsValid)
        {
            contract.Client = await _context.Clients
                .FindAsync(contract.ClientId);

            contract.Vehicle = await _context.Vehicles
                .FindAsync(contract.VehicleId);

            contract.FinalMileage = finalMileage;

            return View(contract);
        }

        contract.FinalMileage = finalMileage.Value;
        contract.CompletedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /RentalContracts/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var contract = await _context.RentalContracts
            .Include(c => c.Client)
            .Include(c => c.Vehicle)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
        {
            return NotFound();
        }

        return View(contract);
    }

    // GET: /RentalContracts/Cancel/5
    public async Task<IActionResult> Cancel(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var contract = await _context.RentalContracts
            .Include(c => c.Client)
            .Include(c => c.Vehicle)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
        {
            return NotFound();
        }

        if (contract.CompletedAt.HasValue ||
            contract.CancelledAt.HasValue ||
            contract.StartDate.Date <= DateTime.Today)
        {
            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }

        return View(contract);
    }


    // POST: /RentalContracts/Cancel/5
    [HttpPost, ActionName("Cancel")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelConfirmed(int id)
    {
        var contract = await _context.RentalContracts
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
        {
            return NotFound();
        }

        if (contract.CompletedAt.HasValue ||
            contract.CancelledAt.HasValue ||
            contract.StartDate.Date <= DateTime.Today)
        {
            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }

        contract.CancelledAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = contract.Id });
    }
}