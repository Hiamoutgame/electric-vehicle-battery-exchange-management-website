using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Payment?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return _context.Payments
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Account)
            .Include(x => x.Booking)
                .ThenInclude(x => x!.Station)
            .Include(x => x.UserSubscription)
                .ThenInclude(x => x!.Plan)
            .Include(x => x.Transaction)
                .ThenInclude(x => x!.Station)
            .Include(x => x.RecordedByAccount)
            .FirstOrDefaultAsync(x => x.PaymentId == paymentId, cancellationToken);
    }

    public Task<List<Payment>> GetPaymentsByAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return _context.Payments
            .AsNoTracking()
            .Include(x => x.Booking)
                .ThenInclude(x => x!.Station)
            .Include(x => x.UserSubscription)
                .ThenInclude(x => x!.Plan)
            .Include(x => x.Transaction)
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.PaidAt ?? x.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Payment>> GetPaymentsByBookingAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return _context.Payments
            .AsNoTracking()
            .Include(x => x.Transaction)
            .Include(x => x.RecordedByAccount)
            .Where(x => x.BookingId == bookingId)
            .OrderByDescending(x => x.PaidAt ?? x.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public Task<Payment?> GetByTransactionReferenceAsync(string transactionReference, CancellationToken cancellationToken = default)
    {
        var normalizedReference = transactionReference.Trim();

        return _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TransactionReference == normalizedReference, cancellationToken);
    }
}


