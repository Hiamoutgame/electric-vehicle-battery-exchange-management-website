using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace EV_BatteryChangeStation_Repository.UnitOfWork;

public interface IUnitOfWork
{
    IAccountRepository AccountRepository { get; }
    IStationRepository StationRepository { get; }
    IBatteryRepository BatteryRepository { get; }
    IVehicleRepository VehicleRepository { get; }
    ISubscriptionRepository SubscriptionRepository { get; }
    IBookingRepository BookingRepository { get; }
    ISwapRepository SwapRepository { get; }
    IPaymentRepository PaymentRepository { get; }
    ISupportRequestRepository SupportRequestRepository { get; }
    IReportRepository ReportRepository { get; }
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}


