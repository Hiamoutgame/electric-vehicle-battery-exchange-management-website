using EV_BatteryChangeStation_Repository.Entities;

namespace EV_BatteryChangeStation_Repository.IRepositories;

public interface IStationRepository
{
    Task<List<Station>> GetActiveStationsAsync(string? keyword = null, string? area = null, Guid? batteryTypeId = null, CancellationToken cancellationToken = default);
    Task<List<Station>> GetStationsForAdminAsync(string? status = null, string? keyword = null, CancellationToken cancellationToken = default);
    Task<Station?> GetStationDetailAsync(Guid stationId, CancellationToken cancellationToken = default);
    Task<Station?> GetByIdForManagementAsync(Guid stationId, CancellationToken cancellationToken = default);
    Task<Guid?> GetAssignedStationIdAsync(Guid staffId, DateOnly? effectiveDate = null, CancellationToken cancellationToken = default);
}

