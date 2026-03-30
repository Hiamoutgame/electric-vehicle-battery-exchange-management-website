using EV_BatteryChangeStation_Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EV_BatteryChangeStation_Repository.Configurations;

internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Booking");
        builder.HasKey(x => x.BookingId);
        builder.Property(x => x.BookingId).HasColumnName("BookingID");
        builder.Property(x => x.AccountId).HasColumnName("AccountID");
        builder.Property(x => x.VehicleId).HasColumnName("VehicleID");
        builder.Property(x => x.StationId).HasColumnName("StationID");
        builder.Property(x => x.RequestedBatteryTypeId).HasColumnName("RequestedBatteryTypeID");
        builder.Property(x => x.TargetTime).HasColumnType("timestamp with time zone");
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(255);
        builder.Property(x => x.StaffNote).HasMaxLength(255);
        builder.Property(x => x.ApprovedDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.CancelledDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Account).WithMany(x => x.Bookings).HasForeignKey(x => x.AccountId);
        builder.HasOne(x => x.Vehicle).WithMany(x => x.Bookings).HasForeignKey(x => x.VehicleId);
        builder.HasOne(x => x.Station).WithMany(x => x.Bookings).HasForeignKey(x => x.StationId);
        builder.HasOne(x => x.RequestedBatteryType).WithMany(x => x.RequestedBookings).HasForeignKey(x => x.RequestedBatteryTypeId);
        builder.HasOne(x => x.ApprovedByAccount).WithMany(x => x.ApprovedBookings).HasForeignKey(x => x.ApprovedBy);
    }
}

internal sealed class SwappingTransactionConfiguration : IEntityTypeConfiguration<SwappingTransaction>
{
    public void Configure(EntityTypeBuilder<SwappingTransaction> builder)
    {
        builder.ToTable("SwappingTransaction");
        builder.HasKey(x => x.TransactionId);
        builder.Property(x => x.TransactionId).HasColumnName("TransactionID");
        builder.Property(x => x.BookingId).HasColumnName("BookingID");
        builder.Property(x => x.VehicleId).HasColumnName("VehicleID");
        builder.Property(x => x.StaffId).HasColumnName("StaffID");
        builder.Property(x => x.StationId).HasColumnName("StationID");
        builder.Property(x => x.ReturnedBatteryId).HasColumnName("ReturnedBatteryID");
        builder.Property(x => x.ReturnedBatterySoH).HasColumnType("numeric(5,2)");
        builder.Property(x => x.ReturnedBatteryCharge).HasColumnType("numeric(5,2)");
        builder.Property(x => x.ReturnedBatteryCondition).HasMaxLength(20);
        builder.Property(x => x.ReleasedBatteryId).HasColumnName("ReleasedBatteryID");
        builder.Property(x => x.ReleasedBatterySoH).HasColumnType("numeric(5,2)");
        builder.Property(x => x.ReleasedBatteryCharge).HasColumnType("numeric(5,2)");
        builder.Property(x => x.SwapFee).HasColumnType("numeric(18,2)");
        builder.Property(x => x.Notes).HasMaxLength(255);
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.BookingId).IsUnique();

        builder.HasOne(x => x.Booking).WithOne(x => x.SwappingTransaction).HasForeignKey<SwappingTransaction>(x => x.BookingId);
        builder.HasOne(x => x.Vehicle).WithMany(x => x.SwappingTransactions).HasForeignKey(x => x.VehicleId);
        builder.HasOne(x => x.Staff).WithMany(x => x.StaffSwapTransactions).HasForeignKey(x => x.StaffId);
        builder.HasOne(x => x.Station).WithMany(x => x.SwappingTransactions).HasForeignKey(x => x.StationId);
        builder.HasOne(x => x.ReturnedBattery).WithMany(x => x.ReturnedSwapTransactions).HasForeignKey(x => x.ReturnedBatteryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReleasedBattery).WithMany(x => x.ReleasedSwapTransactions).HasForeignKey(x => x.ReleasedBatteryId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class BatteryReturnInspectionConfiguration : IEntityTypeConfiguration<BatteryReturnInspection>
{
    public void Configure(EntityTypeBuilder<BatteryReturnInspection> builder)
    {
        builder.ToTable("BatteryReturnInspection");
        builder.HasKey(x => x.InspectionId);
        builder.Property(x => x.InspectionId).HasColumnName("InspectionID");
        builder.Property(x => x.BookingId).HasColumnName("BookingID");
        builder.Property(x => x.BatteryId).HasColumnName("BatteryID");
        builder.Property(x => x.StationId).HasColumnName("StationID");
        builder.Property(x => x.StaffId).HasColumnName("StaffID");
        builder.Property(x => x.SoHPercent).HasColumnType("numeric(5,2)");
        builder.Property(x => x.PhysicalCondition).HasMaxLength(20).IsRequired();
        builder.Property(x => x.InspectionNote).HasMaxLength(255);
        builder.Property(x => x.NextStatus).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Booking).WithMany(x => x.BatteryReturnInspections).HasForeignKey(x => x.BookingId);
        builder.HasOne(x => x.Battery).WithMany(x => x.ReturnInspections).HasForeignKey(x => x.BatteryId);
        builder.HasOne(x => x.Station).WithMany(x => x.BatteryReturnInspections).HasForeignKey(x => x.StationId);
        builder.HasOne(x => x.Staff).WithMany(x => x.BatteryReturnInspections).HasForeignKey(x => x.StaffId);
    }
}

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payment");
        builder.HasKey(x => x.PaymentId);
        builder.Property(x => x.PaymentId).HasColumnName("PaymentID");
        builder.Property(x => x.AccountId).HasColumnName("AccountID");
        builder.Property(x => x.BookingId).HasColumnName("BookingID");
        builder.Property(x => x.UserSubscriptionId).HasColumnName("UserSubscriptionID");
        builder.Property(x => x.TransactionId).HasColumnName("TransactionID");
        builder.Property(x => x.Amount).HasColumnType("numeric(18,2)");
        builder.Property(x => x.Currency).HasMaxLength(10).IsRequired();
        builder.Property(x => x.PaymentType).HasMaxLength(30).IsRequired();
        builder.Property(x => x.PaymentMethod).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.TransactionReference).HasMaxLength(100);
        builder.Property(x => x.RecordedByAccountId).HasColumnName("RecordedByAccountID");
        builder.Property(x => x.PaidAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.TransactionReference).IsUnique();

        builder.HasOne(x => x.Account).WithMany(x => x.Payments).HasForeignKey(x => x.AccountId);
        builder.HasOne(x => x.Booking).WithMany(x => x.Payments).HasForeignKey(x => x.BookingId);
        builder.HasOne(x => x.UserSubscription).WithMany(x => x.Payments).HasForeignKey(x => x.UserSubscriptionId);
        builder.HasOne(x => x.Transaction).WithMany(x => x.Payments).HasForeignKey(x => x.TransactionId);
        builder.HasOne(x => x.RecordedByAccount).WithMany(x => x.RecordedPayments).HasForeignKey(x => x.RecordedByAccountId);
    }
}

internal sealed class SupportRequestConfiguration : IEntityTypeConfiguration<SupportRequest>
{
    public void Configure(EntityTypeBuilder<SupportRequest> builder)
    {
        builder.ToTable("SupportRequest");
        builder.HasKey(x => x.RequestId);
        builder.Property(x => x.RequestId).HasColumnName("RequestID");
        builder.Property(x => x.AccountId).HasColumnName("AccountID");
        builder.Property(x => x.StationId).HasColumnName("StationID");
        builder.Property(x => x.BookingId).HasColumnName("BookingID");
        builder.Property(x => x.TransactionId).HasColumnName("TransactionID");
        builder.Property(x => x.IssueType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Subject).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Priority).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone");
        builder.Property(x => x.ClosedDate).HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Account).WithMany(x => x.SupportRequests).HasForeignKey(x => x.AccountId);
        builder.HasOne(x => x.Station).WithMany(x => x.SupportRequests).HasForeignKey(x => x.StationId);
        builder.HasOne(x => x.Booking).WithMany(x => x.SupportRequests).HasForeignKey(x => x.BookingId);
        builder.HasOne(x => x.Transaction).WithMany(x => x.SupportRequests).HasForeignKey(x => x.TransactionId);
    }
}

internal sealed class SupportRequestResponseConfiguration : IEntityTypeConfiguration<SupportRequestResponse>
{
    public void Configure(EntityTypeBuilder<SupportRequestResponse> builder)
    {
        builder.ToTable("SupportRequestResponse");
        builder.HasKey(x => x.ResponseId);
        builder.Property(x => x.ResponseId).HasColumnName("ResponseID");
        builder.Property(x => x.RequestId).HasColumnName("RequestID");
        builder.Property(x => x.StaffId).HasColumnName("StaffID");
        builder.Property(x => x.StatusAfterResponse).HasMaxLength(20).IsRequired();
        builder.Property(x => x.RespondedAt).HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Request).WithMany(x => x.Responses).HasForeignKey(x => x.RequestId);
        builder.HasOne(x => x.Staff).WithMany(x => x.SupportResponses).HasForeignKey(x => x.StaffId);
    }
}

internal sealed class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.ToTable("Feedback");
        builder.HasKey(x => x.FeedbackId);
        builder.Property(x => x.FeedbackId).HasColumnName("FeedbackID");
        builder.Property(x => x.AccountId).HasColumnName("AccountID");
        builder.Property(x => x.StationId).HasColumnName("StationID");
        builder.Property(x => x.BookingId).HasColumnName("BookingID");
        builder.Property(x => x.TransactionId).HasColumnName("TransactionID");
        builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.Account).WithMany(x => x.Feedbacks).HasForeignKey(x => x.AccountId);
        builder.HasOne(x => x.Station).WithMany(x => x.Feedbacks).HasForeignKey(x => x.StationId);
        builder.HasOne(x => x.Booking).WithMany(x => x.Feedbacks).HasForeignKey(x => x.BookingId);
        builder.HasOne(x => x.Transaction).WithMany(x => x.Feedbacks).HasForeignKey(x => x.TransactionId);
    }
}

