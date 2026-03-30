using EV_BatteryChangeStation_Repository.Entities;

namespace EV_BatteryChangeStation_Repository.IRepositories;

public interface IVehicleRepository
{
    Task<List<Vehicle>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByIdWithDetailsAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetOwnedVehicleAsync(Guid vehicleId, Guid ownerId, CancellationToken cancellationToken = default);
    Task<bool> ExistsLicensePlateAsync(string licensePlate, Guid? excludeVehicleId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsVinAsync(string? vin, Guid? excludeVehicleId = null, CancellationToken cancellationToken = default);
}


