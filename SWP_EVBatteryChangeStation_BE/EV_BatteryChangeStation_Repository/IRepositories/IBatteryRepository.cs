using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.Models;

namespace EV_BatteryChangeStation_Repository.IRepositories;

public interface IBatteryRepository
{
    Task<Battery?> GetByIdAsync(Guid batteryId, CancellationToken cancellationToken = default);
    Task<Battery?> GetBestAvailableBatteryAsync(Guid stationId, Guid batteryTypeId, CancellationToken cancellationToken = default);
    Task<bool> HasAvailableCompatibleBatteryAsync(Guid stationId, Guid batteryTypeId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, int>> GetAvailableCountsByStationAsync(Guid? batteryTypeId = null, CancellationToken cancellationToken = default);
    Task<List<Battery>> GetStationInventoryAsync(Guid stationId, string? status = null, Guid? batteryTypeId = null, CancellationToken cancellationToken = default);
    Task<List<StationInventorySummary>> GetInventorySummaryAsync(Guid? stationId = null, Guid? batteryTypeId = null, CancellationToken cancellationToken = default);
}

