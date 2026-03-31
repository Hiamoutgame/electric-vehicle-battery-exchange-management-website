import axiosClient from "./axiosClient";
import { unwrapApiData, unwrapArray } from "./apiHelpers";

const normalizeStation = (station) => ({
  ...station,
  stationId: station?.stationId || station?.id || "",
  stationName: station?.stationName || station?.name || "",
  name: station?.stationName || station?.name || "",
  accountName: station?.stationName || station?.name || "",
  address: station?.address || "",
  phoneNumber: station?.phoneNumber || "",
  batteryQuantity: station?.batteryQuantity ?? 0,
  status: station?.status ?? true,
});

const stationService = {
  getStationList: async (keyword = "") => {
    const response = await axiosClient.get("/stations", {
      params: keyword ? { keyword } : undefined,
    });

    return unwrapArray(response.data).map(normalizeStation);
  },

  getStationDetail: async (stationId) => {
    const response = await axiosClient.get(`/stations/${stationId}`);
    return normalizeStation(unwrapApiData(response.data));
  },

  createStation: async (payload) => {
    const response = await axiosClient.post("/admin/stations", payload);
    return unwrapApiData(response.data);
  },

  updateStation: async (stationId, payload) => {
    const response = await axiosClient.patch(`/admin/stations/${stationId}`, payload);
    return unwrapApiData(response.data);
  },

  deleteStation: async (stationId) => {
    const response = await axiosClient.delete(`/admin/stations/${stationId}`);
    return unwrapApiData(response.data);
  },

  getStationsByName: async (keyword) => {
    return stationService.getStationList(keyword);
  },
};

export default stationService;
