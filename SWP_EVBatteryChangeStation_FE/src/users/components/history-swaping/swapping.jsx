import React, { useEffect, useState } from "react";
import bookingService from "@/api/bookingService";
import swappingService from "@/api/swappingService";
import paymentService from "@/api/paymentService";
import subcriptionService from "@/api/subcriptionService";

const SwappingHistory = () => {
  const [bookings, setBookings] = useState([]);
  const [swaps, setSwaps] = useState([]);
  const [payments, setPayments] = useState([]);
  const [subscription, setSubscription] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      try {
        const [bookingItems, swapItems, paymentItems, currentSubscription] =
          await Promise.all([
            bookingService.getUserBookings(),
            swappingService.getAllSwapping(),
            paymentService.getPaymentsByAccountId(),
            subcriptionService.getMySubscription(),
          ]);

        setBookings(
          bookingItems.filter(
            (booking) =>
              booking.isApproved === "Approved" ||
              booking.isApproved === "Completed" ||
              booking.isApproved === "Swapped"
          )
        );
        setSwaps(swapItems);
        setPayments(paymentItems);
        setSubscription(currentSubscription);
      } catch {
        setBookings([]);
        setSwaps([]);
        setPayments([]);
        setSubscription(null);
      } finally {
        setLoading(false);
      }
    };

    load();
  }, []);

  if (loading) {
    return <div className="p-16 text-center">Đang tải lịch sử...</div>;
  }

  return (
    <div className="min-h-screen bg-gray-50 p-8">
      <div className="max-w-7xl mx-auto space-y-8">
        <div className="bg-white rounded-2xl border border-gray-200 p-8">
          <h1 className="text-4xl font-bold text-gray-900">Lịch sử đổi pin</h1>
          <p className="text-gray-600 mt-3">
            Màn này đang dùng các API còn tồn tại: bookings, swaps history,
            payments và current subscription.
          </p>
        </div>

        <div className="bg-white rounded-2xl border border-gray-200 p-8">
          <h2 className="text-2xl font-semibold text-gray-900 mb-4">
            Subscription hiện tại
          </h2>
          {subscription ? (
            <div className="space-y-2 text-gray-700">
              <p>Tên gói: {subscription.planName || subscription.name}</p>
              <p>Còn lại: {subscription.remainingSwaps ?? 0} lượt</p>
              <p>Kết thúc: {subscription.endDate || "-"}</p>
            </div>
          ) : (
            <p className="text-gray-600">Chưa có subscription đang hoạt động.</p>
          )}
        </div>

        <div className="grid lg:grid-cols-2 gap-8">
          <div className="bg-white rounded-2xl border border-gray-200 p-8">
            <h2 className="text-2xl font-semibold text-gray-900 mb-4">
              Booking đã xác nhận
            </h2>
            {bookings.length === 0 ? (
              <p className="text-gray-600">Chưa có booking phù hợp.</p>
            ) : (
              <div className="space-y-3">
                {bookings.map((booking) => (
                  <div key={booking.bookingId} className="border rounded-xl p-4">
                    <p className="font-semibold">{booking.stationName || "Station"}</p>
                    <p className="text-gray-600">{booking.dateTime || "-"}</p>
                    <p className="text-gray-600">Status: {booking.isApproved}</p>
                  </div>
                ))}
              </div>
            )}
          </div>

          <div className="bg-white rounded-2xl border border-gray-200 p-8">
            <h2 className="text-2xl font-semibold text-gray-900 mb-4">
              Swap transactions
            </h2>
            {swaps.length === 0 ? (
              <p className="text-gray-600">Chưa có swap transaction nào.</p>
            ) : (
              <div className="space-y-3">
                {swaps.map((swap, index) => (
                  <div
                    key={swap.swapTransactionId || swap.transactionId || index}
                    className="border rounded-xl p-4"
                  >
                    <p className="font-semibold">
                      Transaction: {swap.swapTransactionId || swap.transactionId}
                    </p>
                    <p className="text-gray-600">
                      Vehicle: {swap.vehicleId || "-"}
                    </p>
                    <p className="text-gray-600">Status: {swap.status || "-"}</p>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        <div className="bg-white rounded-2xl border border-gray-200 p-8">
          <h2 className="text-2xl font-semibold text-gray-900 mb-4">
            Payment history
          </h2>
          {payments.length === 0 ? (
            <p className="text-gray-600">Chưa có payment nào.</p>
          ) : (
            <div className="space-y-3">
              {payments.map((payment) => (
                <div key={payment.paymentId} className="border rounded-xl p-4">
                  <p className="font-semibold">{payment.method || "Payment"}</p>
                  <p className="text-gray-600">
                    {(payment.price || 0).toLocaleString("vi-VN")} đ
                  </p>
                  <p className="text-gray-600">{String(payment.status)}</p>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default SwappingHistory;
