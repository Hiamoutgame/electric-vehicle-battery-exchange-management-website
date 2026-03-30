using EV_BatteryChangeStation_Common.DTOs.BookingDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class BookingService : IBookingService
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> GetAllAsync()
    {
        var bookings = await _context.Bookings
            .AsNoTracking()
            .OrderByDescending(x => x.TargetTime)
            .ToListAsync();

        return ServiceResponse.Ok("Bookings retrieved successfully.", bookings.Select(x => x.ToDto()).ToList());
    }

    public async Task<ServiceResult> GetByIdAsync(Guid id)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdWithDetailsAsync(id);
        return booking is null
            ? ServiceResponse.NotFound("Booking not found.")
            : ServiceResponse.Ok("Booking retrieved successfully.", booking.ToDetailDto());
    }

    public async Task<ServiceResult> GetByIdForAccountAsync(Guid bookingId, Guid accountId)
    {
        var booking = await _unitOfWork.BookingRepository.GetDriverBookingByIdAsync(bookingId, accountId);
        return booking is null
            ? ServiceResponse.NotFound("Booking not found.")
            : ServiceResponse.Ok("Booking retrieved successfully.", booking.ToDetailDto());
    }

    public async Task<ServiceResult> CreateAsync(BookingCreateDTO dto)
    {
        var account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == dto.AccountId);
        if (account is null)
        {
            return ServiceResponse.NotFound("Account not found.");
        }

        var vehicle = await _unitOfWork.VehicleRepository.GetOwnedVehicleAsync(dto.VehicleId, dto.AccountId);
        if (vehicle?.Model?.BatteryTypeId is not Guid requestedBatteryTypeId)
        {
            return ServiceResponse.NotFound("Vehicle or compatible battery type not found.");
        }

        var stationExists = await _context.Stations.AnyAsync(x => x.StationId == dto.StationId && x.Status == "ACTIVE");
        if (!stationExists)
        {
            return ServiceResponse.NotFound("Station not found.");
        }

        var hasBattery = await _unitOfWork.BatteryRepository.HasAvailableCompatibleBatteryAsync(dto.StationId, requestedBatteryTypeId);
        if (!hasBattery)
        {
            return ServiceResponse.Conflict("No compatible battery available at this station.");
        }

        var hasConflict = await _unitOfWork.BookingRepository.ExistsConflictingBookingAsync(dto.StationId, dto.VehicleId, dto.DateTime);
        if (hasConflict)
        {
            return ServiceResponse.Conflict("This vehicle already has a nearby booking at the selected time.");
        }

        var subscription = await _unitOfWork.SubscriptionRepository.GetActiveForBookingAsync(dto.AccountId, dto.VehicleId, dto.DateTime);
        if (subscription is null)
        {
            return ServiceResponse.Forbidden("An active subscription is required for booking.");
        }

        var booking = new Booking
        {
            BookingId = Guid.NewGuid(),
            AccountId = dto.AccountId,
            VehicleId = dto.VehicleId,
            StationId = dto.StationId,
            RequestedBatteryTypeId = requestedBatteryTypeId,
            TargetTime = dto.DateTime,
            Status = string.IsNullOrWhiteSpace(dto.IsApproved) ? "PENDING" : dto.IsApproved.Trim().ToUpperInvariant(),
            Notes = dto.Notes?.Trim(),
            CreateDate = dto.CreatedDate ?? DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return ServiceResponse.Created("Booking created successfully.", booking.ToDto());
    }

    public async Task<ServiceResult> UpdateAsync(Guid id, BookingCreateDTO dto)
    {
        var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);
        if (booking is null)
        {
            return ServiceResponse.NotFound("Booking not found.");
        }

        if (booking.Status == "COMPLETED")
        {
            return ServiceResponse.BadRequest("Completed bookings cannot be updated.");
        }

        booking.TargetTime = dto.DateTime;
        booking.Notes = dto.Notes?.Trim();
        booking.UpdateDate = DateTime.UtcNow;
        if (dto.StationId != Guid.Empty)
        {
            booking.StationId = dto.StationId;
        }

        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Booking updated successfully.", booking.ToDto());
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);
        if (booking is null)
        {
            return ServiceResponse.NotFound("Booking not found.");
        }

        booking.Status = "CANCELLED";
        booking.CancelledDate = DateTime.UtcNow;
        booking.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Booking cancelled successfully.");
    }

    public async Task<ServiceResult> HardDeleteAsync(Guid id)
    {
        var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);
        if (booking is null)
        {
            return ServiceResponse.NotFound("Booking not found.");
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Booking deleted successfully.");
    }

    public async Task<ServiceResult> GetByAccountIdAsync(Guid accountId)
    {
        var bookings = await _unitOfWork.BookingRepository.GetBookingsByAccountAsync(accountId);
        return ServiceResponse.Ok("Bookings retrieved successfully.", bookings.Select(x => x.ToDetailDto()).ToList());
    }

    public async Task<ServiceResult> GetByStaffStationAsync(Guid staffAccountId)
    {
        var stationId = await _unitOfWork.StationRepository.GetAssignedStationIdAsync(staffAccountId);
        if (!stationId.HasValue)
        {
            return ServiceResponse.NotFound("Staff is not assigned to any station.");
        }

        var bookings = await _unitOfWork.BookingRepository.GetBookingsByStationAsync(stationId.Value);
        return ServiceResponse.Ok("Station bookings retrieved successfully.", bookings.Select(x => x.ToDetailDto()).ToList());
    }

    public async Task<ServiceResult> UpdateBookingStatusAsync(Guid bookingId, string status, Guid staffId, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return ServiceResponse.BadRequest("Status is required.");
        }

        var normalizedStatus = status.Trim().ToUpperInvariant();
        if (normalizedStatus is not ("APPROVED" or "REJECTED"))
        {
            return ServiceResponse.BadRequest("Status must be Approved or Rejected.");
        }

        var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == bookingId);
        if (booking is null)
        {
            return ServiceResponse.NotFound("Booking not found.");
        }

        var stationId = await _unitOfWork.StationRepository.GetAssignedStationIdAsync(staffId);
        if (!stationId.HasValue || stationId.Value != booking.StationId)
        {
            return ServiceResponse.Forbidden("You can only manage bookings at your assigned station.");
        }

        booking.Status = normalizedStatus;
        booking.StaffNote = notes?.Trim();
        booking.ApprovedBy = normalizedStatus == "APPROVED" ? staffId : null;
        booking.ApprovedDate = normalizedStatus == "APPROVED" ? DateTime.UtcNow : null;
        booking.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Booking status updated successfully.", booking.ToDto());
    }
}
