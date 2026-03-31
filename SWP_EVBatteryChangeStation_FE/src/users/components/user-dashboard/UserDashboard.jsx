import React, { useEffect, useState } from "react";
import { Battery, MapPin, TrendingUp, Star } from "lucide-react";
import bookingService from "@/api/bookingService";
import feedbackService from "@/api/feedbackService";
import tokenUtils from "@/utils/tokenUtils";
import Feedback from "./../feedback/Feedback";

const UserDashboard = () => {
  const [user, setUser] = useState(null);
  const [bookings, setBookings] = useState([]);
  const [feedbacks, setFeedbacks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showFeedback, setShowFeedback] = useState(false);
  const [selectedBooking, setSelectedBooking] = useState(null);

  useEffect(() => {
    const load = async () => {
      const userData = tokenUtils.getUserData();
      setUser(userData);

      try {
        const [userBookings, userFeedbacks] = await Promise.all([
          bookingService.getUserBookings(),
          userData?.accountId
            ? feedbackService.getFeedbackByAccountId(userData.accountId)
            : [],
        ]);
        setBookings(userBookings);
        setFeedbacks(userFeedbacks);
      } catch {
        setBookings([]);
        setFeedbacks([]);
      } finally {
        setLoading(false);
      }
    };

    load();
  }, []);

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="animate-spin h-16 w-16 border-4 border-orange-400 border-t-transparent rounded-full" />
      </div>
    );
  }

  const totalBookings = bookings.length;
  const totalStationsVisited = new Set(bookings.map((booking) => booking.stationName)).size;
  const pendingFeedbackBookings = bookings.filter(
    (booking) =>
      (booking.isApproved === "Completed" || booking.isApproved === "Swapped") &&
      !feedbacks.some((feedback) => feedback.bookingId === booking.bookingId)
  );

  return (
    <div className="p-8 bg-gray-50 min-h-screen">
      <h1 className="text-4xl font-extrabold text-orange-700 mb-8">
        Xin chào, {user?.fullName || "User"}
      </h1>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-12">
        <DashboardCard
          title="Tổng số booking"
          value={totalBookings}
          icon={<Battery className="text-blue-600" />}
        />
        <DashboardCard
          title="Số trạm từng đến"
          value={totalStationsVisited}
          icon={<MapPin className="text-purple-600" />}
        />
        <DashboardCard
          title="Feedback đã gửi"
          value={feedbacks.length}
          icon={<TrendingUp className="text-red-500" />}
        />
      </div>

      <div className="bg-white shadow-xl rounded-xl p-8 mb-12">
        <h2 className="text-2xl font-bold text-gray-800 mb-6">
          Booking gần đây
        </h2>
        {bookings.length === 0 ? (
          <p className="text-gray-500">Chưa có booking nào.</p>
        ) : (
          <div className="space-y-4">
            {bookings.slice(0, 5).map((booking) => (
              <div
                key={booking.bookingId}
                className="border border-gray-200 rounded-xl p-4 flex flex-col md:flex-row md:items-center md:justify-between gap-4"
              >
                <div>
                  <p className="font-semibold text-gray-900">
                    {booking.stationName || "Station"}
                  </p>
                  <p className="text-gray-600">{booking.dateTime || "-"}</p>
                  <p className="text-gray-600">Status: {booking.isApproved}</p>
                </div>

                {(booking.isApproved === "Completed" || booking.isApproved === "Swapped") &&
                  !feedbacks.some((feedback) => feedback.bookingId === booking.bookingId) && (
                    <button
                      onClick={() => {
                        setSelectedBooking(booking);
                        setShowFeedback(true);
                      }}
                      className="flex items-center gap-2 bg-orange-500 text-white px-4 py-2 rounded-lg hover:bg-orange-600"
                    >
                      <Star className="w-4 h-4" />
                      Gửi feedback
                    </button>
                  )}
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="bg-white shadow-xl rounded-xl p-8">
        <h2 className="text-2xl font-bold text-gray-800 mb-6">
          Booking đã hoàn tất nhưng chưa feedback
        </h2>
        {pendingFeedbackBookings.length === 0 ? (
          <p className="text-gray-500">Không còn booking nào cần feedback.</p>
        ) : (
          <div className="space-y-3">
            {pendingFeedbackBookings.map((booking) => (
              <div
                key={booking.bookingId}
                className="border border-gray-200 rounded-xl p-4 flex items-center justify-between"
              >
                <div>
                  <p className="font-semibold text-gray-900">
                    {booking.stationName || "Station"}
                  </p>
                  <p className="text-gray-600">{booking.dateTime || "-"}</p>
                </div>
                <button
                  onClick={() => {
                    setSelectedBooking(booking);
                    setShowFeedback(true);
                  }}
                  className="flex items-center gap-2 bg-orange-500 text-white px-4 py-2 rounded-lg hover:bg-orange-600"
                >
                  <Star className="w-4 h-4" />
                  Gửi feedback
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      {showFeedback && selectedBooking && (
        <Feedback
          booking={selectedBooking}
          accountId={user?.accountId}
          onClose={() => {
            setShowFeedback(false);
            setSelectedBooking(null);
          }}
          onSuccess={async () => {
            if (user?.accountId) {
              setFeedbacks(await feedbackService.getFeedbackByAccountId(user.accountId));
            }
            setShowFeedback(false);
            setSelectedBooking(null);
          }}
        />
      )}
    </div>
  );
};

const DashboardCard = ({ title, value, icon }) => (
  <div className="bg-white p-6 rounded-xl shadow-lg border border-gray-100">
    <div className="flex items-center justify-between">
      <div>
        <p className="text-gray-500 text-sm mb-1">{title}</p>
        <h2 className="text-4xl font-extrabold text-orange-600">{value}</h2>
      </div>
      <div className="p-4 rounded-full bg-gray-50">{icon}</div>
    </div>
  </div>
);

export default UserDashboard;
