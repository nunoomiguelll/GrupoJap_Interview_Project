using GrupoJap.Rentals.Data;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Services;

/// <summary>Regra de não sobreposição de contratos (datas inclusivas) num só sítio.</summary>
public sealed class RentalAvailabilityService(ApplicationDbContext dbContext)
{
    /// <summary>Existe algum contrato do veículo que se sobreponha ao período? Ignora o contrato <paramref name="excludeContractId"/>.</summary>
    public Task<bool> HasOverlapAsync(int vehicleId, DateOnly start, DateOnly end, int? excludeContractId = null, CancellationToken cancellationToken = default)
        => dbContext.RentalContracts.AnyAsync(r =>
                r.VehicleId == vehicleId
                && (excludeContractId == null || r.Id != excludeContractId)
                && r.StartDate <= end
                && r.EndDate >= start,
            cancellationToken);
}
