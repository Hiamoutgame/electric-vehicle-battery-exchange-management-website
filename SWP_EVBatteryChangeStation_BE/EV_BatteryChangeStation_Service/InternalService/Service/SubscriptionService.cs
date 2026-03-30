using EV_BatteryChangeStation_Common.DTOs.SubscriptionDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionService(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> GetAllAsync()
    {
        var plans = await _unitOfWork.SubscriptionRepository.GetPlansAsync("ALL");
        return ServiceResponse.Ok("Subscription plans retrieved successfully.", plans.Select(x => x.ToDto()).ToList());
    }

    public async Task<ServiceResult> GetByIdAsync(Guid id)
    {
        var plan = await _unitOfWork.SubscriptionRepository.GetPlanByIdAsync(id);
        return plan is null
            ? ServiceResponse.NotFound("Subscription plan not found.")
            : ServiceResponse.Ok("Subscription plan retrieved successfully.", plan.ToDto());
    }

    public async Task<ServiceResult> CreateAsync(SubscriptionCreateUpdateDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return ServiceResponse.BadRequest("Plan name is required.");
        }

        var plan = new SubscriptionPlan
        {
            PlanId = Guid.NewGuid(),
            PlanCode = dto.Name.Trim().ToUpperInvariant().Replace(" ", "_"),
            PlanName = dto.Name.Trim(),
            BasePrice = dto.Price,
            ExtraFeePerSwap = dto.ExtraFee,
            Description = dto.Description?.Trim(),
            DurationDays = dto.DurationPackage ?? 30,
            Status = "ACTIVE",
            CreateDate = DateTime.UtcNow
        };

        _context.SubscriptionPlans.Add(plan);
        await _context.SaveChangesAsync();
        return ServiceResponse.Created("Subscription plan created successfully.", plan.ToDto());
    }

    public async Task<ServiceResult> UpdateAsync(Guid id, SubscriptionCreateUpdateDTO dto)
    {
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(x => x.PlanId == id);
        if (plan is null)
        {
            return ServiceResponse.NotFound("Subscription plan not found.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            plan.PlanName = dto.Name.Trim();
            plan.PlanCode = dto.Name.Trim().ToUpperInvariant().Replace(" ", "_");
        }

        plan.BasePrice = dto.Price;
        plan.ExtraFeePerSwap = dto.ExtraFee;
        plan.Description = dto.Description?.Trim();
        plan.DurationDays = dto.DurationPackage ?? plan.DurationDays;
        plan.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Subscription plan updated successfully.", plan.ToDto());
    }

    public async Task<ServiceResult> SoftDeleteAsync(Guid id)
    {
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(x => x.PlanId == id);
        if (plan is null)
        {
            return ServiceResponse.NotFound("Subscription plan not found.");
        }

        plan.Status = "INACTIVE";
        plan.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Subscription plan deactivated successfully.");
    }

    public async Task<ServiceResult> HardDeleteAsync(Guid id)
    {
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(x => x.PlanId == id);
        if (plan is null)
        {
            return ServiceResponse.NotFound("Subscription plan not found.");
        }

        _context.SubscriptionPlans.Remove(plan);
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Subscription plan deleted successfully.");
    }

    public async Task<ServiceResult> RestoreAsync(Guid id)
    {
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(x => x.PlanId == id);
        if (plan is null)
        {
            return ServiceResponse.NotFound("Subscription plan not found.");
        }

        plan.Status = "ACTIVE";
        plan.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Subscription plan restored successfully.", plan.ToDto());
    }

    public async Task<ServiceResult> GetActiveByAccountIdAsync(Guid accountId)
    {
        var subscription = await _unitOfWork.SubscriptionRepository.GetActiveByAccountAsync(accountId);
        return subscription is null
            ? ServiceResponse.NotFound("Active subscription not found.")
            : ServiceResponse.Ok("Active subscription retrieved successfully.", subscription.ToActiveSubscriptionDto());
    }
}
