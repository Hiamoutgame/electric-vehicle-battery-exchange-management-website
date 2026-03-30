using EV_BatteryChangeStation_Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EV_BatteryChangeStation_Repository.Configurations;

internal sealed class StationStaffAssignmentConfiguration : IEntityTypeConfiguration<StationStaffAssignment>
{
    public void Configure(EntityTypeBuilder<StationStaffAssignment> builder)
    {
        builder.ToTable("StationStaffAssignment");
        builder.HasKey(x => x.AssignmentId);
        builder.Property(x => x.AssignmentId).HasColumnName("AssignmentID");
        builder.Property(x => x.StaffId).HasColumnName("StaffID");
        builder.Property(x => x.StationId).HasColumnName("StationID");
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Staff).WithMany(x => x.StationAssignments).HasForeignKey(x => x.StaffId);
        builder.HasOne(x => x.Station).WithMany(x => x.StaffAssignments).HasForeignKey(x => x.StationId);
    }
}

internal sealed class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
{
    public void Configure(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.ToTable("VehicleModel");
        builder.HasKey(x => x.ModelId);
        builder.Property(x => x.ModelId).HasColumnName("ModelID");
        builder.Property(x => x.ModelName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Producer).HasMaxLength(150);
        builder.Property(x => x.BatteryTypeId).HasColumnName("BatteryTypeID");
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => new { x.ModelName, x.Producer }).IsUnique();

        builder.HasOne(x => x.BatteryType).WithMany(x => x.VehicleModels).HasForeignKey(x => x.BatteryTypeId);
    }
}

internal sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicle");
        builder.HasKey(x => x.VehicleId);
        builder.Property(x => x.VehicleId).HasColumnName("VehicleID");
        builder.Property(x => x.Vin).HasColumnName("VIN").HasMaxLength(50);
        builder.Property(x => x.LicensePlate).HasMaxLength(20).IsRequired();
        builder.Property(x => x.ModelId).HasColumnName("ModelID");
        builder.Property(x => x.OwnerId).HasColumnName("OwnerID");
        builder.Property(x => x.CurrentBatteryId).HasColumnName("CurrentBatteryID");
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.LicensePlate).IsUnique();
        builder.HasIndex(x => x.Vin).IsUnique();

        builder.HasOne(x => x.Model).WithMany(x => x.Vehicles).HasForeignKey(x => x.ModelId);
        builder.HasOne(x => x.Owner).WithMany(x => x.OwnedVehicles).HasForeignKey(x => x.OwnerId);
        builder.HasOne(x => x.CurrentBattery).WithMany(x => x.VehiclesUsingAsCurrent).HasForeignKey(x => x.CurrentBatteryId);
    }
}

internal sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlan");
        builder.HasKey(x => x.PlanId);
        builder.Property(x => x.PlanId).HasColumnName("PlanID");
        builder.Property(x => x.PlanCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PlanName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.BasePrice).HasColumnType("numeric(18,2)");
        builder.Property(x => x.Currency).HasMaxLength(10).IsRequired();
        builder.Property(x => x.ExtraFeePerSwap).HasColumnType("numeric(18,2)");
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.PlanCode).IsUnique();
    }
}

internal sealed class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("UserSubscription");
        builder.HasKey(x => x.UserSubscriptionId);
        builder.Property(x => x.UserSubscriptionId).HasColumnName("UserSubscriptionID");
        builder.Property(x => x.AccountId).HasColumnName("AccountID");
        builder.Property(x => x.VehicleId).HasColumnName("VehicleID");
        builder.Property(x => x.PlanId).HasColumnName("PlanID");
        builder.Property(x => x.StartDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.EndDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Account).WithMany(x => x.UserSubscriptions).HasForeignKey(x => x.AccountId);
        builder.HasOne(x => x.Vehicle).WithMany(x => x.UserSubscriptions).HasForeignKey(x => x.VehicleId);
        builder.HasOne(x => x.Plan).WithMany(x => x.UserSubscriptions).HasForeignKey(x => x.PlanId);
    }
}

