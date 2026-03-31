import axiosClient from "./axiosClient";
import { unsupportedOperation, unwrapApiData, unwrapArray } from "./apiHelpers";

const normalizePayment = (payment) => ({
  ...payment,
  paymentId: payment?.paymentId || "",
  price: payment?.price ?? payment?.amount ?? 0,
  method: payment?.method || payment?.paymentMethod || "",
  status: payment?.status ?? false,
  createDate: payment?.createDate || payment?.paidAt || "",
});

const paymentService = {
  createPayment: async () =>
    unsupportedOperation(
      "Backend hiện chưa có route thanh toán subscription cho frontend trong bộ /api/v1."
    ),

  getPaymentById: async (paymentId) => {
    const payments = await paymentService.getPaymentsByAccountId();
    return payments.find((payment) => payment.paymentId === paymentId) || null;
  },

  getAllPayments: async () => paymentService.getPaymentsByAccountId(),

  getPaymentsByAccountId: async () => {
    const response = await axiosClient.get("/driver/payments");
    return unwrapArray(response.data).map(normalizePayment);
  },

  updatePayment: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API cập nhật payment cho frontend trong bộ /api/v1."
    ),

  deletePayment: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API xóa payment cho frontend trong bộ /api/v1."
    ),

  getCurrentSubscription: async () => {
    try {
      const response = await axiosClient.get("/driver/subscriptions/current");
      return unwrapApiData(response.data);
    } catch (error) {
      if (error?.response?.status === 404) {
        return null;
      }

      throw error;
    }
  },

  checkSubscriptionStatus: async () => {
    const subscription = await paymentService.getCurrentSubscription();

    return {
      hasActiveSubscription: !!subscription,
      needsRedirect: !subscription,
      payment: subscription,
      subscription,
    };
  },
};

export default paymentService;
