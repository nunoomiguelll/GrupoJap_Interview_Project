using GrupoJap.Rentals.Models;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Data;

/// <summary>
/// Dados de demonstração para desenvolvimento. Só corre quando <c>SeedDemoData</c> está ativo
/// e a frota está vazia, por isso nunca altera dados existentes.
/// </summary>
public static class DemoDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        if (!configuration.GetValue<bool>("SeedDemoData"))
        {
            return;
        }

        var db = services.GetRequiredService<ApplicationDbContext>();
        if (await db.Vehicles.AnyAsync())
        {
            return;
        }

        var vehicles = new List<Vehicle>
        {
            new() { Brand = "Toyota", Model = "Yaris Hybrid", Registration = "AT-21-PL", YearOfManufacture = 2023, FuelType = FuelType.Hybrid, Mileage = 18_420 },
            new() { Brand = "Toyota", Model = "Corolla Touring", Registration = "BK-42-RM", YearOfManufacture = 2022, FuelType = FuelType.Hybrid, Mileage = 36_900 },
            new() { Brand = "Peugeot", Model = "208", Registration = "CL-08-TS", YearOfManufacture = 2024, FuelType = FuelType.Petrol, Mileage = 6_150 },
            new() { Brand = "Peugeot", Model = "3008", Registration = "DN-30-AV", YearOfManufacture = 2021, FuelType = FuelType.Diesel, Mileage = 61_200 },
            new() { Brand = "Renault", Model = "Clio", Registration = "EQ-55-CL", YearOfManufacture = 2023, FuelType = FuelType.Petrol, Mileage = 22_700 },
            new() { Brand = "Renault", Model = "Megane E-Tech", Registration = "FR-77-ET", YearOfManufacture = 2024, FuelType = FuelType.Electric, Mileage = 9_800 },
            new() { Brand = "Tesla", Model = "Model 3", Registration = "GT-33-MD", YearOfManufacture = 2023, FuelType = FuelType.Electric, Mileage = 27_350 },
            new() { Brand = "Volkswagen", Model = "Golf", Registration = "HV-19-GF", YearOfManufacture = 2022, FuelType = FuelType.Diesel, Mileage = 44_600 },
            new() { Brand = "Volkswagen", Model = "Polo", Registration = "IP-64-VW", YearOfManufacture = 2021, FuelType = FuelType.Petrol, Mileage = 52_100 },
            new() { Brand = "Hyundai", Model = "Tucson", Registration = "JT-80-HY", YearOfManufacture = 2024, FuelType = FuelType.Hybrid, Mileage = 11_400 },
            new() { Brand = "Fiat", Model = "500", Registration = "KF-05-CI", YearOfManufacture = 2020, FuelType = FuelType.Lpg, Mileage = 70_250 },
            new() { Brand = "Mercedes-Benz", Model = "Classe A", Registration = "LM-12-BZ", YearOfManufacture = 2023, FuelType = FuelType.Diesel, Mileage = 19_900 },
            new() { Brand = "Nissan", Model = "Qashqai", Registration = "MQ-48-NS", YearOfManufacture = 2022, FuelType = FuelType.Hybrid, Mileage = 33_800 },
            new() { Brand = "Kia", Model = "EV6", Registration = "NK-61-EV", YearOfManufacture = 2024, FuelType = FuelType.Electric, Mileage = 5_200 }
        };
        db.Vehicles.AddRange(vehicles);

        var customers = new List<Customer>
        {
            new() { FullName = "Marta Sousa", Email = "marta.sousa@example.com", Phone = "912000111", DrivingLicenseNumber = "L-458812" },
            new() { FullName = "Rui Carvalho", Email = "rui.carvalho@example.com", Phone = "933000222", DrivingLicenseNumber = "L-772031" },
            new() { FullName = "Inês Martins", Email = "ines.martins@example.com", Phone = "964000333", DrivingLicenseNumber = "L-119045" }
        };
        db.Customers.AddRange(customers);
        await db.SaveChangesAsync();

        // Contratos inseridos diretamente (dados de demonstração), por isso podem incluir alugueres em curso e concluídos.
        var today = DateOnly.FromDateTime(DateTime.Today);
        db.RentalContracts.AddRange(
            new RentalContract { CustomerId = customers[0].Id, VehicleId = vehicles[0].Id, StartDate = today.AddDays(-2), EndDate = today.AddDays(3), InitialMileage = vehicles[0].Mileage },
            new RentalContract { CustomerId = customers[1].Id, VehicleId = vehicles[6].Id, StartDate = today.AddDays(-1), EndDate = today.AddDays(5), InitialMileage = vehicles[6].Mileage },
            new RentalContract { CustomerId = customers[2].Id, VehicleId = vehicles[3].Id, StartDate = today, EndDate = today.AddDays(2), InitialMileage = vehicles[3].Mileage },
            new RentalContract { CustomerId = customers[0].Id, VehicleId = vehicles[2].Id, StartDate = today.AddDays(7), EndDate = today.AddDays(12), InitialMileage = vehicles[2].Mileage },
            new RentalContract { CustomerId = customers[1].Id, VehicleId = vehicles[9].Id, StartDate = today.AddDays(-20), EndDate = today.AddDays(-15), InitialMileage = vehicles[9].Mileage - 900 });
        await db.SaveChangesAsync();
    }
}
