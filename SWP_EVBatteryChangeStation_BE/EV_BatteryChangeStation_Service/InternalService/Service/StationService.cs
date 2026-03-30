using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class StationService : IStationService
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public StationService(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> GetAllAsync()
    {
        var stations = await _unitOfWork.StationRepository.GetActiveStationsAsync();
        return ServiceResponse.Ok("Stations retrieved successfully.", stations.Select(x => x.ToDto()).ToList());
    }

    public async Task<ServiceResult> GetByIdAsync(Guid id)
    {
        var station = await _unitOfWork.StationRepository.GetStationDetailAsync(id);
        return station is null
            ? ServiceResponse.NotFound("Station not found.")
            : ServiceResponse.Ok("Station retrieved successfully.", station.ToDto());
    }

    public async Task<ServiceResult> CreateAsync(StationCreateDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.StationName) || string.IsNullOrWhiteSpace(dto.Address))
        {
            return ServiceResponse.BadRequest("Station name and address are required.");
        }

        var station = new Station
        {
            StationId = Guid.NewGuid(),
            StationName = dto.StationName.Trim(),
            Address = dto.Address.Trim(),
            PhoneNumber = dto.PhoneNumber?.Trim(),
            Status = LegacyStatusMapper.ToActiveStatus(dto.Status),
            MaxCapacity = dto.BatteryQuantity,
            CurrentBatteryCount = 0,
            CreateDate = DateTime.UtcNow
        };

        _context.Stations.Add(station);
        await _context.SaveChangesAsync();
        return ServiceResponse.Created("Station created successfully.", station.ToDto());
    }

    public async Task<ServiceResult> UpdateAsync(Guid id, StationCreateDTO dto)
    {
        var station = await _context.Stations.FirstOrDefaultAsync(x => x.StationId == id);
        if (station is null)
        {
            return ServiceResponse.NotFound("Station not found.");
        }

        if (!string.IsNullOrWhiteSpace(dto.StationName))
        {
            station.StationName = dto.StationName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.Address))
        {
            station.Address = dto.Address.Trim();
        }

        station.PhoneNumber = dto.PhoneNumber?.Trim() ?? station.PhoneNumber;
        station.Status = dto.Status.HasValue ? LegacyStatusMapper.ToActiveStatus(dto.Status.Value) : station.Status;
        station.MaxCapacity = dto.BatteryQuantity ?? station.MaxCapacity;
        station.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Station updated successfully.", station.ToDto());
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var station = await _context.Stations.FirstOrDefaultAsync(x => x.StationId == id);
        if (station is null)
        {
            return ServiceResponse.NotFound("Station not found.");
        }

        station.Status = "INACTIVE";
        station.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Station deactivated successfully.");
    }

    public async Task<ServiceResult> HardDeleteAsync(Guid id)
    {
        var station = await _context.Stations.FirstOrDefaultAsync(x => x.StationId == id);
        if (station is null)
        {
            return ServiceResponse.NotFound("Station not found.");
        }

        _context.Stations.Remove(station);
        await _context.SaveChangesAsync();
        return ServiceResponse.Ok("Station deleted successfully.");
    }

    public async Task<ServiceResult> SearchByNameAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return ServiceResponse.BadRequest("Keyword is required.");
        }

        var stations = await _unitOfWork.StationRepository.GetActiveStationsAsync(keyword: keyword);
        return ServiceResponse.Ok("Stations retrieved successfully.", stations.Select(x => x.ToDto()).ToList());
    }

    public async Task<ServiceResult> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ServiceResponse.BadRequest("Station name is required.");
        }

        var stations = await _unitOfWork.StationRepository.GetActiveStationsAsync(keyword: name);
        var station = stations.FirstOrDefault(x => string.Equals(x.StationName, name.Trim(), StringComparison.OrdinalIgnoreCase));
        return station is null
            ? ServiceResponse.NotFound("Station not found.")
            : ServiceResponse.Ok("Station retrieved successfully.", station.ToDto());
    }
}
