using EV_BatteryChangeStation_Common.DTOs.RoleDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class RoleService : IRoleService
{
    private readonly AppDbContext _context;

    public RoleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IServiceResult> CreateRoleAsync(CreateRoleDTO createRole)
    {
        if (string.IsNullOrWhiteSpace(createRole.RoleName))
        {
            return ServiceResponse.BadRequest("Role name is required.");
        }

        var normalizedName = createRole.RoleName.Trim().ToUpperInvariant();
        if (await _context.Roles.AnyAsync(x => x.RoleName == normalizedName))
        {
            return ServiceResponse.Conflict("Role already exists.");
        }

        var role = new Role
        {
            RoleId = Guid.NewGuid(),
            RoleName = normalizedName,
            Status = LegacyStatusMapper.ToActiveStatus(createRole.Status),
            CreateDate = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return ServiceResponse.Created("Role created successfully.", role.ToViewDto());
    }

    public async Task<IServiceResult> UpdateRoleAsync(UpdateRoleDTO updateRole)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleId == updateRole.RoleId);
        if (role is null)
        {
            return ServiceResponse.NotFound("Role not found.");
        }

        if (!string.IsNullOrWhiteSpace(updateRole.RoleName))
        {
            var normalizedName = updateRole.RoleName.Trim().ToUpperInvariant();
            var duplicated = await _context.Roles.AnyAsync(x => x.RoleName == normalizedName && x.RoleId != role.RoleId);
            if (duplicated)
            {
                return ServiceResponse.Conflict("Role already exists.");
            }

            role.RoleName = normalizedName;
        }

        if (updateRole.Status.HasValue)
        {
            role.Status = LegacyStatusMapper.ToActiveStatus(updateRole.Status.Value);
        }

        role.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Role updated successfully.", role.ToViewDto());
    }

    public async Task<IServiceResult> DeleteRoleAsync(Guid deleteid)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleId == deleteid);
        if (role is null)
        {
            return ServiceResponse.NotFound("Role not found.");
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Role deleted successfully.");
    }

    public async Task<IServiceResult> SoftDeleteAsync(Guid softId)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleId == softId);
        if (role is null)
        {
            return ServiceResponse.NotFound("Role not found.");
        }

        role.Status = "INACTIVE";
        role.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Role deactivated successfully.");
    }

    public async Task<IServiceResult> GetAllRolesAsync()
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .OrderBy(x => x.RoleName)
            .ToListAsync();

        return ServiceResponse.Ok("Roles retrieved successfully.", roles.Select(x => x.ToViewDto()).ToList());
    }

    public async Task<IServiceResult> GetRoleByNameAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return ServiceResponse.BadRequest("Role name is required.");
        }

        var normalizedName = roleName.Trim().ToUpperInvariant();
        var role = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.RoleName == normalizedName);
        return role is null
            ? ServiceResponse.NotFound("Role not found.")
            : ServiceResponse.Ok("Role retrieved successfully.", role.ToViewDto());
    }
}
