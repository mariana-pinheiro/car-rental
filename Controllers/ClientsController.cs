using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers;

public class ClientsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Clients
    public async Task<IActionResult> Index()
    {
        var clients = await _context.Clients
            .OrderBy(c => c.FullName)
            .ToListAsync();

        return View(clients);
    }

    // GET: /Clients/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Clients/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Client client)
    {
        client.FullName = client.FullName.Trim();
        client.Email = client.Email.Trim().ToLowerInvariant();
        client.Phone = client.Phone.Trim();
        client.DrivingLicense = client.DrivingLicense.Trim().ToUpperInvariant();

        bool emailExists = await _context.Clients
            .AnyAsync(c => c.Email == client.Email);

        if (emailExists)
        {
            ModelState.AddModelError(
                nameof(client.Email),
                "Já existe um cliente com este email."
            );
        }

        if (!ModelState.IsValid)
        {
            return View(client);
        }

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Clients/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

    // GET: /Clients/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = await _context.Clients.FindAsync(id);

        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

    // POST: /Clients/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Client client)
    {
        if (id != client.Id)
        {
            return NotFound();
        }

        client.FullName = client.FullName.Trim();
        client.Email = client.Email.Trim().ToLowerInvariant();
        client.Phone = client.Phone.Trim();
        client.DrivingLicense = client.DrivingLicense.Trim().ToUpperInvariant();

        bool emailExists = await _context.Clients
            .AnyAsync(c =>
                c.Email == client.Email &&
                c.Id != client.Id);

        if (emailExists)
        {
            ModelState.AddModelError(
                nameof(client.Email),
                "Já existe outro cliente com este email."
            );
        }

        if (!ModelState.IsValid)
        {
            return View(client);
        }

        _context.Update(client);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Clients/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

    // POST: /Clients/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client == null)
        {
            return NotFound();
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}