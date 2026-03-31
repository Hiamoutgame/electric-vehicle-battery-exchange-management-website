/* eslint-disable react-refresh/only-export-components */
import React, { createContext, useContext, useMemo, useState, useCallback } from "react";
import staffService from "@/api/staffService";

const StaffSupportContext = createContext();

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

export const StaffSupportProvider = ({ children }) => {
  const [requests, setRequests] = useState([]);
  const [activeTab, setActiveTab] = useState("pending");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRequest, setEditingRequest] = useState(null);
  const [loading, setLoading] = useState(false);

  const fetchAllRequests = useCallback(async () => {
    setLoading(true);
    try {
      const response = await staffService.getAllSupportRequests();
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

  const openEditModal = useCallback((request) => {
    setEditingRequest(request);
    setIsModalOpen(true);
  }, []);

  const closeModal = useCallback(() => {
    setEditingRequest(null);
    setIsModalOpen(false);
  }, []);

  const updateRequest = useCallback(async (requestId, updateData) => {
    const updated = await staffService.updateSupportRequest(requestId, updateData);
    setRequests((prev) =>
      prev.map((request) =>
        request.requestId === requestId ? mapRequest({ ...request, ...updated }) : request
      )
    );
    return updated;
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
    <StaffSupportContext.Provider
      value={{
        requests: filteredRequests,
        allRequests: requests,
        activeTab,
        setActiveTab,
        isModalOpen,
        setIsModalOpen,
        editingRequest,
        openEditModal,
        closeModal,
        updateRequest,
        fetchAllRequests,
        loading,
      }}
    >
      {children}
    </StaffSupportContext.Provider>
  );
};

export const useStaffSupport = () => {
  const context = useContext(StaffSupportContext);
  if (!context) {
    throw new Error("useStaffSupport must be used within StaffSupportProvider");
  }
  return context;
};
