using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Services;
using GrupoJap.Rentals.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("rentals")]
public sealed class RentalsController(ApplicationDbContext dbContext, RentalAvailabilityService availability) : Controller
{
    private const string OverlapMessage = "Este veículo já tem um contrato que se sobrepõe ao período indicado.";

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var today = RentalRules.Today();
        var rows = await dbContext.RentalContracts
            .AsNoTracking()
            .OrderByDescending(r => r.StartDate)
            .ThenByDescending(r => r.Id)
            .Select(r => new
            {
                r.Id,
                CustomerName = r.Customer!.FullName,
                VehicleName = r.Vehicle!.Brand + " " + r.Vehicle.Model,
                r.Vehicle.Registration,
                Start = r.StartDate!.Value,
                End = r.EndDate!.Value,
                Mileage = r.InitialMileage!.Value
            })
            .ToListAsync();

        var model = rows.Select(r => new RentalListItemViewModel
        {
            Id = r.Id,
            CustomerName = r.CustomerName,
            VehicleName = r.VehicleName,
            Registration = r.Registration,
            StartDate = r.Start,
            EndDate = r.End,
            InitialMileage = r.Mileage,
            Status = RentalRules.GetStatus(r.Start, r.End, today)
        }).ToList();

        return View(model);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(int? vehicleId, int? customerId)
    {
        await LoadListsAsync();
        return View(new RentalContract
        {
            VehicleId = vehicleId,
            CustomerId = customerId,
            StartDate = RentalRules.Today(),
            EndDate = RentalRules.Today().AddDays(1)
        });
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CustomerId,VehicleId,StartDate,EndDate,InitialMileage")] RentalContract contract)
    {
        await ValidateReferencesAndOverlapAsync(contract, null);

        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return View(contract);
        }

        dbContext.RentalContracts.Add(contract);
        await dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Contrato de aluguer registado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var contract = await dbContext.RentalContracts.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        if (contract is null)
        {
            return NotFound();
        }

        await LoadListsAsync();
        return View(contract);
    }

    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,VehicleId,StartDate,EndDate,InitialMileage")] RentalContract contract)
    {
        if (id != contract.Id)
        {
            return NotFound();
        }

        var existing = await dbContext.RentalContracts.FirstOrDefaultAsync(r => r.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        // Contratos já iniciados podem ser corrigidos sem alterar a data de início original.
        if (existing.StartDate == contract.StartDate)
        {
            ModelState.Remove(nameof(RentalContract.StartDate));
        }

        await ValidateReferencesAndOverlapAsync(contract, id);

        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return View(contract);
        }

        existing.CustomerId = contract.CustomerId;
        existing.VehicleId = contract.VehicleId;
        existing.StartDate = contract.StartDate;
        existing.EndDate = contract.EndDate;
        existing.InitialMileage = contract.InitialMileage;
        await dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Contrato de aluguer atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var contract = await dbContext.RentalContracts
            .AsNoTracking()
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .FirstOrDefaultAsync(r => r.Id == id);

        return contract is null ? NotFound() : View(contract);
    }

    [HttpPost("{id:int}/delete"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contract = await dbContext.RentalContracts.FirstOrDefaultAsync(r => r.Id == id);
        if (contract is null)
        {
            return NotFound();
        }

        dbContext.RentalContracts.Remove(contract);
        await dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Contrato de aluguer eliminado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateReferencesAndOverlapAsync(RentalContract contract, int? currentId)
    {
        if (contract.CustomerId is int customerId && !await dbContext.Customers.AnyAsync(c => c.Id == customerId))
        {
            ModelState.AddModelError(nameof(RentalContract.CustomerId), "O cliente selecionado não existe.");
        }

        if (contract.VehicleId is int vehicleId && !await dbContext.Vehicles.AnyAsync(v => v.Id == vehicleId))
        {
            ModelState.AddModelError(nameof(RentalContract.VehicleId), "O veículo selecionado não existe.");
        }

        if (contract.VehicleId is int vid && contract.StartDate is DateOnly start && contract.EndDate is DateOnly end && end > start)
        {
            if (await availability.HasOverlapAsync(vid, start, end, currentId))
            {
                ModelState.AddModelError(nameof(RentalContract.VehicleId), OverlapMessage);
            }
        }
    }

    private async Task LoadListsAsync()
    {
        ViewBag.Customers = await dbContext.Customers.AsNoTracking()
            .OrderBy(c => c.FullName)
            .Select(c => new SelectListItem(c.FullName + " (" + c.Email + ")", c.Id.ToString()))
            .ToListAsync();

        ViewBag.Vehicles = await dbContext.Vehicles.AsNoTracking()
            .OrderBy(v => v.Brand).ThenBy(v => v.Model)
            .Select(v => new SelectListItem(v.Brand + " " + v.Model + " · " + v.Registration, v.Id.ToString()))
            .ToListAsync();
    }
}
