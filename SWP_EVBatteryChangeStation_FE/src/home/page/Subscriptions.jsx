import React, { useEffect, useState } from "react";
import paymentService from "@/api/paymentService";
import tokenUtils from "@/utils/tokenUtils";

const Subscriptions = () => {
  const [loading, setLoading] = useState(true);
  const [currentSubscription, setCurrentSubscription] = useState(null);
  const [payments, setPayments] = useState([]);

  useEffect(() => {
    const load = async () => {
      try {
        if (tokenUtils.getToken()) {
          const [subscription, paymentHistory] = await Promise.all([
            paymentService.getCurrentSubscription(),
            paymentService.getPaymentsByAccountId(),
          ]);
          setCurrentSubscription(subscription);
          setPayments(paymentHistory);
        }
      } catch {
        setCurrentSubscription(null);
        setPayments([]);
      } finally {
        setLoading(false);
      }
    };

    load();
  }, []);

  if (loading) {
    return <div className="p-16 text-center">Đang tải dữ liệu subscription...</div>;
  }

  return (
    <div className="min-h-screen bg-gray-50 px-6 py-16">
      <div className="max-w-5xl mx-auto space-y-8">
        <div className="bg-white rounded-2xl border border-gray-200 p-8">
          <h1 className="text-4xl font-bold text-gray-900">Subscription</h1>
          <p className="text-gray-600 mt-3">
            Backend hiện chỉ có endpoint xem gói đang hoạt động qua
            `/api/v1/driver/subscriptions/current`.
          </p>
        </div>

        <div className="bg-white rounded-2xl border border-gray-200 p-8">
          <h2 className="text-2xl font-semibold text-gray-900 mb-4">
            Gói hiện tại
          </h2>

          {currentSubscription ? (
            <div className="space-y-3 text-gray-700">
              <p>Tên gói: {currentSubscription.planName || currentSubscription.name}</p>
              <p>Trạng thái: {currentSubscription.status || "ACTIVE"}</p>
              <p>Còn lại: {currentSubscription.remainingSwaps ?? 0} lượt</p>
              <p>Đã dùng: {currentSubscription.usedSwaps ?? 0} lượt</p>
              <p>Bắt đầu: {currentSubscription.startDate || "-"}</p>
              <p>Kết thúc: {currentSubscription.endDate || "-"}</p>
            </div>
          ) : (
            <div className="rounded-xl bg-amber-50 border border-amber-200 p-4 text-amber-800">
              Tài khoản này chưa có subscription đang hoạt động.
            </div>
          )}
        </div>

        <div className="bg-white rounded-2xl border border-gray-200 p-8">
          <h2 className="text-2xl font-semibold text-gray-900 mb-4">
            Lịch sử payment
          </h2>

          {payments.length === 0 ? (
            <p className="text-gray-600">Chưa có payment nào theo API hiện tại.</p>
          ) : (
            <div className="space-y-3">
              {payments.map((payment) => (
                <div
                  key={payment.paymentId}
                  className="rounded-xl border border-gray-200 p-4 flex flex-col md:flex-row md:items-center md:justify-between gap-3"
                >
                  <div>
                    <p className="font-semibold text-gray-900">
                      {payment.method || "Payment"}
                    </p>
                    <p className="text-gray-600">{payment.createDate || "-"}</p>
                  </div>
                  <div className="text-right">
                    <p className="font-semibold text-blue-700">
                      {(payment.price || 0).toLocaleString("vi-VN")} đ
                    </p>
                    <p className="text-gray-600">{String(payment.status)}</p>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="bg-blue-50 rounded-2xl border border-blue-200 p-8 text-blue-900">
          Màn mua gói và payment gateway cũ của FE đã bị lệch backend. Hiện tại FE
          chỉ hiển thị dữ liệu thực sự có từ backend thay vì gọi các endpoint cũ.
        </div>
      </div>
    </div>
  );
};

export default Subscriptions;
