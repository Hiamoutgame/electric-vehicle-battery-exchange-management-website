import React, { useCallback, useEffect, useMemo, useState } from "react";
import BookingForm from "@/users/components/booking-form/BookingForm";
import bookingService from "@/api/bookingService";

const BookingPage = () => {
  const [bookings, setBookings] = useState([]);
  const [currentPage, setCurrentPage] = useState(0);
  const [showForm, setShowForm] = useState(false);
  const [loading, setLoading] = useState(false);

  const ITEMS_PER_PAGE = 5;

  const fetchBookings = useCallback(async () => {
    try {
      setLoading(true);
      setBookings(await bookingService.getUserBookings());
    } catch {
      setBookings([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchBookings();
  }, [fetchBookings]);

  const processedBookings = useMemo(() => {
    const items = [...bookings];
    items.sort(
      (left, right) =>
        new Date(right.createdDate || right.dateTime) -
        new Date(left.createdDate || left.dateTime)
    );
    return items;
  }, [bookings]);

  const totalPages = Math.ceil(processedBookings.length / ITEMS_PER_PAGE);
  const startIndex = currentPage * ITEMS_PER_PAGE;
  const currentBookings = processedBookings.slice(
    startIndex,
    startIndex + ITEMS_PER_PAGE
  );

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

  const statusStyles = {
    Pending: "bg-gray-100 text-gray-700 border-gray-200",
    Approved: "bg-blue-100 text-blue-800 border-blue-200",
    Completed: "bg-green-100 text-green-800 border-green-200",
    Swapped: "bg-green-100 text-green-800 border-green-200",
    Rejected: "bg-red-100 text-red-800 border-red-200",
    Canceled: "bg-red-100 text-red-800 border-red-200",
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="flex justify-between items-center mb-12">
          <div>
            <h1 className="text-5xl font-bold text-gray-900">Lịch Đổi Pin</h1>
            <p className="text-xl text-gray-600 mt-3">
              Theo dõi trạng thái booking theo đúng flow backend hiện tại.
            </p>
          </div>

          <button
            onClick={() => setShowForm(true)}
            className="bg-blue-600 hover:bg-blue-700 text-white font-medium text-lg px-7 py-4 rounded-xl shadow-sm"
          >
            Đặt lịch mới
          </button>
        </div>

        {loading && (
          <div className="bg-white rounded-2xl border border-gray-200 p-24 text-center">
            <div className="w-14 h-14 border-4 border-gray-200 border-t-blue-600 rounded-full animate-spin mx-auto" />
            <p className="text-xl text-gray-600 mt-6">Đang tải dữ liệu...</p>
          </div>
        )}

        {!loading && processedBookings.length === 0 && (
          <div className="bg-white rounded-2xl border border-gray-200 p-24 text-center">
            <h3 className="text-3xl font-semibold text-gray-800 mb-3">
              Chưa có lịch đặt nào
            </h3>
            <p className="text-xl text-gray-500 mb-10">
              Tạo booking đầu tiên để bắt đầu sử dụng dịch vụ.
            </p>
            <button
              onClick={() => setShowForm(true)}
              className="bg-blue-600 hover:bg-blue-700 text-white font-medium text-lg px-8 py-4 rounded-xl"
            >
              Tạo lịch đặt mới
            </button>
          </div>
        )}

        {!loading && processedBookings.length > 0 && (
          <>
            <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-8 mb-12">
              {currentBookings.map((booking) => (
                <div
                  key={booking.bookingId}
                  className="bg-white rounded-2xl border border-gray-200 overflow-hidden"
                >
                  <div className="px-6 py-4 border-b border-gray-100 bg-gray-50 flex justify-between items-center">
                    <span
                      className={`px-4 py-2 rounded-lg text-lg font-semibold border ${
                        statusStyles[booking.isApproved] || statusStyles.Pending
                      }`}
                    >
                      {booking.isApproved}
                    </span>
                  </div>

                  <div className="p-6 space-y-4">
                    <div>
                      <p className="text-lg text-gray-500 font-medium mb-1">
                        Trạm
                      </p>
                      <p className="text-2xl font-bold text-gray-900">
                        {booking.stationName || "Đang cập nhật"}
                      </p>
                    </div>

                    <div>
                      <p className="text-lg text-gray-500 font-medium mb-1">
                        Thời gian
                      </p>
                      <p className="text-xl text-blue-600 font-semibold">
                        {formatDateTime(booking.dateTime)}
                      </p>
                    </div>

                    <div>
                      <p className="text-lg text-gray-500 font-medium mb-1">
                        Ghi chú
                      </p>
                      <p className="text-gray-700">{booking.notes || "-"}</p>
                    </div>

                    <div className="text-gray-500">
                      Tạo ngày: {formatDateTime(booking.createdDate)}
                    </div>

                    <div className="rounded-xl bg-blue-50 border border-blue-100 p-4 text-sm text-blue-800">
                      Driver hiện chưa có API hủy hoặc sửa booking. FE chỉ hỗ
                      trợ tạo mới và theo dõi trạng thái.
                    </div>
                  </div>
                </div>
              ))}
            </div>

            {totalPages > 1 && (
              <div className="bg-white rounded-2xl border border-gray-200 p-6 flex justify-center gap-3">
                {Array.from({ length: totalPages }, (_, index) => (
                  <button
                    key={index}
                    onClick={() => setCurrentPage(index)}
                    className={`px-5 py-3 rounded-lg text-lg font-medium ${
                      currentPage === index
                        ? "bg-blue-600 text-white"
                        : "border border-gray-300 hover:bg-gray-50"
                    }`}
                  >
                    {index + 1}
                  </button>
                ))}
              </div>
            )}
          </>
        )}

        {showForm && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-2xl shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
              <div className="p-8 border-b border-gray-200">
                <div className="flex justify-between items-center">
                  <h2 className="text-4xl font-bold text-gray-900">
                    Đặt lịch đổi pin
                  </h2>
                  <button
                    onClick={() => setShowForm(false)}
                    className="text-gray-500 hover:text-gray-700 text-3xl"
                  >
                    ×
                  </button>
                </div>
              </div>
              <div className="p-8">
                <BookingForm
                  onSuccess={() => {
                    setShowForm(false);
                    fetchBookings();
                  }}
                  onCancel={() => setShowForm(false)}
                />
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default BookingPage;
