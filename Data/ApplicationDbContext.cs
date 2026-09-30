using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<RentalContract> RentalContracts { get; set; }
}