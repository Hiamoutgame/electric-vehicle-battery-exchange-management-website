import React from "react";
import { Link } from "react-router-dom";

const Payment = () => {
  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-6">
      <div className="max-w-2xl bg-white rounded-2xl border border-gray-200 p-10 text-center">
        <h1 className="text-4xl font-bold text-gray-900">Payment</h1>
        <p className="text-gray-600 mt-4">
          Flow mua gói và thanh toán cũ của FE đang dùng các endpoint không còn
          tồn tại trong backend `/api/v1`.
        </p>
        <p className="text-gray-600 mt-3">
          Với backend hiện tại, FE chỉ có thể xem subscription hiện tại và lịch
          sử payment. Chưa có API purchase plan hoặc VNPay checkout trong route
          mới.
        </p>
        <Link
          to="/userPage/subscriptions"
          className="mt-8 inline-block rounded-lg bg-blue-600 px-6 py-3 text-white font-semibold hover:bg-blue-700"
        >
          Quay lại Subscription
        </Link>
      </div>
    </div>
  );
};

export default Payment;
