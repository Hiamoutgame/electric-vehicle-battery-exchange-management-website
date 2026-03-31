import axiosClient from "./axiosClient";
import { unsupportedOperation, unwrapApiData, unwrapArray } from "./apiHelpers";

const normalizeStatus = (status) => {
  const value = (status || "").toString().trim().toLowerCase();

  if (value === "approve" || value === "approved") return "Approved";
  if (value === "reject" || value === "rejected") return "Rejected";
  if (value === "complete" || value === "completed") return "Completed";
  if (value === "cancel" || value === "canceled") return "Canceled";
  if (value === "swap" || value === "swapped") return "Swapped";
  return "Pending";
};

const normalizeBooking = (booking) => {
  const status = normalizeStatus(booking?.status || booking?.isApproved);

  return {
    ...booking,
    bookingId: booking?.bookingId || "",
    stationId: booking?.stationId || "",
    stationName: booking?.stationName || "",
    vehicleId: booking?.vehicleId || "",
    batteryId: booking?.batteryId || booking?.requestedBatteryTypeId || "",
    dateTime: booking?.dateTime || booking?.bookingTime || booking?.targetTime || "",
    bookingTime: booking?.bookingTime || booking?.dateTime || booking?.targetTime || "",
    createdDate: booking?.createdDate || booking?.createDate || "",
    notes: booking?.notes || booking?.note || "",
    isApproved: status,
    status,
  };
};

const bookingService = {
  createBooking: async ({ dateTime, notes, stationId, vehicleId }) => {
    const response = await axiosClient.post("/driver/bookings", {
      stationId,
      vehicleId,
      bookingTime: dateTime,
      note: notes,
    });

    return normalizeBooking(unwrapApiData(response.data));
  },

  updateBooking: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API cập nhật booking từ phía frontend người dùng."
    ),

  selectAllBookings: async () => {
    const response = await axiosClient.get("/staff/bookings");
    return unwrapArray(response.data).map(normalizeBooking);
  },

  getUserBookings: async () => {
    const response = await axiosClient.get("/driver/bookings");
    return unwrapArray(response.data).map(normalizeBooking);
  },

  getBookingById: async (bookingId) => {
    const response = await axiosClient.get(`/driver/bookings/${bookingId}`);
    return normalizeBooking(unwrapApiData(response.data));
  },

  deleteBooking: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API hủy hoặc xóa booking cho driver."
    ),

  staffBookingsSchedule: async () => {
    const response = await axiosClient.get("/staff/bookings");
    return unwrapArray(response.data).map(normalizeBooking);
  },

  updateBookingStatus: async ({ bookingId, status, notes }) => {
    const normalizedDecision = (status || "").toUpperCase().startsWith("APPRO")
      ? "APPROVE"
      : "REJECT";

    const response = await axiosClient.patch(
      `/staff/bookings/${bookingId}/decision`,
      {
        decision: normalizedDecision,
        staffNote: notes || "",
      }
    );

    return normalizeBooking(unwrapApiData(response.data));
  },
};

export default bookingService;
