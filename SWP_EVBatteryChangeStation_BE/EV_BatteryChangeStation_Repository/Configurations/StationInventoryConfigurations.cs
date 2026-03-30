using EV_BatteryChangeStation_Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EV_BatteryChangeStation_Repository.Configurations;

internal sealed class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.ToTable("Station");
        builder.HasKey(x => x.StationId);
        builder.Property(x => x.StationId).HasColumnName("StationID");
        builder.Property(x => x.StationName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Area).HasMaxLength(100);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.Latitude).HasColumnType("numeric(9,6)");
        builder.Property(x => x.Longitude).HasColumnType("numeric(9,6)");
        builder.Property(x => x.OperatingHours).HasMaxLength(100);
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");
    }
}

internal sealed class BatteryTypeConfiguration : IEntityTypeConfiguration<BatteryType>
{
    public void Configure(EntityTypeBuilder<BatteryType> builder)
    {
        builder.ToTable("BatteryType");
        builder.HasKey(x => x.BatteryTypeId);
        builder.Property(x => x.BatteryTypeId).HasColumnName("BatteryTypeID");
        builder.Property(x => x.BatteryTypeCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.BatteryTypeName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Voltage).HasColumnType("numeric(10,2)");
        builder.Property(x => x.CapacityKwh).HasColumnName("Capacity_kWh").HasColumnType("numeric(10,2)");
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.BatteryTypeCode).IsUnique();
    }
}

internal sealed class StationBatteryTypeConfiguration : IEntityTypeConfiguration<StationBatteryType>
{
    public void Configure(EntityTypeBuilder<StationBatteryType> builder)
    {
        builder.ToTable("StationBatteryType");
        builder.HasKey(x => x.StationBatteryTypeId);
        builder.Property(x => x.StationBatteryTypeId).HasColumnName("StationBatteryTypeID");
        builder.Property(x => x.StationId).HasColumnName("StationID");
        builder.Property(x => x.BatteryTypeId).HasColumnName("BatteryTypeID");
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => new { x.StationId, x.BatteryTypeId }).IsUnique();

        builder.HasOne(x => x.Station).WithMany(x => x.StationBatteryTypes).HasForeignKey(x => x.StationId);
        builder.HasOne(x => x.BatteryType).WithMany(x => x.StationBatteryTypes).HasForeignKey(x => x.BatteryTypeId);
    }
}

internal sealed class BatteryConfiguration : IEntityTypeConfiguration<Battery>
{
    public void Configure(EntityTypeBuilder<Battery> builder)
    {
        builder.ToTable("Battery");
        builder.HasKey(x => x.BatteryId);
        builder.Property(x => x.BatteryId).HasColumnName("BatteryID");
        builder.Property(x => x.SerialNumber).HasMaxLength(100).IsRequired();
        builder.Property(x => x.BatteryTypeId).HasColumnName("BatteryTypeID");
        builder.Property(x => x.CapacityKwh).HasColumnName("Capacity_kWh").HasColumnType("numeric(10,2)");
        builder.Property(x => x.StateOfHealth).HasColumnType("numeric(5,2)");
        builder.Property(x => x.CurrentChargeLevel).HasColumnType("numeric(5,2)");
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.StationId).HasColumnName("StationID");
        builder.Property(x => x.LastChargedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.LastUsedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.SerialNumber).IsUnique();

        builder.HasOne(x => x.BatteryType).WithMany(x => x.Batteries).HasForeignKey(x => x.BatteryTypeId);
        builder.HasOne(x => x.Station).WithMany(x => x.Batteries).HasForeignKey(x => x.StationId);
    }
}

internal sealed class BatteryHistoryConfiguration : IEntityTypeConfiguration<BatteryHistory>
{
    public void Configure(EntityTypeBuilder<BatteryHistory> builder)
    {
        builder.ToTable("BatteryHistory");
        builder.HasKey(x => x.HistoryId);
        builder.Property(x => x.HistoryId).HasColumnName("HistoryID");
        builder.Property(x => x.BatteryId).HasColumnName("BatteryID");
        builder.Property(x => x.FromStationId).HasColumnName("FromStationID");
        builder.Property(x => x.ToStationId).HasColumnName("ToStationID");
        builder.Property(x => x.FromVehicleId).HasColumnName("FromVehicleID");
        builder.Property(x => x.ToVehicleId).HasColumnName("ToVehicleID");
        builder.Property(x => x.ActionType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.FromStatus).HasMaxLength(20);
        builder.Property(x => x.ToStatus).HasMaxLength(20);
        builder.Property(x => x.EventDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.SoHAtTime).HasColumnType("numeric(5,2)");
        builder.Property(x => x.ChargeLevelAtTime).HasColumnType("numeric(5,2)");
        builder.Property(x => x.Note).HasMaxLength(255);
        builder.Property(x => x.ActorAccountId).HasColumnName("ActorAccountID");

        builder.HasOne(x => x.Battery).WithMany(x => x.BatteryHistories).HasForeignKey(x => x.BatteryId);
        builder.HasOne(x => x.FromStation).WithMany().HasForeignKey(x => x.FromStationId);
        builder.HasOne(x => x.ToStation).WithMany().HasForeignKey(x => x.ToStationId);
        builder.HasOne(x => x.FromVehicle).WithMany(x => x.BatteryHistoryFromVehicles).HasForeignKey(x => x.FromVehicleId);
        builder.HasOne(x => x.ToVehicle).WithMany(x => x.BatteryHistoryToVehicles).HasForeignKey(x => x.ToVehicleId);
        builder.HasOne(x => x.ActorAccount).WithMany(x => x.BatteryHistoryActions).HasForeignKey(x => x.ActorAccountId);
    }
}

internal sealed class StationInventoryLogConfiguration : IEntityTypeConfiguration<StationInventoryLog>
{
    public void Configure(EntityTypeBuilder<StationInventoryLog> builder)
    {
        builder.ToTable("StationInventoryLog");
        builder.HasKey(x => x.LogId);
        builder.Property(x => x.LogId).HasColumnName("LogID");
        builder.Property(x => x.StationId).HasColumnName("StationID");
        builder.Property(x => x.BatteryTypeId).HasColumnName("BatteryTypeID");
        builder.Property(x => x.LogTime).HasColumnType("timestamp with time zone");
        builder.Property(x => x.AvgChargeLevel).HasColumnType("numeric(5,2)");

        builder.HasOne(x => x.Station).WithMany(x => x.InventoryLogs).HasForeignKey(x => x.StationId);
        builder.HasOne(x => x.BatteryType).WithMany(x => x.InventoryLogs).HasForeignKey(x => x.BatteryTypeId);
    }
}

