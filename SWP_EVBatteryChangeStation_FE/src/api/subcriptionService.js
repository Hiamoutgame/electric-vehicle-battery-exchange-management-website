import axiosClient from "./axiosClient";
import { unsupportedOperation, unwrapApiData } from "./apiHelpers";

const subcriptionService = {
  getSubscriptions: async () => {
    try {
      const response = await axiosClient.get("/driver/subscriptions/current");
      const currentSubscription = unwrapApiData(response.data);
      return currentSubscription ? [currentSubscription] : [];
    } catch (error) {
      if (
        error?.response?.status === 401 ||
        error?.response?.status === 403 ||
        error?.response?.status === 404
      ) {
        return [];
      }

      throw error;
    }
  },

  getActiveSubscriptions: async () => subcriptionService.getSubscriptions(),

  createSubscription: async (subscription) => {
    const response = await axiosClient.post("/admin/subscription-plans", subscription);
    return unwrapApiData(response.data);
  },

  updateSubscription: async (subscriptionId, subscription) => {
    const response = await axiosClient.patch(
      `/admin/subscription-plans/${subscriptionId}`,
      subscription
    );
    return unwrapApiData(response.data);
  },

  deleteSubscription: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API xóa subscription plan trong bộ route /api/v1."
    ),

  getMySubscription: async () => {
    try {
      const response = await axiosClient.get("/driver/subscriptions/current");
      return unwrapApiData(response.data);
    } catch (error) {
      if (
        error?.response?.status === 401 ||
        error?.response?.status === 403 ||
        error?.response?.status === 404
      ) {
        return null;
      }

      throw error;
    }
  },
};

export default subcriptionService;
