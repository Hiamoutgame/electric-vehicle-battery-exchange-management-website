using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories;

public sealed class SupportRequestRepository : ISupportRequestRepository
{
    private readonly AppDbContext _context;

    public SupportRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<SupportRequest?> GetByIdWithDetailsAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return _context.SupportRequests
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Account)
            .Include(x => x.Station)
            .Include(x => x.Booking)
            .Include(x => x.Transaction)
            .Include(x => x.Responses)
                .ThenInclude(x => x.Staff)
            .FirstOrDefaultAsync(x => x.RequestId == requestId, cancellationToken);
    }

    public Task<List<SupportRequest>> GetByAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return _context.SupportRequests
            .AsNoTracking()
            .Include(x => x.Station)
            .Include(x => x.Responses)
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public Task<List<SupportRequest>> GetByStationAsync(Guid stationId, string? status = null, string? issueType = null, CancellationToken cancellationToken = default)
    {
        IQueryable<SupportRequest> query = _context.SupportRequests
            .AsNoTracking()
            .Include(x => x.Account)
            .Include(x => x.Booking)
            .Include(x => x.Transaction)
            .Include(x => x.Responses)
                .ThenInclude(x => x.Staff)
            .Where(x => x.StationId == stationId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            query = query.Where(x => x.Status.ToUpper() == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(issueType))
        {
            var normalizedIssueType = issueType.Trim().ToUpperInvariant();
            query = query.Where(x => x.IssueType.ToUpper() == normalizedIssueType);
        }

        return query
            .AsSplitQuery()
            .OrderByDescending(x => x.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public Task<List<SupportRequest>> GetForAdminAsync(string? status = null, Guid? stationId = null, string? issueType = null, CancellationToken cancellationToken = default)
    {
        IQueryable<SupportRequest> query = _context.SupportRequests
            .AsNoTracking()
            .Include(x => x.Account)
            .Include(x => x.Station)
            .Include(x => x.Booking)
            .Include(x => x.Transaction)
            .Include(x => x.Responses);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            query = query.Where(x => x.Status.ToUpper() == normalizedStatus);
        }

        if (stationId.HasValue)
        {
            query = query.Where(x => x.StationId == stationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(issueType))
        {
            var normalizedIssueType = issueType.Trim().ToUpperInvariant();
            query = query.Where(x => x.IssueType.ToUpper() == normalizedIssueType);
        }

        return query
            .AsSplitQuery()
            .OrderByDescending(x => x.CreateDate)
            .ToListAsync(cancellationToken);
    }
}

