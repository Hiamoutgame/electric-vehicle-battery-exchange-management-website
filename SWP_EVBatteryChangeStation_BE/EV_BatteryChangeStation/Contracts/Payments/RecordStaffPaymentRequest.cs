namespace EV_BatteryChangeStation.Contracts.Payments;

public sealed class RecordStaffPaymentRequest
{
    public Guid SwapTransactionId { get; init; }

    public decimal? Amount { get; init; }

    public string? PaymentMethod { get; init; }
}
