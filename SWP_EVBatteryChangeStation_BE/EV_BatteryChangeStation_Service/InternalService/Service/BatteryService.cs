using EV_BatteryChangeStation_Common.DTOs.BatteryDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class BatteryService : IBatteryService
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public BatteryService(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult> IsBatteryAvailable(Guid batteryId)
    {
        var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(batteryId);
        if (battery is null)
        {
            return ServiceResponse.NotFound("Battery not found.");
        }

        return ServiceResponse.Ok("Battery availability checked.", new
        {
            battery.BatteryId,
            battery.SerialNumber,
            isAvailable = string.Equals(battery.Status, "AVAILABLE", StringComparison.OrdinalIgnoreCase),
            battery.Status
        });
    }

    public async Task<IServiceResult> GetAllBattery()
    {
        var batteries = await _context.Batteries
            .AsNoTracking()
            .Include(x => x.BatteryType)
            .Include(x => x.Station)
            .OrderByDescending(x => x.CreateDate)
            .ToListAsync();

        return ServiceResponse.Ok("Battery list retrieved successfully.", batteries.Select(x => x.ToDto()).ToList());
    }

    public async Task<IServiceResult> GetBatteryById(Guid batteryId)
    {
        var battery = await _unitOfWork.BatteryRepository.GetByIdAsync(batteryId);
        return battery is null
            ? ServiceResponse.NotFound("Battery not found.")
            : ServiceResponse.Ok("Battery retrieved successfully.", battery.ToDto());
    }

    public async Task<IServiceResult> UpdateBatteryAsync(UpdateBattery updateDTO)
    {
        var battery = await _context.Batteries.FirstOrDefaultAsync(x => x.BatteryId == updateDTO.BatteryId);
        if (battery is null)
        {
            return ServiceResponse.NotFound("Battery not found.");
        }

        if (updateDTO.StationId != Guid.Empty)
        {
            var stationExists = await _context.Stations.AnyAsync(x => x.StationId == updateDTO.StationId);
            if (!stationExists)
            {
                return ServiceResponse.NotFound("Station not found.");
            }

            battery.StationId = updateDTO.StationId;
        }

        if (!string.IsNullOrWhiteSpace(updateDTO.TypeBattery))
        {
            var batteryType = await ResolveBatteryTypeAsync(updateDTO.TypeBattery);
            battery.BatteryTypeId = batteryType.BatteryTypeId;
        }

        battery.CapacityKwh = updateDTO.Capacity ?? battery.CapacityKwh;
        battery.StateOfHealth = updateDTO.StateOfHealth ?? battery.StateOfHealth;
        battery.CurrentChargeLevel = updateDTO.PercentUse ?? battery.CurrentChargeLevel;
        battery.InsuranceDate = updateDTO.InsuranceDate ?? battery.InsuranceDate;
        battery.LastUsedAt = updateDTO.LastUsed ?? battery.LastUsedAt;
        battery.LastChargedAt = updateDTO.BatterySwapDate ?? battery.LastChargedAt;
        if (updateDTO.Status.HasValue)
        {
            battery.Status = updateDTO.Status.Value ? "AVAILABLE" : "MAINTENANCE";
        }

        battery.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        battery = await _context.Batteries
            .AsNoTracking()
            .Include(x => x.BatteryType)
            .Include(x => x.Station)
            .FirstAsync(x => x.BatteryId == updateDTO.BatteryId);

        return ServiceResponse.Ok("Battery updated successfully.", battery.ToDto());
    }

    public async Task<IServiceResult> GetAllBatteryByStationId(Guid stationId)
    {
        var batteries = await _unitOfWork.BatteryRepository.GetStationInventoryAsync(stationId);
        return ServiceResponse.Ok("Station batteries retrieved successfully.", batteries.Select(x => x.ToDto()).ToList());
    }

    public async Task<IServiceResult> CreateBatteryAsync(CreateBatteryDTO createBattery)
    {
        var station = await _context.Stations.FirstOrDefaultAsync(x => x.StationId == createBattery.StationId);
        if (station is null)
        {
            return ServiceResponse.NotFound("Station not found.");
        }

        if (string.IsNullOrWhiteSpace(createBattery.TypeBattery))
        {
            return ServiceResponse.BadRequest("Battery type is required.");
        }

        var batteryType = await ResolveBatteryTypeAsync(createBattery.TypeBattery);
        var battery = new Battery
        {
            BatteryId = Guid.NewGuid(),
            SerialNumber = $"BAT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
            BatteryTypeId = batteryType.BatteryTypeId,
            CapacityKwh = createBattery.Capacity,
            StateOfHealth = createBattery.StateOfHealth ?? 100m,
            CurrentChargeLevel = createBattery.PercentUse ?? 100m,
            Status = createBattery.Status == false ? "MAINTENANCE" : "AVAILABLE",
            StationId = createBattery.StationId,
            InsuranceDate = createBattery.InsuranceDate,
            CreateDate = DateTime.UtcNow
        };

        _context.Batteries.Add(battery);
        station.CurrentBatteryCount += 1;
        await _context.SaveChangesAsync();

        battery.BatteryType = batteryType;
        battery.Station = station;
        return ServiceResponse.Created("Battery created successfully.", battery.ToDto());
    }

    public async Task<IServiceResult> GetBatteryCountByStationId(Guid stationId)
    {
        var count = await _context.Batteries.CountAsync(x => x.StationId == stationId);
        return ServiceResponse.Ok("Battery count retrieved successfully.", new { stationId, count });
    }

    public async Task<IServiceResult> DeleteBattery(Guid batteryId)
    {
        var battery = await _context.Batteries.FirstOrDefaultAsync(x => x.BatteryId == batteryId);
        if (battery is null)
        {
            return ServiceResponse.NotFound("Battery not found.");
        }

        _context.Batteries.Remove(battery);
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Battery deleted successfully.");
    }

    public async Task<IServiceResult> SoftDeleteBattery(Guid batteryId)
    {
        var battery = await _context.Batteries.FirstOrDefaultAsync(x => x.BatteryId == batteryId);
        if (battery is null)
        {
            return ServiceResponse.NotFound("Battery not found.");
        }

        battery.Status = "INACTIVE";
        battery.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Battery deactivated successfully.");
    }

    public async Task<IServiceResult> GetBatteriesByType(string typeBattery)
    {
        if (string.IsNullOrWhiteSpace(typeBattery))
        {
            return ServiceResponse.BadRequest("Battery type is required.");
        }

        var normalized = typeBattery.Trim().ToUpperInvariant();
        var batteries = await _context.Batteries
            .AsNoTracking()
            .Include(x => x.BatteryType)
            .Include(x => x.Station)
            .Where(x => x.BatteryType != null &&
                        (x.BatteryType.BatteryTypeName.ToUpper() == normalized ||
                         x.BatteryType.BatteryTypeCode.ToUpper() == normalized))
            .ToListAsync();

        return ServiceResponse.Ok("Batteries retrieved successfully.", batteries.Select(x => x.ToDto()).ToList());
    }

    public async Task<IServiceResult> GetBatteriesByStaffStationAsync(Guid staffAccountId)
    {
        var assignment = await _unitOfWork.AccountRepository.GetActiveStaffAssignmentAsync(staffAccountId);
        if (assignment is null)
        {
            return ServiceResponse.NotFound("Staff is not assigned to any station.");
        }

        var batteries = await _unitOfWork.BatteryRepository.GetStationInventoryAsync(assignment.StationId);
        return ServiceResponse.Ok("Station inventory retrieved successfully.", batteries.Select(x => x.ToDto()).ToList());
    }

    public async Task<IServiceResult> PreviewBatteryForBookingAsync(Guid stationId, Guid vehicleId)
    {
        var vehicle = await _unitOfWork.VehicleRepository.GetByIdWithDetailsAsync(vehicleId);
        if (vehicle?.Model?.BatteryTypeId is not Guid batteryTypeId)
        {
            return ServiceResponse.NotFound("Vehicle or compatible battery type not found.");
        }

        var battery = await _unitOfWork.BatteryRepository.GetBestAvailableBatteryAsync(stationId, batteryTypeId);
        return battery is null
            ? ServiceResponse.NotFound("No compatible battery available.")
            : ServiceResponse.Ok("Battery preview retrieved successfully.", battery.ToDto());
    }

    private async Task<BatteryType> ResolveBatteryTypeAsync(string batteryTypeName)
    {
        var normalized = batteryTypeName.Trim();
        var existing = await _context.BatteryTypes
            .FirstOrDefaultAsync(x => x.BatteryTypeName == normalized || x.BatteryTypeCode == normalized);

        if (existing is not null)
        {
            return existing;
        }

        var batteryType = new BatteryType
        {
            BatteryTypeId = Guid.NewGuid(),
            BatteryTypeCode = normalized.ToUpperInvariant().Replace(" ", "_"),
            BatteryTypeName = normalized,
            Status = "ACTIVE",
            CreateDate = DateTime.UtcNow
        };

        _context.BatteryTypes.Add(batteryType);
        await _context.SaveChangesAsync();
        return batteryType;
    }
}
