import { jwtDecode } from "jwt-decode";

export const tokenUtils = {
  decodeToken: (token) => {
    try {
      return jwtDecode(token);
    } catch {
      return null;
    }
  },

  getToken: () => localStorage.getItem("token"),

  getUserData: () => {
    try {
      const userData = localStorage.getItem("user");
      return userData ? JSON.parse(userData) : null;
    } catch {
      return null;
    }
  },

  saveUserData: (userData) => {
    try {
      localStorage.setItem("user", JSON.stringify(userData));
      return true;
    } catch {
      return false;
    }
  },

  applyPendingProfileLocally: () => false,

  clearUserData: () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    localStorage.removeItem("pendingProfile");
  },

  mapApiResponseToUserProfile: (apiData, tokenData = null) => ({
    accountId: apiData?.accountId || tokenData?.sub || "",
    accountName: apiData?.username || apiData?.accountName || tokenData?.name || "",
    username: apiData?.username || apiData?.accountName || tokenData?.name || "",
    email: apiData?.email || tokenData?.email || "",
    fullName:
      apiData?.fullName ||
      apiData?.username ||
      apiData?.accountName ||
      tokenData?.name ||
      "",
    name:
      apiData?.fullName ||
      apiData?.username ||
      apiData?.accountName ||
      tokenData?.name ||
      "",
    gender: apiData?.gender || "",
    address: apiData?.address || "",
    phoneNumber: apiData?.phoneNumber || "",
    dateOfBirth: apiData?.dateOfBirth || null,
    status: apiData?.status ?? true,
    roleId: apiData?.roleId || "",
    role: apiData?.roleName || tokenData?.role || "",
    roleName: apiData?.roleName || tokenData?.role || "",
    stationId: apiData?.stationId || null,
    stationName: apiData?.stationName || "",
  }),

  processLoginToken: async (token) => {
    const decoded = tokenUtils.decodeToken(token);
    if (!decoded) return null;
    return tokenUtils.fetchUserDataFromAPI(decoded);
  },

  getAccountNameFromToken: () => {
    const token = tokenUtils.getToken();
    if (!token) return null;

    const decoded = tokenUtils.decodeToken(token);
    return decoded?.name || null;
  },

  fetchUserDataFromAPI: async (decodedToken = null) => {
    try {
      const { default: authService } = await import("@/api/authService");
      const decoded = decodedToken || tokenUtils.decodeToken(tokenUtils.getToken());
      const me = await authService.getMe();
      const userProfile = tokenUtils.mapApiResponseToUserProfile(me, decoded);
      tokenUtils.saveUserData(userProfile);
      return userProfile;
    } catch {
      return null;
    }
  },

  isTokenValid: () => {
    try {
      const token = tokenUtils.getToken();
      if (!token) return false;

      const decoded = tokenUtils.decodeToken(token);
      if (!decoded) return false;

      if (decoded.exp) {
        return decoded.exp >= Math.floor(Date.now() / 1000);
      }

      return true;
    } catch {
      return false;
    }
  },

  isLoggedIn: () => {
    return !!(
      localStorage.getItem("token") &&
      localStorage.getItem("user") &&
      tokenUtils.isTokenValid()
    );
  },

  autoLogin: async () => {
    const token = tokenUtils.getToken();
    if (!token) return false;

    if (!tokenUtils.isTokenValid()) {
      tokenUtils.clearUserData();
      return false;
    }

    const refreshed = await tokenUtils.fetchUserDataFromAPI();
    return refreshed || tokenUtils.getUserData() || false;
  },

  getUserProfile: async (forceRefresh = false) => {
    if (forceRefresh) {
      return await tokenUtils.fetchUserDataFromAPI();
    }

    const cached = tokenUtils.getUserData();
    if (cached?.email) {
      return cached;
    }

    return await tokenUtils.fetchUserDataFromAPI();
  },
};

export default tokenUtils;
