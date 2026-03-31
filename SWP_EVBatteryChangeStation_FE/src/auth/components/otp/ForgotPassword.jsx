import React from "react";
import { Link } from "react-router-dom";

const ForgotPassword = () => {
  return (
    <div className="flex h-full w-full flex-col items-center justify-center rounded-2xl p-8 text-white bg-[linear-gradient(90deg,_rgba(42,123,155,1)_0%,_rgba(119,87,199,0.98)_15%,_rgba(84,216,223,1)_100%)]">
      <div className="max-w-2xl text-center">
        <h1 className="text-5xl font-bold">Quên mật khẩu</h1>
        <p className="mt-6 text-2xl opacity-90">
          Backend hiện chưa có API quên mật khẩu trong bộ route `/api/v1/auth`.
        </p>
        <p className="mt-4 text-xl opacity-80">
          Bạn cần hoàn thiện flow backend trước khi mở màn này.
        </p>
        <Link
          to="/login"
          className="mt-8 inline-block rounded-lg bg-white px-8 py-3 text-lg font-bold text-[#2a7b9b] shadow-lg"
        >
          Quay lại đăng nhập
        </Link>
      </div>
    </div>
  );
};

export default ForgotPassword;
