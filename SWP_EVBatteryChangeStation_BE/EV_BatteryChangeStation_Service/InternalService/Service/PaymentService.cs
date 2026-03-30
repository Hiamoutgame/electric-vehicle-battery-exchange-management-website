using EV_BatteryChangeStation_Common.DTOs.PaymentDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult> CreatePayment(CreatePaymentDto create)
    {
        Guid accountId;
        Guid? bookingId = null;
        Guid? transactionId = null;
        Guid? userSubscriptionId = create.SubscriptionId;
        string paymentType;
        decimal amount;

        if (create.TransactionId.HasValue)
        {
            var transaction = await _context.SwappingTransactions
                .AsNoTracking()
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(x => x.TransactionId == create.TransactionId.Value);

            if (transaction?.Booking is null)
            {
                return ServiceResponse.NotFound("Swap transaction not found.");
            }

            accountId = transaction.Booking.AccountId;
            bookingId = transaction.BookingId;
            transactionId = transaction.TransactionId;
            paymentType = "SWAP";
            amount = create.Price ?? transaction.SwapFee;
        }
        else if (create.AccountId.HasValue)
        {
            accountId = create.AccountId.Value;
            paymentType = "SUBSCRIPTION";
            amount = create.Price ?? 0m;
        }
        else
        {
            return ServiceResponse.BadRequest("TransactionId or AccountId is required.");
        }

        var payment = new Payment
        {
            PaymentId = Guid.NewGuid(),
            AccountId = accountId,
            BookingId = bookingId,
            UserSubscriptionId = userSubscriptionId,
            TransactionId = transactionId,
            Amount = amount,
            PaymentType = paymentType,
            PaymentMethod = string.IsNullOrWhiteSpace(create.Method) ? "CASH" : create.Method.Trim().ToUpperInvariant(),
            Status = string.Equals(create.Method, "CASH", StringComparison.OrdinalIgnoreCase) ? "PAID" : "PENDING",
            PaymentGatewayId = create.PaymentGateId <= 0 ? null : create.PaymentGateId,
            TransactionReference = create.PaymentGateId > 0 ? create.PaymentGateId.ToString() : null,
            PaidAt = string.Equals(create.Method, "CASH", StringComparison.OrdinalIgnoreCase) ? DateTime.UtcNow : null,
            CreateDate = create.CreateDate ?? DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return ServiceResponse.Created("Payment created successfully.", payment.ToRespondDto());
    }

    public async Task<IServiceResult> GetPaymentById(Guid paymentId)
    {
        var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
        return payment is null
            ? ServiceResponse.NotFound("Payment not found.")
            : ServiceResponse.Ok("Payment retrieved successfully.", payment.ToRespondDto());
    }

    public async Task<IServiceResult> UpdatePayment(UpdatePaymentDto update)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(x => x.PaymentId == update.PaymentId);
        if (payment is null)
        {
            return ServiceResponse.NotFound("Payment not found.");
        }

        payment.AccountId = update.AccountId;
        payment.Status = update.Status.Trim().ToUpperInvariant();
        if (payment.Status == "PAID")
        {
            payment.PaidAt ??= DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Payment updated successfully.", payment.ToRespondDto());
    }

    public async Task<IServiceResult> DeletePayment(Guid paymentId)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(x => x.PaymentId == paymentId);
        if (payment is null)
        {
            return ServiceResponse.NotFound("Payment not found.");
        }

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Payment deleted successfully.");
    }

    public async Task<IServiceResult> SoftDeletePayment(Guid paymentId)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(x => x.PaymentId == paymentId);
        if (payment is null)
        {
            return ServiceResponse.NotFound("Payment not found.");
        }

        payment.Status = "CANCELLED";
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Payment cancelled successfully.");
    }

    public async Task<IServiceResult> GetPaymentByAccountId(Guid accountId)
    {
        var payments = await _unitOfWork.PaymentRepository.GetPaymentsByAccountAsync(accountId);
        return ServiceResponse.Ok("Payments retrieved successfully.", payments.Select(x => x.ToRespondDto()).ToList());
    }

    public async Task<IServiceResult> GetAllPayment()
    {
        var payments = await _context.Payments
            .AsNoTracking()
            .OrderByDescending(x => x.PaidAt ?? x.CreateDate)
            .ToListAsync();

        return ServiceResponse.Ok("Payments retrieved successfully.", payments.Select(x => x.ToRespondDto()).ToList());
    }

    public async Task<IServiceResult> GetPaymentByTransactionId(Guid transactionId)
    {
        var payment = await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TransactionId == transactionId);

        return payment is null
            ? ServiceResponse.NotFound("Payment not found.")
            : ServiceResponse.Ok("Payment retrieved successfully.", payment.ToRespondDto());
    }

    public async Task<IServiceResult> GetByGateWayId(long gateway)
    {
        var payment = await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PaymentGatewayId == gateway);

        return payment is null
            ? ServiceResponse.NotFound("Payment not found.")
            : ServiceResponse.Ok("Payment retrieved successfully.", payment.ToRespondDto());
    }

    public async Task<IServiceResult> CheckSubscriptionStatus(Guid accountId)
    {
        var activeSubscription = await _unitOfWork.SubscriptionRepository.GetActiveByAccountAsync(accountId);
        var latestPayment = await _context.Payments
            .AsNoTracking()
            .Where(x => x.AccountId == accountId && x.PaymentType == "SUBSCRIPTION")
            .OrderByDescending(x => x.PaidAt ?? x.CreateDate)
            .FirstOrDefaultAsync();

        var payload = new SubscriptionStatusCheckDto
        {
            HasActiveSubscription = activeSubscription is not null,
            NeedsRedirect = activeSubscription is null,
            Payment = latestPayment?.ToRespondDto()
        };

        return ServiceResponse.Ok("Subscription status checked successfully.", payload);
    }
}
