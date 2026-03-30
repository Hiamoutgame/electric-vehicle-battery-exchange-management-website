using EV_BatteryChangeStation_Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EV_BatteryChangeStation_Repository.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role");
        builder.HasKey(x => x.RoleId);
        builder.Property(x => x.RoleId).HasColumnName("RoleID");
        builder.Property(x => x.RoleName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.RoleName).IsUnique();
    }
}

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permission");
        builder.HasKey(x => x.PermissionId);
        builder.Property(x => x.PermissionId).HasColumnName("PermissionID");
        builder.Property(x => x.PermissionCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PermissionName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.ModuleName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.PermissionCode).IsUnique();
    }
}

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermission");
        builder.HasKey(x => x.RolePermissionId);
        builder.Property(x => x.RolePermissionId).HasColumnName("RolePermissionID");
        builder.Property(x => x.RoleId).HasColumnName("RoleID");
        builder.Property(x => x.PermissionId).HasColumnName("PermissionID");
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => new { x.RoleId, x.PermissionId }).IsUnique();

        builder.HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId);
        builder.HasOne(x => x.Permission).WithMany(x => x.RolePermissions).HasForeignKey(x => x.PermissionId);
    }
}

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Account");
        builder.HasKey(x => x.AccountId);
        builder.Property(x => x.AccountId).HasColumnName("AccountID");
        builder.Property(x => x.Username).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
        builder.Property(x => x.FullName).HasMaxLength(150);
        builder.Property(x => x.Email).HasMaxLength(150).IsRequired();
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.Gender).HasMaxLength(20);
        builder.Property(x => x.Address).HasMaxLength(255);
        builder.Property(x => x.RoleId).HasColumnName("RoleID");
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.LastLoginAt).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasOne(x => x.Role).WithMany(x => x.Accounts).HasForeignKey(x => x.RoleId);
    }
}

