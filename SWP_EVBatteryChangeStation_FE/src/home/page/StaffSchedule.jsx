import React, { useCallback, useEffect, useState } from "react";
import bookingService from "@/api/bookingService";
import swappingService from "@/api/swappingService";
import { notifySuccess, notifyError } from "@/components/notification/notification";

const StaffSchedule = () => {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(false);
  const [activeBooking, setActiveBooking] = useState(null);

  const loadBookings = useCallback(async () => {
    try {
      setLoading(true);
      setBookings(await bookingService.staffBookingsSchedule());
    } catch (error) {
      setBookings([]);
      notifyError(
        error?.response?.data?.message || "Không thể tải danh sách booking."
      );
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadBookings();
  }, [loadBookings]);

  const processDecision = async (bookingId, status) => {
    try {
      await bookingService.updateBookingStatus({
        bookingId,
        status,
        notes: `Processed by staff: ${status}`,
      });
      notifySuccess(`Đã ${status === "Approved" ? "duyệt" : "từ chối"} booking.`);
      setActiveBooking(null);
      loadBookings();
    } catch (error) {
      notifyError(error?.response?.data?.message || "Cập nhật booking thất bại.");
    }
  };

  const completeSwap = async (bookingId) => {
    try {
      await swappingService.confirmAndSwap({
        bookingId,
        notes: `Complete swap for booking ${bookingId}`,
      });
      notifySuccess("Đổi pin thành công.");
      setActiveBooking(null);
      loadBookings();
    } catch (error) {
      notifyError(error?.response?.data?.message || "Không thể hoàn tất đổi pin.");
    }
  };

  const formatDateTime = (value) => {
    if (!value) return "-";
    return new Intl.DateTimeFormat("vi-VN", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
    }).format(new Date(value));
  };

  return (
    <div className="min-h-screen bg-gray-50 p-8">
      <div className="max-w-7xl mx-auto">
        <div className="mb-10">
          <h1 className="text-4xl font-bold text-gray-900">Booking của trạm</h1>
          <p className="text-gray-600 mt-2">
            Staff chỉ được xem và xử lý booking thuộc trạm được gán.
          </p>
        </div>

        {loading ? (
          <div className="bg-white rounded-2xl border border-gray-200 p-16 text-center">
            Đang tải dữ liệu...
          </div>
        ) : bookings.length === 0 ? (
          <div className="bg-white rounded-2xl border border-gray-200 p-16 text-center">
            Không có booking nào cho trạm hiện tại.
          </div>
        ) : (
          <div className="grid gap-6">
            {bookings.map((booking) => (
              <div
                key={booking.bookingId}
                className="bg-white rounded-2xl border border-gray-200 p-6"
              >
                <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
                  <div>
                    <h2 className="text-2xl font-semibold text-gray-900">
                      {booking.stationName || "Assigned station"}
                    </h2>
                    <p className="text-gray-600 mt-1">
                      Booking time: {formatDateTime(booking.dateTime)}
                    </p>
                    <p className="text-gray-600 mt-1">
                      Vehicle ID: {booking.vehicleId || "N/A"}
                    </p>
                    <p className="text-gray-600 mt-1">
                      Ghi chú: {booking.notes || "-"}
                    </p>
                  </div>

                  <div className="flex items-center gap-3">
                    <span className="px-4 py-2 rounded-full bg-blue-50 text-blue-700 font-semibold">
                      {booking.isApproved}
                    </span>
                    <button
                      className="px-4 py-2 rounded-lg border border-gray-300 hover:bg-gray-50"
                      onClick={() => setActiveBooking(booking)}
                    >
                      Xử lý
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {activeBooking && (
          <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-2xl shadow-xl w-full max-w-2xl p-8">
              <h2 className="text-3xl font-bold text-gray-900 mb-4">
                Xử lý booking
              </h2>
              <div className="space-y-3 text-gray-700">
                <p>Booking ID: {activeBooking.bookingId}</p>
                <p>Station: {activeBooking.stationName || "Assigned station"}</p>
                <p>Time: {formatDateTime(activeBooking.dateTime)}</p>
                <p>Notes: {activeBooking.notes || "-"}</p>
                <p>Status: {activeBooking.isApproved}</p>
              </div>

              <div className="mt-8 flex flex-wrap gap-3">
                {activeBooking.isApproved === "Pending" && (
                  <>
                    <button
                      className="px-4 py-2 rounded-lg bg-blue-600 text-white hover:bg-blue-700"
                      onClick={() => processDecision(activeBooking.bookingId, "Approved")}
                    >
                      Duyệt
                    </button>
                    <button
                      className="px-4 py-2 rounded-lg bg-red-600 text-white hover:bg-red-700"
                      onClick={() => processDecision(activeBooking.bookingId, "Rejected")}
                    >
                      Từ chối
                    </button>
                  </>
                )}

                {activeBooking.isApproved === "Approved" && (
                  <button
                    className="px-4 py-2 rounded-lg bg-green-600 text-white hover:bg-green-700"
                    onClick={() => completeSwap(activeBooking.bookingId)}
                  >
                    Xác nhận đổi pin
                  </button>
                )}

                <button
                  className="px-4 py-2 rounded-lg border border-gray-300 hover:bg-gray-50"
                  onClick={() => setActiveBooking(null)}
                >
                  Đóng
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default StaffSchedule;
