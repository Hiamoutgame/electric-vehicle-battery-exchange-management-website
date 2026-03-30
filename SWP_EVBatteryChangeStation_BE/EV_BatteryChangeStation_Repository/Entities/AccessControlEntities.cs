using System;
using System.Collections.Generic;

namespace EV_BatteryChangeStation_Repository.Entities;

public class Role
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class Permission
{
    public Guid PermissionId { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreateDate { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class RolePermission
{
    public Guid RolePermissionId { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public DateTime CreateDate { get; set; }

    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}

public class Account
{
    public Guid AccountId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Guid RoleId { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public Role? Role { get; set; }
    public ICollection<StationStaffAssignment> StationAssignments { get; set; } = new List<StationStaffAssignment>();
    public ICollection<Vehicle> OwnedVehicles { get; set; } = new List<Vehicle>();
    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Payment> RecordedPayments { get; set; } = new List<Payment>();
    public ICollection<SupportRequest> SupportRequests { get; set; } = new List<SupportRequest>();
    public ICollection<SupportRequestResponse> SupportResponses { get; set; } = new List<SupportRequestResponse>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public ICollection<SwappingTransaction> StaffSwapTransactions { get; set; } = new List<SwappingTransaction>();
    public ICollection<BatteryHistory> BatteryHistoryActions { get; set; } = new List<BatteryHistory>();
    public ICollection<BatteryReturnInspection> BatteryReturnInspections { get; set; } = new List<BatteryReturnInspection>();
    public ICollection<Booking> ApprovedBookings { get; set; } = new List<Booking>();
}

