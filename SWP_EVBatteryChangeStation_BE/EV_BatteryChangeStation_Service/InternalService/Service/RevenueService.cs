using EV_BatteryChangeStation_Common.DTOs.RevenueDTO;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.InternalService.IService;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class RevenueService : IRevenueService
{
    private readonly IUnitOfWork _unitOfWork;

    public RevenueService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<RevenueByStationDto>> GetRevenueByStationAsync()
    {
        var items = await _unitOfWork.ReportRepository.GetRevenueReportAsync(
            DateTime.UtcNow.AddYears(-1),
            DateTime.UtcNow,
            "station");

        return items.Select(x => new RevenueByStationDto
        {
            StationId = x.StationId ?? Guid.Empty,
            StationName = x.StationName ?? x.Label,
            TotalRevenue = x.Revenue,
            TotalTransaction = x.Label
        }).ToList();
    }
}
