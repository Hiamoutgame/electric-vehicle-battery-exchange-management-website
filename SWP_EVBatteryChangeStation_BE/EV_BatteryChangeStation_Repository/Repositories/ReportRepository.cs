using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.IRepositories;
using EV_BatteryChangeStation_Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories;

public sealed class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RevenueReportItem>> GetRevenueReportAsync(DateTime fromDate, DateTime toDate, string groupBy = "day", Guid? stationId = null, CancellationToken cancellationToken = default)
    {
        var normalizedGroupBy = string.IsNullOrWhiteSpace(groupBy) ? "day" : groupBy.Trim().ToLowerInvariant();

        IQueryable<Entities.Payment> payments = _context.Payments
            .AsNoTracking()
            .Where(x => x.Status == "PAID"
                        && x.PaidAt.HasValue
                        && x.PaidAt.Value >= fromDate
                        && x.PaidAt.Value <= toDate);

        if (stationId.HasValue)
        {
            payments = payments.Where(x =>
                (x.Transaction != null && x.Transaction.StationId == stationId.Value) ||
                (x.Booking != null && x.Booking.StationId == stationId.Value));
        }

        if (normalizedGroupBy == "month")
        {
            var monthlyRows = await payments
                .GroupBy(x => new { x.PaidAt!.Value.Year, x.PaidAt.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Revenue = g.Sum(x => x.Amount) })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync(cancellationToken);

            return monthlyRows
                .Select(x => new RevenueReportItem($"{x.Year:D4}-{x.Month:D2}", x.Revenue))
                .OrderBy(x => x.Label)
                .ToList();
        }

        if (normalizedGroupBy == "station")
        {
            return await payments
                .GroupBy(x => new
                {
                    StationId = x.TransactionId != null
                        ? (Guid?)x.Transaction!.StationId
                        : x.BookingId != null
                            ? (Guid?)x.Booking!.StationId
                            : null,
                    StationName = x.TransactionId != null
                        ? x.Transaction!.Station!.StationName
                        : x.BookingId != null
                            ? x.Booking!.Station!.StationName
                            : "SYSTEM"
                })
                .Select(g => new RevenueReportItem(
                    g.Key.StationName,
                    g.Sum(x => x.Amount),
                    g.Key.StationId,
                    g.Key.StationName))
                .OrderByDescending(x => x.Revenue)
                .ThenBy(x => x.Label)
                .ToListAsync(cancellationToken);
        }

        var dailyRows = await payments
            .GroupBy(x => x.PaidAt!.Value.Date)
            .Select(g => new { Date = g.Key, Revenue = g.Sum(x => x.Amount) })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        return dailyRows
            .Select(x => new RevenueReportItem(x.Date.ToString("yyyy-MM-dd"), x.Revenue))
            .ToList();
    }

    public Task<List<PeakHourDemandItem>> GetPeakHourDemandAsync(DateTime fromDate, DateTime toDate, Guid? stationId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Entities.Booking> bookings = _context.Bookings
            .AsNoTracking()
            .Where(x => x.TargetTime >= fromDate && x.TargetTime <= toDate);

        if (stationId.HasValue)
        {
            bookings = bookings.Where(x => x.StationId == stationId.Value);
        }

        return bookings
            .GroupBy(x => x.TargetTime.Hour)
            .Select(g => new PeakHourDemandItem(g.Key, g.Count()))
            .OrderByDescending(x => x.BookingCount)
            .ThenBy(x => x.Hour)
            .ToListAsync(cancellationToken);
    }

    public Task<List<StationSwapCountItem>> GetSwapCountByStationAsync(DateTime fromDate, DateTime toDate, Guid? stationId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Entities.SwappingTransaction> swaps = _context.SwappingTransactions
            .AsNoTracking()
            .Where(x => x.CreateDate >= fromDate && x.CreateDate <= toDate);

        if (stationId.HasValue)
        {
            swaps = swaps.Where(x => x.StationId == stationId.Value);
        }

        return swaps
            .GroupBy(x => new { x.StationId, x.Station!.StationName })
            .Select(g => new StationSwapCountItem(g.Key.StationId, g.Key.StationName, g.Count()))
            .OrderByDescending(x => x.SwapCount)
            .ThenBy(x => x.StationName)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DailySwapCountItem>> GetDailySwapCountsAsync(DateTime fromDate, DateTime toDate, Guid? stationId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Entities.SwappingTransaction> swaps = _context.SwappingTransactions
            .AsNoTracking()
            .Where(x => x.CreateDate >= fromDate && x.CreateDate <= toDate);

        if (stationId.HasValue)
        {
            swaps = swaps.Where(x => x.StationId == stationId.Value);
        }

        var rows = await swaps
            .GroupBy(x => new { Date = x.CreateDate.Date, x.StationId, x.Station!.StationName })
            .Select(g => new
            {
                g.Key.Date,
                g.Key.StationId,
                g.Key.StationName,
                SwapCount = g.Count()
            })
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StationName)
            .ToListAsync(cancellationToken);

        return rows
            .Select(x => new DailySwapCountItem(DateOnly.FromDateTime(x.Date), x.SwapCount, x.StationId, x.StationName))
            .ToList();
    }

    public Task<List<StationInventoryForecastInput>> GetLatestInventorySnapshotsAsync(Guid? stationId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Entities.StationInventoryLog> logs = _context.StationInventoryLogs.AsNoTracking();

        if (stationId.HasValue)
        {
            logs = logs.Where(x => x.StationId == stationId.Value);
        }

        var latestByStation = logs
            .GroupBy(x => x.StationId)
            .Select(g => new
            {
                StationId = g.Key,
                LogTime = g.Max(x => x.LogTime)
            });

        return (
            from log in logs
            join latest in latestByStation
                on new { log.StationId, log.LogTime } equals new { latest.StationId, latest.LogTime }
            select new StationInventoryForecastInput(
                log.StationId,
                log.Station!.StationName,
                log.LogTime,
                log.AvailableBatteries,
                log.ReservedBatteries,
                log.ChargingBatteries,
                log.InVehicleBatteries,
                log.MaintenanceBatteries,
                log.AvgChargeLevel))
            .OrderBy(x => x.StationName)
            .ToListAsync(cancellationToken);
    }
}

