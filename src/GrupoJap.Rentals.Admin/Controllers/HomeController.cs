using System.Diagnostics;
using GrupoJap.Rentals.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.ViewModels;

namespace GrupoJap.Rentals.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public HomeController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var vehicleCount = await _dbContext.Vehicles.CountAsync();
        var customerCount = await _dbContext.Customers.CountAsync();
        var activeRentals = _dbContext.RentalContracts
            .AsNoTracking()
            .Where(rental => rental.StartDate <= today && rental.EndDate >= today);
        var activeRentalCount = await activeRentals.CountAsync();
        var rentedVehicleCount = await activeRentals
            .Select(rental => rental.VehicleId)
            .Distinct()
            .CountAsync();
        var recentRentals = await _dbContext.RentalContracts
            .AsNoTracking()
            .OrderByDescending(rental => rental.StartDate)
            .ThenByDescending(rental => rental.Id)
            .Take(5)
            .Select(rental => new RecentRentalViewModel
            {
                Id = rental.Id,
                CustomerName = rental.Customer!.FullName,
                VehicleName = rental.Vehicle!.Brand + " " + rental.Vehicle.Model,
                Registration = rental.Vehicle.Registration,
                StartDate = rental.StartDate!.Value,
                EndDate = rental.EndDate!.Value,
                IsActive = rental.StartDate <= today && rental.EndDate >= today,
                IsUpcoming = rental.StartDate > today
            })
            .ToListAsync();

        return View(new DashboardViewModel
        {
            VehicleCount = vehicleCount,
            AvailableVehicleCount = Math.Max(0, vehicleCount - rentedVehicleCount),
            ActiveRentalCount = activeRentalCount,
            CustomerCount = customerCount,
            RecentRentals = recentRentals
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
