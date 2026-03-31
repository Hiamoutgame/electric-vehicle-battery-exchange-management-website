import axiosClient from "./axiosClient";
import { unwrapApiData, unwrapArray } from "./apiHelpers";

const normalizeSupportRequest = (request) => ({
  ...request,
  requestId: request?.requestId || request?.supportRequestId || "",
  issueType: request?.issueType || request?.category || "Khác",
  description: request?.description || "",
  createDate: request?.createDate || request?.createdAt || "",
  responseText: request?.responseText || "",
});

const staffService = {
  getAllSupportRequests: async () => {
    const response = await axiosClient.get("/staff/support-requests");
    return unwrapArray(response.data).map(normalizeSupportRequest);
  },

  updateSupportRequest: async (id, supportRequest) => {
    const response = await axiosClient.patch(`/staff/support-requests/${id}/response`, {
      status: supportRequest.status,
      responseMessage: supportRequest.responseText || supportRequest.responseMessage || "",
    });

    return normalizeSupportRequest(unwrapApiData(response.data));
  },
};

export default staffService;
