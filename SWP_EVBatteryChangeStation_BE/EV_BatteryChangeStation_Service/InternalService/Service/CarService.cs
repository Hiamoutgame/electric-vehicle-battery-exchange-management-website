using EV_BatteryChangeStation_Common.DTOs.CarDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class CarService : ICarService
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CarService(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult> AddCarAsync(CreateCarDto createCar)
    {
        if (!createCar.OwnerId.HasValue || createCar.OwnerId == Guid.Empty)
        {
            return ServiceResponse.BadRequest("OwnerId is required.");
        }

        if (string.IsNullOrWhiteSpace(createCar.Model) ||
            string.IsNullOrWhiteSpace(createCar.BatteryType) ||
            string.IsNullOrWhiteSpace(createCar.LicensePlate))
        {
            return ServiceResponse.BadRequest("Model, battery type and license plate are required.");
        }

        var ownerExists = await _context.Accounts.AnyAsync(x => x.AccountId == createCar.OwnerId.Value);
        if (!ownerExists)
        {
            return ServiceResponse.NotFound("Owner account not found.");
        }

        if (await _unitOfWork.VehicleRepository.ExistsLicensePlateAsync(createCar.LicensePlate))
        {
            return ServiceResponse.Conflict("License plate already exists.");
        }

        if (!string.IsNullOrWhiteSpace(createCar.Vin) && await _unitOfWork.VehicleRepository.ExistsVinAsync(createCar.Vin))
        {
            return ServiceResponse.Conflict("VIN already exists.");
        }

        var batteryType = await ResolveBatteryTypeAsync(createCar.BatteryType);
        var model = await ResolveVehicleModelAsync(createCar.Model, createCar.Producer, batteryType.BatteryTypeId);

        var vehicle = new Vehicle
        {
            VehicleId = Guid.NewGuid(),
            OwnerId = createCar.OwnerId.Value,
            ModelId = model.ModelId,
            Vin = createCar.Vin?.Trim(),
            LicensePlate = createCar.LicensePlate.Trim().ToUpperInvariant(),
            Status = string.IsNullOrWhiteSpace(createCar.Status) ? "ACTIVE" : createCar.Status.Trim().ToUpperInvariant(),
            CreateDate = createCar.CreateDate ?? DateTime.UtcNow
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        vehicle.Model = model;
        return ServiceResponse.Created("Vehicle created successfully.", vehicle.ToDto());
    }

    public async Task<IServiceResult> GetAllCarsAsync()
    {
        var vehicles = await _context.Vehicles
            .AsNoTracking()
            .Include(x => x.Model)
                .ThenInclude(x => x!.BatteryType)
            .Include(x => x.CurrentBattery)
            .OrderByDescending(x => x.CreateDate)
            .ToListAsync();

        return ServiceResponse.Ok("Vehicles retrieved successfully.", vehicles.Select(x => x.ToDto()).ToList());
    }

    public async Task<IServiceResult> GetCarByIdAsync(Guid carId)
    {
        var vehicle = await _unitOfWork.VehicleRepository.GetByIdWithDetailsAsync(carId);
        return vehicle is null
            ? ServiceResponse.NotFound("Vehicle not found.")
            : ServiceResponse.Ok("Vehicle retrieved successfully.", vehicle.ToDto());
    }

    public async Task<IServiceResult> UpdateCarAsync(UpdateCarDto updateCarDto)
    {
        var vehicle = await _context.Vehicles
            .Include(x => x.Model)
                .ThenInclude(x => x!.BatteryType)
            .FirstOrDefaultAsync(x => x.VehicleId == updateCarDto.VehicleId);

        if (vehicle is null)
        {
            return ServiceResponse.NotFound("Vehicle not found.");
        }

        if (!string.IsNullOrWhiteSpace(updateCarDto.LicensePlate) &&
            !string.Equals(updateCarDto.LicensePlate.Trim(), vehicle.LicensePlate, StringComparison.OrdinalIgnoreCase))
        {
            if (await _unitOfWork.VehicleRepository.ExistsLicensePlateAsync(updateCarDto.LicensePlate, vehicle.VehicleId))
            {
                return ServiceResponse.Conflict("License plate already exists.");
            }

            vehicle.LicensePlate = updateCarDto.LicensePlate.Trim().ToUpperInvariant();
        }

        if (!string.IsNullOrWhiteSpace(updateCarDto.Vin) &&
            !string.Equals(updateCarDto.Vin.Trim(), vehicle.Vin, StringComparison.OrdinalIgnoreCase))
        {
            if (await _unitOfWork.VehicleRepository.ExistsVinAsync(updateCarDto.Vin, vehicle.VehicleId))
            {
                return ServiceResponse.Conflict("VIN already exists.");
            }

            vehicle.Vin = updateCarDto.Vin.Trim();
        }

        if (!string.IsNullOrWhiteSpace(updateCarDto.Model) && !string.IsNullOrWhiteSpace(updateCarDto.BatteryType))
        {
            var batteryType = await ResolveBatteryTypeAsync(updateCarDto.BatteryType);
            var model = await ResolveVehicleModelAsync(updateCarDto.Model, updateCarDto.Producer, batteryType.BatteryTypeId);
            vehicle.ModelId = model.ModelId;
            vehicle.Model = model;
        }

        if (updateCarDto.OwnerId.HasValue && updateCarDto.OwnerId.Value != Guid.Empty)
        {
            vehicle.OwnerId = updateCarDto.OwnerId.Value;
        }

        if (!string.IsNullOrWhiteSpace(updateCarDto.Status))
        {
            vehicle.Status = updateCarDto.Status.Trim().ToUpperInvariant();
        }

        vehicle.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ServiceResponse.Ok("Vehicle updated successfully.", vehicle.ToDto());
    }

    public async Task<IServiceResult> DeleteCarAsync(Guid carId)
    {
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(x => x.VehicleId == carId);
        if (vehicle is null)
        {
            return ServiceResponse.NotFound("Vehicle not found.");
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Vehicle deleted successfully.");
    }

    public async Task<IServiceResult> SoftDeleteCarAsync(Guid carid)
    {
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(x => x.VehicleId == carid);
        if (vehicle is null)
        {
            return ServiceResponse.NotFound("Vehicle not found.");
        }

        vehicle.Status = "INACTIVE";
        vehicle.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Vehicle deactivated successfully.");
    }

    public async Task<IServiceResult> GetOwnerByCarIdAsync(Guid carid)
    {
        var vehicle = await _context.Vehicles
            .AsNoTracking()
            .Include(x => x.Owner)
                .ThenInclude(x => x!.Role)
            .FirstOrDefaultAsync(x => x.VehicleId == carid);

        return vehicle?.Owner is null
            ? ServiceResponse.NotFound("Vehicle owner not found.")
            : ServiceResponse.Ok("Vehicle owner retrieved successfully.", vehicle.Owner.ToViewDto());
    }

    public async Task<IServiceResult> GetCarByNameAsync(string modelName)
    {
        if (string.IsNullOrWhiteSpace(modelName))
        {
            return ServiceResponse.BadRequest("Model name is required.");
        }

        var vehicles = await _context.Vehicles
            .AsNoTracking()
            .Include(x => x.Model)
                .ThenInclude(x => x!.BatteryType)
            .Where(x => x.Model != null && x.Model.ModelName.ToUpper().Contains(modelName.Trim().ToUpper()))
            .ToListAsync();

        return ServiceResponse.Ok("Vehicles retrieved successfully.", vehicles.Select(x => x.ToDto()).ToList());
    }

    public async Task<IServiceResult> GetCarsByOwnerIdAsync(Guid ownerId)
    {
        var vehicles = await _unitOfWork.VehicleRepository.GetByOwnerAsync(ownerId);
        return ServiceResponse.Ok("Vehicles retrieved successfully.", vehicles.Select(x => x.ToDto()).ToList());
    }

    public async Task<IServiceResult> GetBatteriesByCarAsync(Guid vehicle)
    {
        var car = await _unitOfWork.VehicleRepository.GetByIdWithDetailsAsync(vehicle);
        if (car is null)
        {
            return ServiceResponse.NotFound("Vehicle not found.");
        }

        if (car.CurrentBattery is null)
        {
            return ServiceResponse.NotFound("Vehicle does not have a current battery.");
        }

        return ServiceResponse.Ok("Current battery retrieved successfully.", car.CurrentBattery.ToDto());
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

    private async Task<VehicleModel> ResolveVehicleModelAsync(string modelName, string? producer, Guid batteryTypeId)
    {
        var normalizedModel = modelName.Trim();
        var normalizedProducer = producer?.Trim();

        var existing = await _context.VehicleModels.FirstOrDefaultAsync(x =>
            x.ModelName == normalizedModel &&
            x.BatteryTypeId == batteryTypeId &&
            (x.Producer ?? string.Empty) == (normalizedProducer ?? string.Empty));

        if (existing is not null)
        {
            return existing;
        }

        var model = new VehicleModel
        {
            ModelId = Guid.NewGuid(),
            ModelName = normalizedModel,
            Producer = normalizedProducer,
            BatteryTypeId = batteryTypeId,
            Status = "ACTIVE",
            CreateDate = DateTime.UtcNow
        };

        _context.VehicleModels.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }
}
