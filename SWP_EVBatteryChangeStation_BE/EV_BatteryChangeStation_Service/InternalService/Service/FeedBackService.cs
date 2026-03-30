using EV_BatteryChangeStation_Common.DTOs.FeedBackDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class FeedBackService : IFeedBackService
{
    private readonly AppDbContext _context;

    public FeedBackService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult> GetAllAsync()
    {
        var feedbacks = await _context.Feedbacks
            .AsNoTracking()
            .OrderByDescending(x => x.CreateDate)
            .ToListAsync();

        return ServiceResponse.Ok("Feedback list retrieved successfully.", feedbacks.Select(x => x.ToDto()).ToList());
    }

    public async Task<ServiceResult> GetByIdAsync(Guid id)
    {
        var feedback = await _context.Feedbacks.AsNoTracking().FirstOrDefaultAsync(x => x.FeedbackId == id);
        return feedback is null
            ? ServiceResponse.NotFound("Feedback not found.")
            : ServiceResponse.Ok("Feedback retrieved successfully.", feedback.ToDto());
    }

    public async Task<ServiceResult> CreateAsync(CreateFeedBackDTO dto)
    {
        if (!dto.Rating.HasValue || dto.Rating < 1 || dto.Rating > 5)
        {
            return ServiceResponse.BadRequest("Rating must be between 1 and 5.");
        }

        var booking = await _context.Bookings.AsNoTracking().FirstOrDefaultAsync(x => x.BookingId == dto.BookingId);
        if (booking is null)
        {
            return ServiceResponse.NotFound("Booking not found.");
        }

        if (booking.AccountId != dto.AccountId)
        {
            return ServiceResponse.Forbidden("You can only submit feedback for your own booking.");
        }

        var feedback = new Feedback
        {
            FeedbackId = Guid.NewGuid(),
            AccountId = dto.AccountId,
            BookingId = dto.BookingId,
            StationId = booking.StationId,
            Rating = dto.Rating.Value,
            Comment = dto.Comment?.Trim(),
            CreateDate = DateTime.UtcNow
        };

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();
        return ServiceResponse.Created("Feedback created successfully.", feedback.ToDto());
    }

    public async Task<ServiceResult> UpdateAsync(Guid id, UpdateFeedBackDTO dto)
    {
        var feedback = await _context.Feedbacks.FirstOrDefaultAsync(x => x.FeedbackId == id);
        if (feedback is null)
        {
            return ServiceResponse.NotFound("Feedback not found.");
        }

        if (dto.Rating.HasValue)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return ServiceResponse.BadRequest("Rating must be between 1 and 5.");
            }

            feedback.Rating = dto.Rating.Value;
        }

        if (dto.Comment is not null)
        {
            feedback.Comment = dto.Comment.Trim();
        }

        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Feedback updated successfully.", feedback.ToDto());
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var feedback = await _context.Feedbacks.FirstOrDefaultAsync(x => x.FeedbackId == id);
        if (feedback is null)
        {
            return ServiceResponse.NotFound("Feedback not found.");
        }

        _context.Feedbacks.Remove(feedback);
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Feedback deleted successfully.");
    }

    public async Task<ServiceResult> GetByAccountIdAsync(Guid accountId)
    {
        var feedbacks = await _context.Feedbacks
            .AsNoTracking()
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.CreateDate)
            .ToListAsync();

        return ServiceResponse.Ok("Feedback list retrieved successfully.", feedbacks.Select(x => x.ToDto()).ToList());
    }
}
