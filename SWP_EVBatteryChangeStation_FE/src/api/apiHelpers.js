const API_VERSION_SEGMENT = "/api/v1";

export const normalizeApiBaseUrl = (rawBaseUrl) => {
  const trimmed = (rawBaseUrl || "").trim().replace(/\/+$/, "");

  if (!trimmed) {
    return API_VERSION_SEGMENT;
  }

  if (trimmed.endsWith(API_VERSION_SEGMENT)) {
    return trimmed;
  }

  if (trimmed.endsWith("/api")) {
    return `${trimmed}/v1`;
  }

  return `${trimmed}${API_VERSION_SEGMENT}`;
};

export const unwrapApiData = (payload) => {
  if (payload && typeof payload === "object" && "data" in payload) {
    return payload.data;
  }

  return payload;
};

export const unwrapArray = (payload) => {
  const data = unwrapApiData(payload);
  return Array.isArray(data) ? data : [];
};

export const getApiMessage = (error, fallback = "Request failed.") =>
  error?.response?.data?.message ||
  error?.response?.data?.title ||
  error?.message ||
  fallback;

export const unsupportedOperation = (message) => {
  throw new Error(message);
};
