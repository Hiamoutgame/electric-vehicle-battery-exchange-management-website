import React, { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import tokenUtils from "@/utils/tokenUtils";
import feedbackService from "@/api/feedbackService";
import bookingService from "@/api/bookingService";
import { MessageSquare, Star } from "lucide-react";
import FeedbackCard from "../components/feedback/FeedbackCard";
import FeedbackFormModal from "../components/feedback/FeedbackFormModal";

const UserFeedback = () => {
  const navigate = useNavigate();
  const [userData, setUserData] = useState(null);
  const [activeTab, setActiveTab] = useState("my");
  const [allFeedbacks, setAllFeedbacks] = useState([]);
  const [myFeedbacks, setMyFeedbacks] = useState([]);
  const [completedBookings, setCompletedBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showFeedbackForm, setShowFeedbackForm] = useState(false);

  useEffect(() => {
    if (!tokenUtils.isLoggedIn()) {
      navigate("/login");
      return;
    }

    setUserData(tokenUtils.getUserData());
  }, [navigate]);

  const reload = async (accountId) => {
    const [feedbacks, bookings] = await Promise.all([
      feedbackService.getAllFeedbacks(),
      bookingService.getUserBookings(),
    ]);

    setAllFeedbacks(feedbacks);
    setMyFeedbacks(feedbacks.filter((feedback) => feedback.accountId === accountId));
    setCompletedBookings(
      bookings.filter(
        (booking) =>
          booking.isApproved === "Completed" || booking.isApproved === "Swapped"
      )
    );
  };

  useEffect(() => {
    const load = async () => {
      if (!userData?.accountId) return;

      try {
        await reload(userData.accountId);
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [userData]);

  const currentFeedbacks = useMemo(
    () => (activeTab === "my" ? myFeedbacks : allFeedbacks),
    [activeTab, myFeedbacks, allFeedbacks]
  );

  if (!userData) {
    return <div className="p-16 text-center">Đang kiểm tra đăng nhập...</div>;
  }

  return (
    <div className="w-full min-h-screen bg-gradient-to-br from-blue-50 to-gray-100 py-16 px-6">
      <div className="max-w-7xl mx-auto">
        <div className="text-center mb-12">
          <div className="inline-flex items-center justify-center w-20 h-20 mb-6 rounded-full bg-gradient-to-r from-blue-600 to-indigo-600">
            <MessageSquare className="w-10 h-10 text-white" />
          </div>
          <h1 className="text-5xl font-bold text-gray-900 mb-4">Feedback</h1>
          <p className="text-xl text-gray-600">
            FE hiện dùng dữ liệu feedback tạo ra từ flow `/api/v1/driver/feedback`.
          </p>
        </div>

        <div className="mb-8 flex flex-col md:flex-row justify-between items-center gap-4">
          <div className="flex gap-2 bg-white rounded-lg p-1 shadow-md">
            <button
              onClick={() => setActiveTab("my")}
              className={`px-6 py-3 rounded-lg font-semibold text-lg ${
                activeTab === "my"
                  ? "bg-blue-600 text-white"
                  : "text-gray-600 hover:bg-gray-100"
              }`}
            >
              Feedback của tôi
            </button>
            <button
              onClick={() => setActiveTab("all")}
              className={`px-6 py-3 rounded-lg font-semibold text-lg ${
                activeTab === "all"
                  ? "bg-blue-600 text-white"
                  : "text-gray-600 hover:bg-gray-100"
              }`}
            >
              Tất cả feedback
            </button>
          </div>

          {completedBookings.length > 0 && (
            <button
              onClick={() => setShowFeedbackForm(true)}
              className="bg-gradient-to-r from-orange-500 to-orange-600 text-white px-6 py-3 rounded-lg font-semibold text-lg hover:from-orange-600 hover:to-orange-700 flex items-center gap-2"
            >
              <Star className="w-5 h-5" />
              Tạo feedback mới
            </button>
          )}
        </div>

        {loading ? (
          <div className="bg-white rounded-lg shadow-md p-12 text-center">
            Đang tải feedback...
          </div>
        ) : currentFeedbacks.length === 0 ? (
          <div className="bg-white rounded-lg shadow-md p-12 text-center">
            Chưa có feedback nào.
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
            {currentFeedbacks.map((feedback) => (
              <FeedbackCard
                key={feedback.feedbackId}
                feedback={{
                  ...feedback,
                  fullName:
                    feedback.accountId === userData.accountId
                      ? userData.fullName || "Bạn"
                      : "Khách hàng",
                }}
              />
            ))}
          </div>
        )}

        {showFeedbackForm && (
          <FeedbackFormModal
            bookings={completedBookings}
            accountId={userData.accountId}
            onClose={() => setShowFeedbackForm(false)}
            onSuccess={async () => {
              await reload(userData.accountId);
              setShowFeedbackForm(false);
            }}
          />
        )}
      </div>
    </div>
  );
};

export default UserFeedback;
