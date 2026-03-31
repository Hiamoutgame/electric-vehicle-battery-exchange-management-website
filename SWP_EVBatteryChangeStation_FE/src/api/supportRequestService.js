import axiosClient from "./axiosClient";
import { unsupportedOperation, unwrapApiData, unwrapArray } from "./apiHelpers";

const normalizeSupportRequest = (request) => ({
  ...request,
  requestId: request?.requestId || request?.supportRequestId || "",
  issueType: request?.issueType || request?.category || "Khác",
  description: request?.description || "",
  createDate: request?.createDate || request?.createdAt || "",
  responseText: request?.responseText || "",
});

const supportRequestService = {
  createSupportRequest: async (supportRequest) => {
    const response = await axiosClient.post("/driver/support-requests", {
      issueType: supportRequest.issueType,
      description: supportRequest.description,
    });

    return normalizeSupportRequest(unwrapApiData(response.data));
  },

  getByAccountId: async () => {
    const response = await axiosClient.get("/driver/support-requests");
    return unwrapArray(response.data).map(normalizeSupportRequest);
  },

  softDelete: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API xóa support request từ phía driver."
    ),

  hardDelete: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API xóa support request từ phía driver."
    ),
};

export default supportRequestService;
