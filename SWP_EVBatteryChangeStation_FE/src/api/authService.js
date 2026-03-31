import axiosClient from "./axiosClient";
import {
  getApiMessage,
  unsupportedOperation,
  unwrapApiData,
  unwrapArray,
} from "./apiHelpers";

const authService = {
  login: async (email, password) => {
    const response = await axiosClient.post("/auth/login", {
      email,
      password,
    });

    return unwrapApiData(response.data);
  },

  register: async (emailOrPayload, password) => {
    const payload =
      typeof emailOrPayload === "object"
        ? emailOrPayload
        : { email: emailOrPayload, password };

    const response = await axiosClient.post("/auth/register", payload);
    return unwrapApiData(response.data);
  },

  verifyOtp: async (email, otp) => {
    const response = await axiosClient.post("/auth/verify-otp", {
      email,
      otpCode: otp,
    });

    return unwrapApiData(response.data);
  },

  sendForgotOtp: async () =>
    unsupportedOperation(
      "Backend hiện chưa hỗ trợ quên mật khẩu theo tài liệu API hiện tại."
    ),

  verifyForgotOtp: async () =>
    unsupportedOperation(
      "Backend hiện chưa hỗ trợ quên mật khẩu theo tài liệu API hiện tại."
    ),

  resetPassword: async () =>
    unsupportedOperation(
      "Backend hiện chưa hỗ trợ quên mật khẩu theo tài liệu API hiện tại."
    ),

  logout: async () => {
    try {
      await axiosClient.post("/auth/logout");
    } catch (error) {
      if (error?.response?.status !== 401) {
        throw error;
      }
    }
  },

  getMe: async () => {
    const response = await axiosClient.get("/auth/me");
    return unwrapApiData(response.data);
  },

  getUserByName: async () => authService.getMe(),

  getAll: async () => {
    try {
      const response = await axiosClient.get("/admin/users");
      return unwrapArray(response.data);
    } catch (error) {
      if (error?.response?.status === 401 || error?.response?.status === 403) {
        return [];
      }

      throw error;
    }
  },

  softDeleteAccounts: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API xóa mềm tài khoản trong bộ route /api/v1."
    ),

  updateProfile: async () =>
    unsupportedOperation(
      "Backend hiện chỉ hỗ trợ xem profile qua /auth/me, chưa có API cập nhật profile."
    ),

  createAccount: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API tạo tài khoản quản trị trong bộ route /api/v1."
    ),

  getErrorMessage: (error, fallback) => getApiMessage(error, fallback),
};

export default authService;
