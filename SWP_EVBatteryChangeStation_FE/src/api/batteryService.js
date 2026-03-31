import stationService from "./stationService";
import carService from "./carService";
import axiosClient from "./axiosClient";
import { unsupportedOperation, unwrapApiData, unwrapArray } from "./apiHelpers";

const normalizeBattery = (battery) => ({
  ...battery,
  batteryId: battery?.batteryId || "",
  stationId: battery?.stationId || "",
  typeBattery: battery?.typeBattery || battery?.batteryTypeId || "",
  status: battery?.status ?? true,
});

const batteryService = {
  getAllBatteries: async () =>
    unsupportedOperation(
      "Backend hiện không có API public lấy toàn bộ pin trong bộ route /api/v1."
    ),

  getMyStationBatteries: async () => {
    const response = await axiosClient.get("/staff/inventory");
    return unwrapArray(response.data).map(normalizeBattery);
  },

  getBatteryById: async (batteryId) => {
    const batteries = await batteryService.getMyStationBatteries();
    return batteries.find((battery) => battery.batteryId === batteryId) || null;
  },

  getBatteriesByType: async (batteryType) => {
    const batteries = await batteryService.getMyStationBatteries();
    const normalizedType = (batteryType || "").toLowerCase();
    return batteries.filter(
      (battery) => (battery.typeBattery || "").toLowerCase() === normalizedType
    );
  },

  getBatteryCountByStationId: async (stationId) => {
    const stations = await stationService.getStationList();
    const station = stations.find((item) => item.stationId === stationId);
    return station?.batteryQuantity ?? 0;
  },

  getBatteriesByStationId: async (stationId) => {
    const batteries = await batteryService.getMyStationBatteries();
    return batteries.filter((battery) => battery.stationId === stationId);
  },

  updateBattery: async (batteryId, payload) => {
    const response = await axiosClient.patch(`/staff/batteries/${batteryId}/status`, {
      status: payload?.status ?? false,
      reason: payload?.reason || "",
    });

    return unwrapApiData(response.data);
  },

  previewForBooking: async (stationId, vehicleId) => {
    const [stationCount, car] = await Promise.all([
      batteryService.getBatteryCountByStationId(stationId),
      carService.getCarById(vehicleId),
    ]);

    return {
      isAvailable: stationCount > 0,
      batteryType: car?.batteryType || "Compatible battery",
      batteryId: null,
    };
  },
};

export default batteryService;
