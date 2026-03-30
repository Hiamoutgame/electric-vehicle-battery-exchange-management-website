using EV_BatteryChangeStation_Repository.Entities;

namespace EV_BatteryChangeStation_Repository.IRepositories;

public interface ISubscriptionRepository
{
    Task<UserSubscription?> GetActiveByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<UserSubscription?> GetActiveByVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<UserSubscription?> GetActiveForBookingAsync(Guid accountId, Guid vehicleId, DateTime targetTime, CancellationToken cancellationToken = default);
    Task<SubscriptionPlan?> GetPlanByIdAsync(Guid planId, CancellationToken cancellationToken = default);
    Task<List<SubscriptionPlan>> GetPlansAsync(string? status = null, CancellationToken cancellationToken = default);
}


