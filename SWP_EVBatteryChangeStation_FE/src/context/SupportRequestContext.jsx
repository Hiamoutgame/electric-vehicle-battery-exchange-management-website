/* eslint-disable react-refresh/only-export-components */
import React, { createContext, useContext, useMemo, useState, useCallback } from "react";
import supportRequestService from "@/api/supportRequestService";

const SupportRequestContext = createContext();

const mapRequest = (item) => ({
  id: item.requestId,
  requestId: item.requestId,
  title: item.issueType || "Yêu cầu hỗ trợ",
  supportType: item.issueType || "Khác",
  issueType: item.issueType || "Khác",
  description: item.description || "",
  details: item.description || "",
  status:
    item.responseText && item.responseText.trim() !== "" ? "resolved" : "pending",
  createdAt: item.createDate,
  createDate: item.createDate,
  responseText: item.responseText || "",
  responseDate: item.responseDate || null,
  accountId: item.accountId || null,
  staffId: item.staffId || null,
});

export const SupportRequestProvider = ({ children }) => {
  const [requests, setRequests] = useState([]);
  const [activeTab, setActiveTab] = useState("pending");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [loading, setLoading] = useState(false);

  const fetchRequests = useCallback(async () => {
    setLoading(true);
    try {
      const response = await supportRequestService.getByAccountId();
      const mapped = response.map(mapRequest).sort(
        (left, right) =>
          new Date(right.createDate || 0) - new Date(left.createDate || 0)
      );
      setRequests(mapped);
    } catch {
      setRequests([]);
    } finally {
      setLoading(false);
    }
  }, []);

  const addRequest = useCallback((newRequest) => {
    setRequests((prev) => [mapRequest(newRequest), ...prev]);
  }, []);

  const updateRequestStatus = useCallback((id, status) => {
    setRequests((prev) =>
      prev.map((request) => (request.id === id ? { ...request, status } : request))
    );
  }, []);

  const deleteRequest = useCallback(async () => {
    throw new Error("Backend hiện chưa hỗ trợ xóa support request.");
  }, []);

  const filteredRequests = useMemo(
    () =>
      requests.filter((request) =>
        activeTab === "pending"
          ? request.status === "pending"
          : request.status === "resolved"
      ),
    [activeTab, requests]
  );

  return (
    <SupportRequestContext.Provider
      value={{
        requests: filteredRequests,
        allRequests: requests,
        activeTab,
        setActiveTab,
        isModalOpen,
        setIsModalOpen,
        addRequest,
        updateRequestStatus,
        deleteRequest,
        fetchRequests,
        loading,
      }}
    >
      {children}
    </SupportRequestContext.Provider>
  );
};

export const useSupportRequest = () => {
  const context = useContext(SupportRequestContext);
  if (!context) {
    throw new Error("useSupportRequest must be used within SupportRequestProvider");
  }
  return context;
};
