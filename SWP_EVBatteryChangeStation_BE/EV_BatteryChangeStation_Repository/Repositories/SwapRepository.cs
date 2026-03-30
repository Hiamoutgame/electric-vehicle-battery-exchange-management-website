using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.Repositories;

public sealed class SwapRepository : ISwapRepository
{
    private readonly AppDbContext _context;

    public SwapRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<SwappingTransaction?> GetByIdWithDetailsAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return _context.SwappingTransactions
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Booking)
                .ThenInclude(x => x!.Account)
            .Include(x => x.Vehicle)
                .ThenInclude(x => x!.Model)
                    .ThenInclude(x => x!.BatteryType)
            .Include(x => x.Staff)
            .Include(x => x.Station)
            .Include(x => x.ReturnedBattery)
                .ThenInclude(x => x!.BatteryType)
            .Include(x => x.ReleasedBattery)
                .ThenInclude(x => x!.BatteryType)
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.TransactionId == transactionId, cancellationToken);
    }

    public Task<SwappingTransaction?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return _context.SwappingTransactions
            .AsNoTracking()
            .Include(x => x.Station)
            .Include(x => x.ReturnedBattery)
            .Include(x => x.ReleasedBattery)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId, cancellationToken);
    }

    public Task<List<SwappingTransaction>> GetHistoryByVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        return _context.SwappingTransactions
            .AsNoTracking()
            .Include(x => x.Station)
            .Include(x => x.Booking)
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public Task<List<SwappingTransaction>> GetHistoryByAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return _context.SwappingTransactions
            .AsNoTracking()
            .Include(x => x.Station)
            .Include(x => x.Booking)
            .Where(x => x.Booking != null && x.Booking.AccountId == accountId)
            .OrderByDescending(x => x.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsForBookingAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return _context.SwappingTransactions
            .AsNoTracking()
            .AnyAsync(x => x.BookingId == bookingId, cancellationToken);
    }
}


