using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Account?> GetByIdWithRoleAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .AsNoTracking()
            .Include(x => x.Role)
            .Include(x => x.StationAssignments)
                .ThenInclude(x => x.Station)
            .FirstOrDefaultAsync(x => x.AccountId == accountId, cancellationToken);
    }

    public Task<Account?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim();

        return _context.Accounts
            .AsNoTracking()
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
    }

    public Task<Account?> GetByUsernameWithRoleAsync(string username, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim();

        return _context.Accounts
            .AsNoTracking()
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Username == normalizedUsername, cancellationToken);
    }

    public Task<List<Account>> GetAccountsAsync(string? roleName = null, string? status = null, string? keyword = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Account> query = _context.Accounts
            .AsNoTracking()
            .Include(x => x.Role)
            .Include(x => x.StationAssignments)
                .ThenInclude(x => x.Station);

        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var normalizedRole = roleName.Trim().ToUpperInvariant();
            query = query.Where(x => x.Role != null && x.Role.RoleName.ToUpper() == normalizedRole);
        }

        if (!string.IsNullOrWhiteSpace(status) && !string.Equals(status.Trim(), "ALL", StringComparison.OrdinalIgnoreCase))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            query = query.Where(x => x.Status.ToUpper() == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var pattern = $"%{keyword.Trim().ToUpperInvariant()}%";
            query = query.Where(x =>
                EF.Functions.Like(x.Username.ToUpper(), pattern) ||
                EF.Functions.Like(x.Email.ToUpper(), pattern) ||
                (x.FullName != null && EF.Functions.Like(x.FullName.ToUpper(), pattern)) ||
                (x.PhoneNumber != null && EF.Functions.Like(x.PhoneNumber.ToUpper(), pattern)));
        }

        return query
            .AsSplitQuery()
            .OrderBy(x => x.FullName ?? x.Username)
            .ToListAsync(cancellationToken);
    }

    public Task<StationStaffAssignment?> GetActiveStaffAssignmentAsync(Guid staffId, DateOnly? effectiveDate = null, CancellationToken cancellationToken = default)
    {
        var targetDate = effectiveDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        return _context.StationStaffAssignments
            .AsNoTracking()
            .Include(x => x.Station)
            .Include(x => x.Staff)
            .Where(x => x.StaffId == staffId
                        && x.Status == "ACTIVE"
                        && x.EffectiveFrom <= targetDate
                        && (x.EffectiveTo == null || x.EffectiveTo >= targetDate))
            .OrderByDescending(x => x.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, Guid? excludeAccountId = null, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim();

        return _context.Accounts
            .AsNoTracking()
            .AnyAsync(x => x.Email == normalizedEmail
                           && (!excludeAccountId.HasValue || x.AccountId != excludeAccountId.Value),
                cancellationToken);
    }

    public Task<bool> UsernameExistsAsync(string username, Guid? excludeAccountId = null, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim();

        return _context.Accounts
            .AsNoTracking()
            .AnyAsync(x => x.Username == normalizedUsername
                           && (!excludeAccountId.HasValue || x.AccountId != excludeAccountId.Value),
                cancellationToken);
    }
}


