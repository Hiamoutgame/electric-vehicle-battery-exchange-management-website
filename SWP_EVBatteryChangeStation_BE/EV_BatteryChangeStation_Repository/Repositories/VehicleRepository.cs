using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories;

public sealed class VehicleRepository : IVehicleRepository
{
    private readonly AppDbContext _context;

    public VehicleRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Vehicle>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return _context.Vehicles
            .AsNoTracking()
            .Include(x => x.Model)
                .ThenInclude(x => x!.BatteryType)
            .Include(x => x.CurrentBattery)
            .Where(x => x.OwnerId == ownerId)
            .OrderByDescending(x => x.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public Task<Vehicle?> GetByIdWithDetailsAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        return _context.Vehicles
            .AsNoTracking()
            .Include(x => x.Model)
                .ThenInclude(x => x!.BatteryType)
            .Include(x => x.Owner)
                .ThenInclude(x => x!.Role)
            .Include(x => x.CurrentBattery)
                .ThenInclude(x => x!.BatteryType)
            .FirstOrDefaultAsync(x => x.VehicleId == vehicleId, cancellationToken);
    }

    public Task<Vehicle?> GetOwnedVehicleAsync(Guid vehicleId, Guid ownerId, CancellationToken cancellationToken = default)
    {
        return _context.Vehicles
            .AsNoTracking()
            .Include(x => x.Model)
                .ThenInclude(x => x!.BatteryType)
            .Include(x => x.CurrentBattery)
            .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.OwnerId == ownerId, cancellationToken);
    }

    public Task<bool> ExistsLicensePlateAsync(string licensePlate, Guid? excludeVehicleId = null, CancellationToken cancellationToken = default)
    {
        var normalizedLicensePlate = licensePlate.Trim();

        return _context.Vehicles
            .AsNoTracking()
            .AnyAsync(x => x.LicensePlate == normalizedLicensePlate
                           && (!excludeVehicleId.HasValue || x.VehicleId != excludeVehicleId.Value),
                cancellationToken);
    }

    public Task<bool> ExistsVinAsync(string? vin, Guid? excludeVehicleId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(vin))
        {
            return Task.FromResult(false);
        }

        var normalizedVin = vin.Trim();

        return _context.Vehicles
            .AsNoTracking()
            .AnyAsync(x => x.Vin == normalizedVin
                           && (!excludeVehicleId.HasValue || x.VehicleId != excludeVehicleId.Value),
                cancellationToken);
    }
}


