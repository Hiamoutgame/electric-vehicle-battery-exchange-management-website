using EV_BatteryChangeStation_Repository.Entities;

namespace EV_BatteryChangeStation_Repository.IRepositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdWithDetailsAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<Booking?> GetDriverBookingByIdAsync(Guid bookingId, Guid accountId, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetBookingsByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetBookingsByStationAsync(Guid stationId, string? status = null, DateTime? targetDate = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsConflictingBookingAsync(Guid stationId, Guid vehicleId, DateTime targetTime, CancellationToken cancellationToken = default);
}

