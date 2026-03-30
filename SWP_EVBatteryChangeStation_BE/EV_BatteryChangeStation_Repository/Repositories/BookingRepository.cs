using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories;

public sealed class BookingRepository : IBookingRepository
{
    private static readonly string[] ActiveStatuses = ["PENDING", "APPROVED"];

    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Booking?> GetByIdWithDetailsAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return _context.Bookings
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Account)
            .Include(x => x.Vehicle)
                .ThenInclude(x => x!.Model)
                    .ThenInclude(x => x!.BatteryType)
            .Include(x => x.Station)
            .Include(x => x.RequestedBatteryType)
            .Include(x => x.ApprovedByAccount)
            .Include(x => x.SwappingTransaction)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId, cancellationToken);
    }

    public Task<Booking?> GetDriverBookingByIdAsync(Guid bookingId, Guid accountId, CancellationToken cancellationToken = default)
    {
        return _context.Bookings
            .AsNoTracking()
            .Include(x => x.Station)
            .Include(x => x.RequestedBatteryType)
            .Include(x => x.SwappingTransaction)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId && x.AccountId == accountId, cancellationToken);
    }

    public Task<List<Booking>> GetBookingsByAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return _context.Bookings
            .AsNoTracking()
            .Include(x => x.Station)
            .Include(x => x.RequestedBatteryType)
            .Include(x => x.SwappingTransaction)
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.TargetTime)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Booking>> GetBookingsByStationAsync(Guid stationId, string? status = null, DateTime? targetDate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Booking> query = _context.Bookings
            .AsNoTracking()
            .Include(x => x.Account)
            .Include(x => x.Vehicle)
            .Include(x => x.RequestedBatteryType)
            .Where(x => x.StationId == stationId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            query = query.Where(x => x.Status == normalizedStatus);
        }

        if (targetDate.HasValue)
        {
            var from = targetDate.Value.Date;
            var to = from.AddDays(1);
            query = query.Where(x => x.TargetTime >= from && x.TargetTime < to);
        }

        return query.OrderBy(x => x.TargetTime).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsConflictingBookingAsync(Guid stationId, Guid vehicleId, DateTime targetTime, CancellationToken cancellationToken = default)
    {
        var slotStart = targetTime.AddMinutes(-30);
        var slotEnd = targetTime.AddMinutes(30);

        return _context.Bookings
            .AsNoTracking()
            .AnyAsync(x => x.StationId == stationId
                           && x.VehicleId == vehicleId
                           && ActiveStatuses.Contains(x.Status)
                           && x.TargetTime >= slotStart
                           && x.TargetTime <= slotEnd,
                cancellationToken);
    }
}

