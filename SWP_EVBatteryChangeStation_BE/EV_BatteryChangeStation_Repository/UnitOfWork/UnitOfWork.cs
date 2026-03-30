using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.IRepositories;
using EV_BatteryChangeStation_Repository.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace EV_BatteryChangeStation_Repository.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IAccountRepository? _accountRepository;
    private IStationRepository? _stationRepository;
    private IBatteryRepository? _batteryRepository;
    private IVehicleRepository? _vehicleRepository;
    private ISubscriptionRepository? _subscriptionRepository;
    private IBookingRepository? _bookingRepository;
    private ISwapRepository? _swapRepository;
    private IPaymentRepository? _paymentRepository;
    private ISupportRequestRepository? _supportRequestRepository;
    private IReportRepository? _reportRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IAccountRepository AccountRepository => _accountRepository ??= new AccountRepository(_context);
    public IStationRepository StationRepository => _stationRepository ??= new StationRepository(_context);
    public IBatteryRepository BatteryRepository => _batteryRepository ??= new BatteryRepository(_context);
    public IVehicleRepository VehicleRepository => _vehicleRepository ??= new VehicleRepository(_context);
    public ISubscriptionRepository SubscriptionRepository => _subscriptionRepository ??= new SubscriptionRepository(_context);
    public IBookingRepository BookingRepository => _bookingRepository ??= new BookingRepository(_context);
    public ISwapRepository SwapRepository => _swapRepository ??= new SwapRepository(_context);
    public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);
    public ISupportRequestRepository SupportRequestRepository => _supportRequestRepository ??= new SupportRequestRepository(_context);
    public IReportRepository ReportRepository => _reportRepository ??= new ReportRepository(_context);

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.Database.BeginTransactionAsync(cancellationToken);
    }
}

