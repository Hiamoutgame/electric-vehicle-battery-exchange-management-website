import React, { useEffect, useMemo, useState } from "react";
import feedbackService from "@/api/feedbackService";
import { MessageSquare, Filter } from "lucide-react";
import FeedbackCard from "../components/feedback/FeedbackCard";

const Feedback = () => {
  const [allFeedbacks, setAllFeedbacks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedRating, setSelectedRating] = useState(0);
  const [currentPage, setCurrentPage] = useState(0);
  const ITEMS_PER_PAGE = 5;

  useEffect(() => {
    const load = async () => {
      try {
        setAllFeedbacks(await feedbackService.getAllFeedbacks());
      } finally {
        setLoading(false);
      }
    };

    load();
  }, []);

  const filteredFeedbacks = useMemo(() => {
    if (selectedRating === 0) return allFeedbacks;
    return allFeedbacks.filter((feedback) => feedback.rating === selectedRating);
  }, [allFeedbacks, selectedRating]);

  const totalPages = Math.ceil(filteredFeedbacks.length / ITEMS_PER_PAGE);
  const startIndex = currentPage * ITEMS_PER_PAGE;
  const paginatedFeedbacks = filteredFeedbacks.slice(
    startIndex,
    startIndex + ITEMS_PER_PAGE
  );

  useEffect(() => {
    setCurrentPage(0);
  }, [selectedRating]);

  return (
    <div className="w-full min-h-screen bg-gradient-to-br from-blue-50 to-gray-100 py-16 px-6 mt-20">
      <div className="max-w-7xl mx-auto">
        <div className="text-center mb-12">
          <div className="inline-flex items-center justify-center w-20 h-20 mb-6 rounded-full bg-gradient-to-r from-blue-600 to-indigo-600">
            <MessageSquare className="w-10 h-10 text-white" />
          </div>
          <h1 className="text-5xl font-bold text-gray-900 mb-4">
            Feedback Khách Hàng
          </h1>
        </div>

        <div className="mb-8 bg-white rounded-lg p-4 shadow-md flex items-center gap-4 flex-wrap">
          <Filter className="w-5 h-5 text-gray-600" />
          <span className="font-semibold text-gray-700">Lọc theo đánh giá:</span>
          <button
            onClick={() => setSelectedRating(0)}
            className={`px-4 py-2 rounded-lg font-medium ${
              selectedRating === 0
                ? "bg-blue-600 text-white"
                : "bg-gray-100 text-gray-700"
            }`}
          >
            Tất cả
          </button>
          {[5, 4, 3, 2, 1].map((rating) => (
            <button
              key={rating}
              onClick={() => setSelectedRating(rating)}
              className={`px-4 py-2 rounded-lg font-medium ${
                selectedRating === rating
                  ? "bg-yellow-500 text-white"
                  : "bg-gray-100 text-gray-700"
              }`}
            >
              {rating} sao
            </button>
          ))}
        </div>

        {loading ? (
          <div className="bg-white rounded-lg shadow-md p-12 text-center">
            Đang tải feedback...
          </div>
        ) : paginatedFeedbacks.length === 0 ? (
          <div className="bg-white rounded-lg shadow-md p-12 text-center">
            Chưa có feedback nào.
          </div>
        ) : (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
              {paginatedFeedbacks.map((feedback) => (
                <FeedbackCard
                  key={feedback.feedbackId}
                  feedback={{ ...feedback, fullName: "Khách hàng" }}
                />
              ))}
            </div>

            {totalPages > 1 && (
              <div className="bg-white rounded-lg shadow-md p-6 flex justify-center gap-2">
                {Array.from({ length: totalPages }, (_, index) => (
                  <button
                    key={index}
                    onClick={() => setCurrentPage(index)}
                    className={`px-4 py-2 rounded-lg ${
                      currentPage === index
                        ? "bg-blue-600 text-white"
                        : "border border-gray-300"
                    }`}
                  >
                    {index + 1}
                  </button>
                ))}
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
};

export default Feedback;
