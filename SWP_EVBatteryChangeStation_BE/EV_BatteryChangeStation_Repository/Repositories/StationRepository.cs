using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories;

public sealed class StationRepository : IStationRepository
{
    private readonly AppDbContext _context;

    public StationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Station>> GetActiveStationsAsync(string? keyword = null, string? area = null, Guid? batteryTypeId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Station> query = _context.Stations
            .AsNoTracking()
            .Include(x => x.StationBatteryTypes)
                .ThenInclude(x => x.BatteryType)
            .Where(x => x.Status == "ACTIVE");

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var pattern = $"%{keyword.Trim().ToUpperInvariant()}%";
            query = query.Where(x =>
                EF.Functions.Like(x.StationName.ToUpper(), pattern) ||
                EF.Functions.Like(x.Address.ToUpper(), pattern) ||
                (x.Area != null && EF.Functions.Like(x.Area.ToUpper(), pattern)));
        }

        if (!string.IsNullOrWhiteSpace(area))
        {
            var areaPattern = $"%{area.Trim().ToUpperInvariant()}%";
            query = query.Where(x => x.Area != null && EF.Functions.Like(x.Area.ToUpper(), areaPattern));
        }

        if (batteryTypeId.HasValue)
        {
            query = query.Where(x => x.StationBatteryTypes.Any(sbt =>
                sbt.BatteryTypeId == batteryTypeId.Value && sbt.Status == "ACTIVE"));
        }

        return await query.OrderBy(x => x.StationName).ToListAsync(cancellationToken);
    }

    public Task<List<Station>> GetStationsForAdminAsync(string? status = null, string? keyword = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Station> query = _context.Stations
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.StationBatteryTypes)
                .ThenInclude(x => x.BatteryType)
            .Include(x => x.StaffAssignments)
            .Include(x => x.Batteries);

        if (!string.IsNullOrWhiteSpace(status) && !string.Equals(status.Trim(), "ALL", StringComparison.OrdinalIgnoreCase))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            query = query.Where(x => x.Status.ToUpper() == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var pattern = $"%{keyword.Trim().ToUpperInvariant()}%";
            query = query.Where(x =>
                EF.Functions.Like(x.StationName.ToUpper(), pattern) ||
                EF.Functions.Like(x.Address.ToUpper(), pattern) ||
                (x.Area != null && EF.Functions.Like(x.Area.ToUpper(), pattern)));
        }

        return query
            .OrderBy(x => x.StationName)
            .ToListAsync(cancellationToken);
    }

    public Task<Station?> GetStationDetailAsync(Guid stationId, CancellationToken cancellationToken = default)
    {
        return _context.Stations
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.StationBatteryTypes)
                .ThenInclude(x => x.BatteryType)
            .Include(x => x.Batteries)
            .FirstOrDefaultAsync(x => x.StationId == stationId, cancellationToken);
    }

    public Task<Station?> GetByIdForManagementAsync(Guid stationId, CancellationToken cancellationToken = default)
    {
        return _context.Stations
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.StationBatteryTypes)
                .ThenInclude(x => x.BatteryType)
            .Include(x => x.StaffAssignments)
                .ThenInclude(x => x.Staff)
            .Include(x => x.Batteries)
            .FirstOrDefaultAsync(x => x.StationId == stationId, cancellationToken);
    }

    public Task<Guid?> GetAssignedStationIdAsync(Guid staffId, DateOnly? effectiveDate = null, CancellationToken cancellationToken = default)
    {
        var targetDate = effectiveDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        return _context.StationStaffAssignments
            .AsNoTracking()
            .Where(x => x.StaffId == staffId
                        && x.Status == "ACTIVE"
                        && x.EffectiveFrom <= targetDate
                        && (x.EffectiveTo == null || x.EffectiveTo >= targetDate))
            .OrderByDescending(x => x.EffectiveFrom)
            .Select(x => (Guid?)x.StationId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

