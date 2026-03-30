using EV_BatteryChangeStation_Common.DTOs.SupportRequestDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class SupportRequestService : ISupportRequestService
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public SupportRequestService(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> GetAllAsync()
    {
        var requests = await _unitOfWork.SupportRequestRepository.GetForAdminAsync();
        return ServiceResponse.Ok("Support requests retrieved successfully.", requests.Select(x => x.ToDto()).ToList());
    }

    public async Task<ServiceResult> GetByIdAsync(Guid id)
    {
        var request = await _unitOfWork.SupportRequestRepository.GetByIdWithDetailsAsync(id);
        return request is null
            ? ServiceResponse.NotFound("Support request not found.")
            : ServiceResponse.Ok("Support request retrieved successfully.", new
            {
                request.RequestId,
                request.AccountId,
                request.StationId,
                request.BookingId,
                request.TransactionId,
                request.IssueType,
                request.Subject,
                request.Description,
                request.Priority,
                request.Status,
                request.CreateDate,
                responses = request.Responses
                    .OrderByDescending(x => x.RespondedAt)
                    .Select(x => new
                    {
                        x.ResponseId,
                        x.StaffId,
                        x.ResponseMessage,
                        x.StatusAfterResponse,
                        x.RespondedAt
                    })
            });
    }

    public async Task<ServiceResult> CreateAsync(SupportRequestCreateDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.IssueType) || string.IsNullOrWhiteSpace(dto.Description))
        {
            return ServiceResponse.BadRequest("Issue type and description are required.");
        }

        var latestBooking = await _context.Bookings
            .AsNoTracking()
            .Where(x => x.AccountId == dto.AccountId)
            .OrderByDescending(x => x.CreateDate)
            .FirstOrDefaultAsync();

        var request = new SupportRequest
        {
            RequestId = Guid.NewGuid(),
            AccountId = dto.AccountId,
            StationId = latestBooking?.StationId,
            BookingId = latestBooking?.BookingId,
            IssueType = dto.IssueType.Trim().ToUpperInvariant(),
            Subject = dto.IssueType.Trim(),
            Description = dto.Description.Trim(),
            Priority = "MEDIUM",
            Status = "OPEN",
            CreateDate = DateTime.UtcNow
        };

        _context.SupportRequests.Add(request);
        await _context.SaveChangesAsync();
        return ServiceResponse.Created("Support request created successfully.", request.ToDto());
    }

    public async Task<ServiceResult> UpdateAsync(Guid id, SupportRequestUpdateDTO dto)
    {
        var request = await _context.SupportRequests
            .Include(x => x.Responses)
            .FirstOrDefaultAsync(x => x.RequestId == id);

        if (request is null)
        {
            return ServiceResponse.NotFound("Support request not found.");
        }

        if (dto.StaffId.HasValue && dto.StaffId.Value != Guid.Empty)
        {
            if (string.IsNullOrWhiteSpace(dto.ResponseText))
            {
                return ServiceResponse.BadRequest("Response text is required.");
            }

            _context.SupportRequestResponses.Add(new SupportRequestResponse
            {
                ResponseId = Guid.NewGuid(),
                RequestId = request.RequestId,
                StaffId = dto.StaffId.Value,
                ResponseMessage = dto.ResponseText.Trim(),
                StatusAfterResponse = "RESOLVED",
                RespondedAt = DateTime.UtcNow
            });

            request.Status = "RESOLVED";
            request.ClosedDate = DateTime.UtcNow;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(dto.IssueType))
            {
                request.IssueType = dto.IssueType.Trim().ToUpperInvariant();
                request.Subject = dto.IssueType.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                request.Description = dto.Description.Trim();
            }
        }

        request.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        request = await _context.SupportRequests
            .AsNoTracking()
            .Include(x => x.Responses)
            .FirstAsync(x => x.RequestId == id);

        return ServiceResponse.Ok("Support request updated successfully.", request.ToDto());
    }

    public async Task<ServiceResult> SoftDeleteAsync(Guid id)
    {
        var request = await _context.SupportRequests.FirstOrDefaultAsync(x => x.RequestId == id);
        if (request is null)
        {
            return ServiceResponse.NotFound("Support request not found.");
        }

        request.Status = "CLOSED";
        request.ClosedDate = DateTime.UtcNow;
        request.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Support request closed successfully.");
    }

    public async Task<ServiceResult> HardDeleteAsync(Guid id)
    {
        var request = await _context.SupportRequests.FirstOrDefaultAsync(x => x.RequestId == id);
        if (request is null)
        {
            return ServiceResponse.NotFound("Support request not found.");
        }

        _context.SupportRequests.Remove(request);
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Support request deleted successfully.");
    }

    public async Task<ServiceResult> GetByAccountIdAsync(Guid accountId)
    {
        var requests = await _unitOfWork.SupportRequestRepository.GetByAccountAsync(accountId);
        return ServiceResponse.Ok("Support requests retrieved successfully.", requests.Select(x => x.ToDto()).ToList());
    }

    public async Task<ServiceResult> GetByStaffIdAsync(Guid staffId)
    {
        var stationId = await _unitOfWork.StationRepository.GetAssignedStationIdAsync(staffId);
        if (!stationId.HasValue)
        {
            return ServiceResponse.NotFound("Staff is not assigned to any station.");
        }

        var requests = await _unitOfWork.SupportRequestRepository.GetByStationAsync(stationId.Value);
        return ServiceResponse.Ok("Support requests retrieved successfully.", requests.Select(x => x.ToDto()).ToList());
    }
}
