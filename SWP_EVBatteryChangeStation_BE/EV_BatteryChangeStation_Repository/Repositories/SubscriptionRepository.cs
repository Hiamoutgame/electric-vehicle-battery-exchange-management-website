using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _context;

    public SubscriptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<UserSubscription?> GetActiveByAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return _context.UserSubscriptions
            .AsNoTracking()
            .Include(x => x.Plan)
            .Include(x => x.Vehicle)
                .ThenInclude(x => x!.Model)
                    .ThenInclude(x => x!.BatteryType)
            .Where(x => x.AccountId == accountId
                        && x.Status == "ACTIVE"
                        && x.StartDate <= now
                        && x.EndDate >= now)
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<UserSubscription?> GetActiveByVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return _context.UserSubscriptions
            .AsNoTracking()
            .Include(x => x.Plan)
            .Include(x => x.Account)
            .Where(x => x.VehicleId == vehicleId
                        && x.Status == "ACTIVE"
                        && x.StartDate <= now
                        && x.EndDate >= now)
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<UserSubscription?> GetActiveForBookingAsync(Guid accountId, Guid vehicleId, DateTime targetTime, CancellationToken cancellationToken = default)
    {
        return _context.UserSubscriptions
            .AsNoTracking()
            .Include(x => x.Plan)
            .Where(x => x.AccountId == accountId
                        && x.VehicleId == vehicleId
                        && x.Status == "ACTIVE"
                        && x.StartDate <= targetTime
                        && x.EndDate >= targetTime
                        && (!x.RemainingSwaps.HasValue || x.RemainingSwaps > 0))
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<SubscriptionPlan?> GetPlanByIdAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        return _context.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PlanId == planId, cancellationToken);
    }

    public Task<List<SubscriptionPlan>> GetPlansAsync(string? status = null, CancellationToken cancellationToken = default)
    {
        IQueryable<SubscriptionPlan> query = _context.SubscriptionPlans.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status) && !string.Equals(status.Trim(), "ALL", StringComparison.OrdinalIgnoreCase))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            query = query.Where(x => x.Status.ToUpper() == normalizedStatus);
        }

        return query
            .OrderBy(x => x.PlanName)
            .ToListAsync(cancellationToken);
    }
}

