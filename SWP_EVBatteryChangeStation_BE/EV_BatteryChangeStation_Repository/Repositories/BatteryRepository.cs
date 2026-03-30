using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using EV_BatteryChangeStation_Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories;

public sealed class BatteryRepository : IBatteryRepository
{
    private readonly AppDbContext _context;

    public BatteryRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Battery?> GetByIdAsync(Guid batteryId, CancellationToken cancellationToken = default)
    {
        return _context.Batteries
            .AsNoTracking()
            .Include(x => x.BatteryType)
            .Include(x => x.Station)
            .FirstOrDefaultAsync(x => x.BatteryId == batteryId, cancellationToken);
    }

    public Task<Battery?> GetBestAvailableBatteryAsync(Guid stationId, Guid batteryTypeId, CancellationToken cancellationToken = default)
    {
        return _context.Batteries
            .AsNoTracking()
            .Where(x => x.StationId == stationId
                        && x.BatteryTypeId == batteryTypeId
                        && x.Status == "AVAILABLE")
            .OrderByDescending(x => x.StateOfHealth ?? 0)
            .ThenByDescending(x => x.CurrentChargeLevel ?? 0)
            .ThenBy(x => x.CreateDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> HasAvailableCompatibleBatteryAsync(Guid stationId, Guid batteryTypeId, CancellationToken cancellationToken = default)
    {
        return _context.Batteries
            .AsNoTracking()
            .AnyAsync(x => x.StationId == stationId
                           && x.BatteryTypeId == batteryTypeId
                           && x.Status == "AVAILABLE",
                cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> GetAvailableCountsByStationAsync(Guid? batteryTypeId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Battery> query = _context.Batteries
            .AsNoTracking()
            .Where(x => x.StationId != null && x.Status == "AVAILABLE");

        if (batteryTypeId.HasValue)
        {
            query = query.Where(x => x.BatteryTypeId == batteryTypeId.Value);
        }

        return await query
            .GroupBy(x => x.StationId!.Value)
            .Select(g => new { StationId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.StationId, x => x.Count, cancellationToken);
    }

    public Task<List<Battery>> GetStationInventoryAsync(Guid stationId, string? status = null, Guid? batteryTypeId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Battery> query = _context.Batteries
            .AsNoTracking()
            .Include(x => x.BatteryType)
            .Include(x => x.Station)
            .Where(x => x.StationId == stationId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            query = query.Where(x => x.Status.ToUpper() == normalizedStatus);
        }

        if (batteryTypeId.HasValue)
        {
            query = query.Where(x => x.BatteryTypeId == batteryTypeId.Value);
        }

        return query
            .OrderBy(x => x.Status)
            .ThenBy(x => x.SerialNumber)
            .ToListAsync(cancellationToken);
    }

    public Task<List<StationInventorySummary>> GetInventorySummaryAsync(Guid? stationId = null, Guid? batteryTypeId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Station> query = _context.Stations.AsNoTracking();

        if (stationId.HasValue)
        {
            query = query.Where(x => x.StationId == stationId.Value);
        }

        return query
            .Select(x => new StationInventorySummary(
                x.StationId,
                x.StationName,
                x.Batteries.Count(b => (!batteryTypeId.HasValue || b.BatteryTypeId == batteryTypeId.Value) && b.Status == "AVAILABLE"),
                x.Batteries.Count(b => (!batteryTypeId.HasValue || b.BatteryTypeId == batteryTypeId.Value) && b.Status == "RESERVED"),
                x.Batteries.Count(b => (!batteryTypeId.HasValue || b.BatteryTypeId == batteryTypeId.Value) && b.Status == "CHARGING"),
                x.Batteries.Count(b => (!batteryTypeId.HasValue || b.BatteryTypeId == batteryTypeId.Value) && b.Status == "IN_VEHICLE"),
                x.Batteries.Count(b => (!batteryTypeId.HasValue || b.BatteryTypeId == batteryTypeId.Value) && b.Status == "MAINTENANCE"),
                x.Batteries.Count(b => (!batteryTypeId.HasValue || b.BatteryTypeId == batteryTypeId.Value) && b.Status == "FAULTY"),
                x.Batteries.Count(b => !batteryTypeId.HasValue || b.BatteryTypeId == batteryTypeId.Value)))
            .OrderBy(x => x.StationName)
            .ToListAsync(cancellationToken);
    }
}

