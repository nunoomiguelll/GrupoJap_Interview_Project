using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Localization;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Services;
using GrupoJap.Rentals.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("customers")]
public sealed class CustomersController(ApplicationDbContext dbContext, Translator T) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var today = RentalRules.Today();
        var customers = await dbContext.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.FullName)
            .Select(customer => new CustomerListItemViewModel
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                DrivingLicenseNumber = customer.DrivingLicenseNumber,
                ContractCount = customer.RentalContracts.Count,
                HasActiveRental = customer.RentalContracts.Any(r => r.StartDate <= today && r.EndDate >= today)
            })
            .ToListAsync();

        return View(customers);
    }

    [HttpGet("create")]
    public IActionResult Create() => View(new Customer());

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FullName,Email,Phone,DrivingLicenseNumber")] Customer customer)
    {
        Normalize(customer);

        if (await dbContext.Customers.AnyAsync(c => c.Email == customer.Email))
        {
            ModelState.AddModelError(nameof(customer.Email), T["validation.customer_duplicate"]);
        }

        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        dbContext.Customers.Add(customer);

        if (!await TrySaveAsync(customer))
        {
            return View(customer);
        }

        TempData["SuccessMessage"] = T["msg.customer_created"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return customer is null ? NotFound() : View(customer);
    }

    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,Phone,DrivingLicenseNumber")] Customer customer)
    {
        if (id != customer.Id)
        {
            return NotFound();
        }

        var existing = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        Normalize(customer);

        if (await dbContext.Customers.AnyAsync(c => c.Email == customer.Email && c.Id != id))
        {
            ModelState.AddModelError(nameof(customer.Email), T["validation.customer_duplicate"]);
        }

        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        existing.FullName = customer.FullName;
        existing.Email = customer.Email;
        existing.Phone = customer.Phone;
        existing.DrivingLicenseNumber = customer.DrivingLicenseNumber;

        if (!await TrySaveAsync(customer))
        {
            return View(customer);
        }

        TempData["SuccessMessage"] = T["msg.customer_updated"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null)
        {
            return NotFound();
        }

        ViewBag.ContractCount = await dbContext.RentalContracts.CountAsync(r => r.CustomerId == id);
        return View(customer);
    }

    [HttpPost("{id:int}/delete"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null)
        {
            return NotFound();
        }

        if (await dbContext.RentalContracts.AnyAsync(r => r.CustomerId == id))
        {
            TempData["ErrorMessage"] = T["msg.customer_delete_blocked"].Value;
            return RedirectToAction(nameof(Index));
        }

        dbContext.Customers.Remove(customer);
        await dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = T["msg.customer_deleted"].Value;
        return RedirectToAction(nameof(Index));
    }

    private static void Normalize(Customer customer)
    {
        customer.FullName = (customer.FullName ?? string.Empty).Trim();
        customer.Email = (customer.Email ?? string.Empty).Trim().ToLowerInvariant();
        customer.Phone = (customer.Phone ?? string.Empty).Replace(" ", string.Empty).Trim();
        customer.DrivingLicenseNumber = (customer.DrivingLicenseNumber ?? string.Empty).Trim().ToUpperInvariant();
    }

    private async Task<bool> TrySaveAsync(Customer customer)
    {
        try
        {
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            ModelState.AddModelError(nameof(customer.Email), T["validation.customer_duplicate"]);
            return false;
        }
    }
}
