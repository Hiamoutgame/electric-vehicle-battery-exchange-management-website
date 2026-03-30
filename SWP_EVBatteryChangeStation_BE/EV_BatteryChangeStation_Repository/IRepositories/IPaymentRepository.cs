using EV_BatteryChangeStation_Repository.Entities;

namespace EV_BatteryChangeStation_Repository.IRepositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<List<Payment>> GetPaymentsByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<List<Payment>> GetPaymentsByBookingAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByTransactionReferenceAsync(string transactionReference, CancellationToken cancellationToken = default);
}


