using GrupoJap.Rentals.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<RentalContract> RentalContracts => Set<RentalContract>();

    public DbSet<Translation> Translations => Set<Translation>();

    public DbSet<TranslationValue> TranslationValues => Set<TranslationValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RentalContract>()
            .HasOne(rentalContract => rentalContract.Customer)
            .WithMany(customer => customer.RentalContracts)
            .HasForeignKey(rentalContract => rentalContract.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RentalContract>()
            .HasOne(rentalContract => rentalContract.Vehicle)
            .WithMany(vehicle => vehicle.RentalContracts)
            .HasForeignKey(rentalContract => rentalContract.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Translation>(translation =>
        {
            translation.HasIndex(t => t.Key).IsUnique();
            translation.HasIndex(t => t.Category);
        });

        modelBuilder.Entity<TranslationValue>(value =>
        {
            value.HasKey(v => new { v.TranslationId, v.Language });
            value.HasOne(v => v.Translation)
                .WithMany(t => t.Values)
                .HasForeignKey(v => v.TranslationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}