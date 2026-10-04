using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Services;
using GrupoJap.Rentals.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("vehicles")]
public sealed class VehiclesController(ApplicationDbContext dbContext) : Controller
{
    private const string DuplicateMessage = "Já existe um veículo com esta matrícula.";

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var today = RentalRules.Today();
        var vehicles = await dbContext.Vehicles
            .AsNoTracking()
            .OrderBy(vehicle => vehicle.Brand)
            .ThenBy(vehicle => vehicle.Model)
            .Select(vehicle => new VehicleListItemViewModel
            {
                Id = vehicle.Id,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Registration = vehicle.Registration,
                YearOfManufacture = vehicle.YearOfManufacture,
                FuelType = vehicle.FuelType,
                IsRented = vehicle.RentalContracts.Any(rental =>
                    rental.StartDate <= today && rental.EndDate >= today)
            })
            .ToListAsync();

        return View(vehicles);
    }

    [HttpGet("create")]
    public IActionResult Create() => View(new Vehicle { YearOfManufacture = DateTime.Today.Year });

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Brand,Model,Registration,YearOfManufacture,FuelType,Mileage")] Vehicle vehicle)
    {
        Normalize(vehicle);

        if (await dbContext.Vehicles.AnyAsync(existing => existing.Registration == vehicle.Registration))
        {
            ModelState.AddModelError(nameof(vehicle.Registration), DuplicateMessage);
        }

        if (!ModelState.IsValid)
        {
            return View(vehicle);
        }

        dbContext.Vehicles.Add(vehicle);

        if (!await TrySaveAsync(vehicle))
        {
            return View(vehicle);
        }

        TempData["SuccessMessage"] = "Veículo registado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var vehicle = await dbContext.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
        return vehicle is null ? NotFound() : View(vehicle);
    }

    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,Registration,YearOfManufacture,FuelType,Mileage")] Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return NotFound();
        }

        var existing = await dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        Normalize(vehicle);

        if (await dbContext.Vehicles.AnyAsync(v => v.Registration == vehicle.Registration && v.Id != id))
        {
            ModelState.AddModelError(nameof(vehicle.Registration), DuplicateMessage);
        }

        if (!ModelState.IsValid)
        {
            return View(vehicle);
        }

        existing.Brand = vehicle.Brand;
        existing.Model = vehicle.Model;
        existing.Registration = vehicle.Registration;
        existing.YearOfManufacture = vehicle.YearOfManufacture;
        existing.FuelType = vehicle.FuelType;
        existing.Mileage = vehicle.Mileage;

        if (!await TrySaveAsync(vehicle))
        {
            return View(vehicle);
        }

        TempData["SuccessMessage"] = "Veículo atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var vehicle = await dbContext.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
        if (vehicle is null)
        {
            return NotFound();
        }

        ViewBag.ContractCount = await dbContext.RentalContracts.CountAsync(r => r.VehicleId == id);
        return View(vehicle);
    }

    [HttpPost("{id:int}/delete"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
        if (vehicle is null)
        {
            return NotFound();
        }

        if (await dbContext.RentalContracts.AnyAsync(r => r.VehicleId == id))
        {
            TempData["ErrorMessage"] = "Não é possível eliminar um veículo com contratos de aluguer associados.";
            return RedirectToAction(nameof(Index));
        }

        dbContext.Vehicles.Remove(vehicle);
        await dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Veículo eliminado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private static void Normalize(Vehicle vehicle)
    {
        vehicle.Brand = (vehicle.Brand ?? string.Empty).Trim();
        vehicle.Model = (vehicle.Model ?? string.Empty).Trim();
        vehicle.Registration = (vehicle.Registration ?? string.Empty).Trim().ToUpperInvariant();
    }

    private async Task<bool> TrySaveAsync(Vehicle vehicle)
    {
        try
        {
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            ModelState.AddModelError(nameof(vehicle.Registration), DuplicateMessage);
            return false;
        }
    }
}
