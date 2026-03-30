using EV_BatteryChangeStation_Common.DTOs.AccountDto;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class AccountService : IAccountService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Account> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(AppDbContext context, IUnitOfWork unitOfWork, IPasswordHasher<Account> passwordHasher)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<IServiceResult> CreateAccountAsync(CreateAccountDTO createAccount)
    {
        if (string.IsNullOrWhiteSpace(createAccount.AccountName) ||
            string.IsNullOrWhiteSpace(createAccount.Password) ||
            string.IsNullOrWhiteSpace(createAccount.Email))
        {
            return ServiceResponse.BadRequest("Username, password and email are required.");
        }

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleId == createAccount.RoleId);
        if (role is null)
        {
            return ServiceResponse.NotFound("Role not found.");
        }

        if (await _unitOfWork.AccountRepository.UsernameExistsAsync(createAccount.AccountName))
        {
            return ServiceResponse.Conflict("Username already exists.");
        }

        if (await _unitOfWork.AccountRepository.EmailExistsAsync(createAccount.Email))
        {
            return ServiceResponse.Conflict("Email already exists.");
        }

        var account = new Account
        {
            AccountId = Guid.NewGuid(),
            Username = createAccount.AccountName.Trim(),
            Email = createAccount.Email.Trim(),
            FullName = createAccount.FullName?.Trim(),
            PhoneNumber = createAccount.PhoneNumber?.Trim(),
            Gender = createAccount.Gender?.Trim(),
            Address = createAccount.Address?.Trim(),
            DateOfBirth = createAccount.DateOfBirth,
            RoleId = role.RoleId,
            Status = "ACTIVE",
            CreateDate = DateTime.UtcNow
        };
        account.PasswordHash = _passwordHasher.HashPassword(account, createAccount.Password);

        _context.Accounts.Add(account);

        if (string.Equals(role.RoleName, "STAFF", StringComparison.OrdinalIgnoreCase) &&
            createAccount.StationId != Guid.Empty)
        {
            var stationExists = await _context.Stations.AnyAsync(x => x.StationId == createAccount.StationId);
            if (!stationExists)
            {
                return ServiceResponse.NotFound("Assigned station not found.");
            }

            _context.StationStaffAssignments.Add(new StationStaffAssignment
            {
                AssignmentId = Guid.NewGuid(),
                StaffId = account.AccountId,
                StationId = createAccount.StationId,
                EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = "ACTIVE",
                CreateDate = DateTime.UtcNow
            });
        }

        await _unitOfWork.CommitAsync();
        account.Role = role;

        return ServiceResponse.Created("Account created successfully.", account.ToViewDto(createAccount.StationId));
    }

    public async Task<IServiceResult> UpdateAccountAsync(UpdateAccountDTO updateAccount)
    {
        var account = await _context.Accounts
            .Include(x => x.Role)
            .Include(x => x.StationAssignments)
            .FirstOrDefaultAsync(x => x.AccountId == updateAccount.AccountId);

        if (account is null)
        {
            return ServiceResponse.NotFound("Account not found.");
        }

        if (!string.IsNullOrWhiteSpace(updateAccount.AccountName) &&
            !string.Equals(updateAccount.AccountName.Trim(), account.Username, StringComparison.Ordinal))
        {
            if (await _unitOfWork.AccountRepository.UsernameExistsAsync(updateAccount.AccountName, account.AccountId))
            {
                return ServiceResponse.Conflict("Username already exists.");
            }

            account.Username = updateAccount.AccountName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(updateAccount.Email) &&
            !string.Equals(updateAccount.Email.Trim(), account.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _unitOfWork.AccountRepository.EmailExistsAsync(updateAccount.Email, account.AccountId))
            {
                return ServiceResponse.Conflict("Email already exists.");
            }

            account.Email = updateAccount.Email.Trim();
        }

        if (updateAccount.RoleId.HasValue)
        {
            var roleExists = await _context.Roles.AnyAsync(x => x.RoleId == updateAccount.RoleId.Value);
            if (!roleExists)
            {
                return ServiceResponse.NotFound("Role not found.");
            }

            account.RoleId = updateAccount.RoleId.Value;
        }

        if (!string.IsNullOrWhiteSpace(updateAccount.Password))
        {
            account.PasswordHash = _passwordHasher.HashPassword(account, updateAccount.Password);
        }

        account.FullName = updateAccount.FullName?.Trim() ?? account.FullName;
        account.PhoneNumber = updateAccount.PhoneNumber?.Trim() ?? account.PhoneNumber;
        account.Gender = updateAccount.Gender?.Trim() ?? account.Gender;
        account.Address = updateAccount.Address?.Trim() ?? account.Address;
        account.DateOfBirth = updateAccount.DateOfBirth ?? account.DateOfBirth;
        account.UpdateDate = DateTime.UtcNow;

        if (updateAccount.StationId.HasValue && updateAccount.StationId.Value != Guid.Empty)
        {
            var stationExists = await _context.Stations.AnyAsync(x => x.StationId == updateAccount.StationId.Value);
            if (!stationExists)
            {
                return ServiceResponse.NotFound("Assigned station not found.");
            }

            var activeAssignment = account.StationAssignments.FirstOrDefault(x => x.Status == "ACTIVE");
            if (activeAssignment is null)
            {
                _context.StationStaffAssignments.Add(new StationStaffAssignment
                {
                    AssignmentId = Guid.NewGuid(),
                    StaffId = account.AccountId,
                    StationId = updateAccount.StationId.Value,
                    EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
                    Status = "ACTIVE",
                    CreateDate = DateTime.UtcNow
                });
            }
            else
            {
                activeAssignment.StationId = updateAccount.StationId.Value;
                activeAssignment.UpdateDate = DateTime.UtcNow;
            }
        }

        await _unitOfWork.CommitAsync();
        return ServiceResponse.Ok("Account updated successfully.", account.ToViewDto(updateAccount.StationId));
    }

    public async Task<IServiceResult> DeleteAccountAsync(Guid id)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == id);
        if (account is null)
        {
            return ServiceResponse.NotFound("Account not found.");
        }

        _context.Accounts.Remove(account);
        await _unitOfWork.CommitAsync();
        return ServiceResponse.Ok("Account deleted successfully.");
    }

    public async Task<IServiceResult> GetAllAccountsAsync()
    {
        var accounts = await _unitOfWork.AccountRepository.GetAccountsAsync();
        return ServiceResponse.Ok("Accounts retrieved successfully.", accounts.Select(x => x.ToViewDto()).ToList());
    }

    public async Task<IServiceResult> GetAccountProfileAsync(Guid accountId)
    {
        var account = await _unitOfWork.AccountRepository.GetByIdWithRoleAsync(accountId);
        if (account is null)
        {
            return ServiceResponse.NotFound("Account not found.");
        }

        var activeAssignment = account.StationAssignments.FirstOrDefault(x => x.Status == "ACTIVE");

        return ServiceResponse.Ok("Account profile retrieved successfully.", new
        {
            account.AccountId,
            account.Username,
            account.Email,
            account.FullName,
            account.PhoneNumber,
            account.Gender,
            account.Address,
            account.DateOfBirth,
            account.Status,
            roleName = account.Role?.RoleName,
            stationId = activeAssignment?.StationId,
            stationName = activeAssignment?.Station?.StationName
        });
    }

    public async Task<IServiceResult> GetAccountByNameAsync(string accountName)
    {
        if (string.IsNullOrWhiteSpace(accountName))
        {
            return ServiceResponse.BadRequest("Account name is required.");
        }

        var account = await _unitOfWork.AccountRepository.GetByUsernameWithRoleAsync(accountName);
        return account is null
            ? ServiceResponse.NotFound("Account not found.")
            : ServiceResponse.Ok("Account retrieved successfully.", account.ToViewDto());
    }

    public async Task<IServiceResult> SoftDeleteAsync(Guid id)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == id);
        if (account is null)
        {
            return ServiceResponse.NotFound("Account not found.");
        }

        account.Status = "INACTIVE";
        account.UpdateDate = DateTime.UtcNow;
        await _unitOfWork.CommitAsync();
        return ServiceResponse.Ok("Account deactivated successfully.");
    }

    public async Task<IServiceResult> GetAllStaffAccountAsync()
    {
        var accounts = await _unitOfWork.AccountRepository.GetAccountsAsync("STAFF", "ACTIVE");
        return ServiceResponse.Ok("Staff accounts retrieved successfully.", accounts.Select(x => x.ToViewDto()).ToList());
    }
}
