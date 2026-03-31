import axiosClient from "./axiosClient";
import carService from "./carService";
import { unsupportedOperation, unwrapApiData, unwrapArray } from "./apiHelpers";

const swappingService = {
  createSwapping: async () =>
    unsupportedOperation(
      "Backend hiện chỉ hỗ trợ staff xác nhận hoàn tất đổi pin qua /staff/swaps/complete."
    ),

  updateSwapping: async () =>
    unsupportedOperation(
      "Backend hiện không có API cập nhật swap transaction cho frontend."
    ),

  getAllSwapping: async () => {
    const vehicles = await carService.getAllCars();
    const swapGroups = await Promise.all(
      vehicles.map(async (vehicle) => {
        try {
          const response = await axiosClient.get("/driver/swaps/history", {
            params: { vehicleId: vehicle.vehicleId },
          });
          return unwrapArray(response.data);
        } catch {
          return [];
        }
      })
    );

    return swapGroups.flat();
  },

  getSwappingByTransactionId: async (transactionId) => {
    const swaps = await swappingService.getAllSwapping();
    return (
      swaps.find(
        (swap) =>
          swap.transactionId === transactionId ||
          swap.swapTransactionId === transactionId
      ) || null
    );
  },

  getSwappingByBookingId: async (bookingId) => {
    const swaps = await swappingService.getAllSwapping();
    return swaps.find((swap) => swap.bookingId === bookingId) || null;
  },

  createSwappingFromBooking: async () =>
    unsupportedOperation(
      "Backend hiện chỉ hỗ trợ staff xác nhận hoàn tất đổi pin qua /staff/swaps/complete."
    ),

  confirmAndSwap: async ({ bookingId, notes }) => {
    const response = await axiosClient.post("/staff/swaps/complete", {
      bookingId,
      note: notes || "",
    });

    return unwrapApiData(response.data);
  },
};

export default swappingService;
