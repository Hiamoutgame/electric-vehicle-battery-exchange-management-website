using EV_BatteryChangeStation_Repository.Entities;

namespace EV_BatteryChangeStation_Repository.IRepositories;

public interface ISwapRepository
{
    Task<SwappingTransaction?> GetByIdWithDetailsAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task<SwappingTransaction?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<List<SwappingTransaction>> GetHistoryByVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<List<SwappingTransaction>> GetHistoryByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForBookingAsync(Guid bookingId, CancellationToken cancellationToken = default);
}


