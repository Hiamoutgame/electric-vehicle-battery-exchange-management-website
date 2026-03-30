using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;

namespace EV_BatteryChangeStation.Controllers;

[Route("api/v1/stations")]
public sealed class StationsController : ApiControllerBase
{
    private readonly IStationService _stationService;

    public StationsController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStations([FromQuery] string? keyword)
    {
        var result = string.IsNullOrWhiteSpace(keyword)
            ? await _stationService.GetAllAsync()
            : await _stationService.SearchByNameAsync(keyword);

        return ApiResult(result, "STATION_LIST_FETCHED", "STATION_LIST_FAILED");
    }

    [HttpGet("{stationId:guid}")]
    public async Task<IActionResult> GetStation(Guid stationId)
    {
        var result = await _stationService.GetByIdAsync(stationId);
        return ApiResult(result, "STATION_DETAIL_FETCHED", "STATION_NOT_FOUND");
    }
}
