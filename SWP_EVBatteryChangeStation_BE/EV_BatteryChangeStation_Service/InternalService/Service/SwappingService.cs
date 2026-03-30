using EV_BatteryChangeStation_Common.DTOs.SwappingtransactionDto;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class SwappingService : ISwappingService
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public SwappingService(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult> GetAllTransactionsAsync()
    {
        var transactions = await _context.SwappingTransactions
            .AsNoTracking()
            .OrderByDescending(x => x.CreateDate)
            .ToListAsync();

        return ServiceResponse.Ok("Swap transactions retrieved successfully.", transactions.Select(x => x.ToDto()).ToList());
    }

    public async Task<IServiceResult> GetTransactionByIdAsync(Guid transactionId)
    {
        var transaction = await _unitOfWork.SwapRepository.GetByIdWithDetailsAsync(transactionId);
        return transaction is null
            ? ServiceResponse.NotFound("Swap transaction not found.")
            : ServiceResponse.Ok("Swap transaction retrieved successfully.", transaction.ToDto());
    }

    public Task<IServiceResult> CreateTransactionAsync(CreateSwappingDto createSwappingDto)
    {
        return Task.FromResult<IServiceResult>(ServiceResponse.BadRequest("Use ConfirmAndSwapAsync for swap completion."));
    }

    public async Task<IServiceResult> UpdateTransactionAsync(UpdateSwappingDto updateSwappingDto)
    {
        var transaction = await _context.SwappingTransactions.FirstOrDefaultAsync(x => x.TransactionId == updateSwappingDto.TransactionId);
        if (transaction is null)
        {
            return ServiceResponse.NotFound("Swap transaction not found.");
        }

        transaction.Notes = updateSwappingDto.Notes?.Trim();
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Swap transaction updated successfully.", transaction.ToDto());
    }

    public async Task<IServiceResult> DeleteTransactionAsync(Guid transactionId)
    {
        var transaction = await _context.SwappingTransactions.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        if (transaction is null)
        {
            return ServiceResponse.NotFound("Swap transaction not found.");
        }

        _context.SwappingTransactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Swap transaction deleted successfully.");
    }

    public Task<IServiceResult> SoftDeleteTransactionAsync(Guid transactionid)
    {
        return Task.FromResult<IServiceResult>(ServiceResponse.BadRequest("Soft delete is not supported for swap transactions."));
    }

    public async Task<IServiceResult> GetTransactionByCarIdAsync(Guid carid)
    {
        var transactions = await _unitOfWork.SwapRepository.GetHistoryByVehicleAsync(carid);
        return ServiceResponse.Ok("Swap history retrieved successfully.", transactions.Select(x => x.ToDto()).ToList());
    }

    public async Task<IServiceResult> ConfirmAndSwapAsync(ConfirmSwapDTO dto)
    {
        var booking = await _context.Bookings
            .Include(x => x.Vehicle)
                .ThenInclude(x => x!.Model)
                    .ThenInclude(x => x!.BatteryType)
            .FirstOrDefaultAsync(x => x.BookingId == dto.BookingId);

        if (booking?.Vehicle?.Model?.BatteryTypeId is not Guid batteryTypeId)
        {
            return ServiceResponse.NotFound("Booking, vehicle or battery type not found.");
        }

        if (booking.Status != "APPROVED")
        {
            return ServiceResponse.BadRequest("Only approved bookings can be completed.");
        }

        var staffStationId = await _unitOfWork.StationRepository.GetAssignedStationIdAsync(dto.StaffId);
        if (!staffStationId.HasValue || staffStationId.Value != booking.StationId)
        {
            return ServiceResponse.Forbidden("You can only complete swaps at your assigned station.");
        }

        var existingSwap = await _unitOfWork.SwapRepository.ExistsForBookingAsync(dto.BookingId);
        if (existingSwap)
        {
            return ServiceResponse.Conflict("This booking has already been completed.");
        }

        if (booking.Vehicle.CurrentBatteryId is not Guid returnedBatteryId)
        {
            return ServiceResponse.BadRequest("Vehicle does not have a current battery to return.");
        }

        var releasedBattery = await _unitOfWork.BatteryRepository.GetBestAvailableBatteryAsync(booking.StationId, batteryTypeId);
        if (releasedBattery is null)
        {
            return ServiceResponse.NotFound("No compatible released battery is available.");
        }

        await using var transactionScope = await _unitOfWork.BeginTransactionAsync();

        var returnedBattery = await _context.Batteries.FirstAsync(x => x.BatteryId == returnedBatteryId);
        var trackedReleasedBattery = await _context.Batteries.FirstAsync(x => x.BatteryId == releasedBattery.BatteryId);
        var vehicle = await _context.Vehicles.FirstAsync(x => x.VehicleId == booking.VehicleId);
        var subscription = await _unitOfWork.SubscriptionRepository.GetActiveForBookingAsync(booking.AccountId, booking.VehicleId, DateTime.UtcNow);

        var swap = new SwappingTransaction
        {
            TransactionId = Guid.NewGuid(),
            BookingId = booking.BookingId,
            VehicleId = booking.VehicleId,
            StaffId = dto.StaffId,
            StationId = booking.StationId,
            ReturnedBatteryId = returnedBattery.BatteryId,
            ReturnedBatterySoH = returnedBattery.StateOfHealth,
            ReturnedBatteryCharge = returnedBattery.CurrentChargeLevel,
            ReturnedBatteryCondition = returnedBattery.Status,
            ReleasedBatteryId = trackedReleasedBattery.BatteryId,
            ReleasedBatterySoH = trackedReleasedBattery.StateOfHealth,
            ReleasedBatteryCharge = trackedReleasedBattery.CurrentChargeLevel,
            SwapFee = subscription?.Plan?.ExtraFeePerSwap ?? 0m,
            UsedSubscription = subscription is not null,
            Notes = dto.Notes?.Trim(),
            CreateDate = DateTime.UtcNow
        };

        _context.SwappingTransactions.Add(swap);

        booking.Status = "COMPLETED";
        booking.UpdateDate = DateTime.UtcNow;
        booking.StaffNote = dto.Notes?.Trim();

        vehicle.CurrentBatteryId = trackedReleasedBattery.BatteryId;
        vehicle.UpdateDate = DateTime.UtcNow;

        returnedBattery.StationId = booking.StationId;
        returnedBattery.Status = "CHARGING";
        returnedBattery.LastUsedAt = DateTime.UtcNow;
        returnedBattery.UpdateDate = DateTime.UtcNow;

        trackedReleasedBattery.StationId = null;
        trackedReleasedBattery.Status = "IN_VEHICLE";
        trackedReleasedBattery.LastUsedAt = DateTime.UtcNow;
        trackedReleasedBattery.UpdateDate = DateTime.UtcNow;

        _context.BatteryHistories.Add(new BatteryHistory
        {
            HistoryId = Guid.NewGuid(),
            BatteryId = returnedBattery.BatteryId,
            FromVehicleId = vehicle.VehicleId,
            ToStationId = booking.StationId,
            ActionType = "RETURN",
            FromStatus = "IN_VEHICLE",
            ToStatus = returnedBattery.Status,
            EventDate = DateTime.UtcNow,
            SoHAtTime = returnedBattery.StateOfHealth,
            ChargeLevelAtTime = returnedBattery.CurrentChargeLevel,
            Note = dto.Notes,
            ActorAccountId = dto.StaffId
        });

        _context.BatteryHistories.Add(new BatteryHistory
        {
            HistoryId = Guid.NewGuid(),
            BatteryId = trackedReleasedBattery.BatteryId,
            FromStationId = booking.StationId,
            ToVehicleId = vehicle.VehicleId,
            ActionType = "RELEASE",
            FromStatus = "AVAILABLE",
            ToStatus = trackedReleasedBattery.Status,
            EventDate = DateTime.UtcNow,
            SoHAtTime = trackedReleasedBattery.StateOfHealth,
            ChargeLevelAtTime = trackedReleasedBattery.CurrentChargeLevel,
            Note = dto.Notes,
            ActorAccountId = dto.StaffId
        });

        if (subscription?.RemainingSwaps is > 0)
        {
            subscription.RemainingSwaps -= 1;
            _context.UserSubscriptions.Update(subscription);
        }

        await _unitOfWork.CommitAsync();
        await transactionScope.CommitAsync();

        return ServiceResponse.Ok("Swap completed successfully.", swap.ToDto());
    }
}
