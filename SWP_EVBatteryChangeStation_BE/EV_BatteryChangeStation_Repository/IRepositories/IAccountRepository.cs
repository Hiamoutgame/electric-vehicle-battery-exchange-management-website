using EV_BatteryChangeStation_Repository.Entities;

namespace EV_BatteryChangeStation_Repository.IRepositories;

public interface IAccountRepository
{
    Task<Account?> GetByIdWithRoleAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Account?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default);
    Task<Account?> GetByUsernameWithRoleAsync(string username, CancellationToken cancellationToken = default);
    Task<List<Account>> GetAccountsAsync(string? roleName = null, string? status = null, string? keyword = null, CancellationToken cancellationToken = default);
    Task<StationStaffAssignment?> GetActiveStaffAssignmentAsync(Guid staffId, DateOnly? effectiveDate = null, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeAccountId = null, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, Guid? excludeAccountId = null, CancellationToken cancellationToken = default);
}


