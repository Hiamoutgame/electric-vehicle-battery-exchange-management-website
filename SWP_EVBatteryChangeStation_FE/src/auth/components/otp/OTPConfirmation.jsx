import React, { useState } from "react";
import OTPInput from "react-otp-input";
import { toast } from "react-toastify";
import { useLocation, useNavigate } from "react-router-dom";
import authService from "@/api/authService";

const OTPConfirmation = () => {
  const [otp, setOtp] = useState("");
  const [loading, setLoading] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();

  const email =
    location.state?.email || localStorage.getItem("pendingVerificationEmail");
  const flow = location.state?.flow || "verify";

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (!email) {
      toast.error("Khong tim thay email dang cho xac thuc. Hay dang ky lai.");
      navigate("/signup");
      return;
    }

    if (otp.length !== 6) {
      toast.warning("Vui long nhap du 6 so OTP.");
      return;
    }

    if (flow === "reset") {
      toast.error("Backend hien chua ho tro flow quen mat khau.");
      return;
    }

    setLoading(true);
    try {
      await authService.verifyOtp(email, otp);
      localStorage.removeItem("pendingVerificationEmail");
      toast.success("Xac thuc OTP thanh cong.");
      setTimeout(() => navigate("/login"), 1200);
    } catch (error) {
      toast.error(
        authService.getErrorMessage(
          error,
          "Ma OTP khong hop le hoac da het han."
        )
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex h-full w-full flex-col items-center justify-center rounded-2xl bg-[linear-gradient(90deg,_rgba(42,123,155,1)_0%,_rgba(119,87,199,0.98)_15%,_rgba(84,216,223,1)_100%)] p-4 text-white">
      <div className="mb-12 text-center font-bold">
        <h1 className="text-5xl">Xac nhan OTP</h1>
        <p className="mt-2 text-2xl opacity-90">
          Nhap ma OTP tu log backend cho{" "}
          <span className="font-semibold">{email || "email cua ban"}</span>
        </p>
      </div>

      <form onSubmit={handleSubmit} className="flex flex-col items-center gap-8">
        <OTPInput
          value={otp}
          onChange={setOtp}
          numInputs={6}
          shouldAutoFocus
          renderSeparator={<span className="mx-2 text-2xl text-white">-</span>}
          renderInput={(props) => (
            <input
              {...props}
              className="!h-14 !w-14 rounded-lg border border-gray-700 bg-white/10 text-center text-2xl font-semibold text-white focus:outline-none focus:ring-2 focus:ring-[#54d8df]"
              inputMode="numeric"
              maxLength="1"
            />
          )}
        />
        <button
          type="submit"
          disabled={loading}
          className="mt-6 rounded-lg bg-white px-8 py-3 text-lg font-bold text-[#2a7b9b] shadow-lg transition-all hover:opacity-90 disabled:opacity-70"
        >
          {loading ? "Dang xac thuc..." : "Xac nhan"}
        </button>
      </form>
    </div>
  );
};

export default OTPConfirmation;
