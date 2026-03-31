import axiosClient from "./axiosClient";
import { unsupportedOperation, unwrapApiData } from "./apiHelpers";

const STORAGE_KEY = "ev_feedback_cache";

const readCache = () => {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? JSON.parse(raw) : [];
  } catch {
    return [];
  }
};

const writeCache = (items) => {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
};

const normalizeFeedback = (feedback) => ({
  ...feedback,
  feedbackId: feedback?.feedbackId || crypto.randomUUID(),
  bookingId: feedback?.bookingId || "",
  accountId: feedback?.accountId || "",
  rating: Number(feedback?.rating || 0),
  comment: feedback?.comment || "",
  createDate: feedback?.createDate || new Date().toISOString(),
});

const feedbackService = {
  getAllFeedbacks: async () => {
    return readCache().map(normalizeFeedback);
  },

  getFeedbackById: async (id) => {
    const feedbacks = readCache().map(normalizeFeedback);
    return feedbacks.find((feedback) => feedback.feedbackId === id) || null;
  },

  createFeedback: async ({ bookingId, accountId, rating, comment }) => {
    const response = await axiosClient.post("/driver/feedback", {
      bookingId,
      rating,
      comment,
    });

    const apiData = unwrapApiData(response.data) || {};
    const newFeedback = normalizeFeedback({
      ...apiData,
      bookingId,
      accountId,
      rating,
      comment,
    });

    const feedbacks = readCache();
    const nextFeedbacks = [
      newFeedback,
      ...feedbacks.filter((item) => item.bookingId !== bookingId),
    ];
    writeCache(nextFeedbacks);

    return newFeedback;
  },

  updateFeedback: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API cập nhật feedback trong bộ /api/v1."
    ),

  deleteFeedback: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API xóa feedback trong bộ /api/v1."
    ),

  getFeedbackByAccountId: async (accountId) => {
    return readCache()
      .map(normalizeFeedback)
      .filter((feedback) => feedback.accountId === accountId);
  },
};

export default feedbackService;
