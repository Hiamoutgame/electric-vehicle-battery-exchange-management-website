using EV_BatteryChangeStation_Repository.Entities;

namespace EV_BatteryChangeStation_Repository.IRepositories;

public interface ISupportRequestRepository
{
    Task<SupportRequest?> GetByIdWithDetailsAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<List<SupportRequest>> GetByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<List<SupportRequest>> GetByStationAsync(Guid stationId, string? status = null, string? issueType = null, CancellationToken cancellationToken = default);
    Task<List<SupportRequest>> GetForAdminAsync(string? status = null, Guid? stationId = null, string? issueType = null, CancellationToken cancellationToken = default);
}


