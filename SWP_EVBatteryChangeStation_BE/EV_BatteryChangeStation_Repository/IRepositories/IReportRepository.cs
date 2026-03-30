using EV_BatteryChangeStation_Repository.Models;

namespace EV_BatteryChangeStation_Repository.IRepositories;

public interface IReportRepository
{
    Task<List<RevenueReportItem>> GetRevenueReportAsync(DateTime fromDate, DateTime toDate, string groupBy = "day", Guid? stationId = null, CancellationToken cancellationToken = default);
    Task<List<PeakHourDemandItem>> GetPeakHourDemandAsync(DateTime fromDate, DateTime toDate, Guid? stationId = null, CancellationToken cancellationToken = default);
    Task<List<StationSwapCountItem>> GetSwapCountByStationAsync(DateTime fromDate, DateTime toDate, Guid? stationId = null, CancellationToken cancellationToken = default);
    Task<List<DailySwapCountItem>> GetDailySwapCountsAsync(DateTime fromDate, DateTime toDate, Guid? stationId = null, CancellationToken cancellationToken = default);
    Task<List<StationInventoryForecastInput>> GetLatestInventorySnapshotsAsync(Guid? stationId = null, CancellationToken cancellationToken = default);
}


