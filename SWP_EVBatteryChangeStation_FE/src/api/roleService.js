const roleService = {
  getAllRoles: async () => [
    { roleId: "admin", roleName: "Admin" },
    { roleId: "staff", roleName: "Staff" },
    { roleId: "driver", roleName: "Customer" },
  ],

  getRoleByName: async (roleName) => {
    const roles = await roleService.getAllRoles();
    const normalized = (roleName || "").toLowerCase();
    return roles.find((role) => role.roleName.toLowerCase() === normalized);
  },

  getRoleIdByName: async (roleName) => {
    const role = await roleService.getRoleByName(roleName);
    return role?.roleId || null;
  },

  getRedirectPathByRole: async (roleName, roleId) => {
    const normalizedId = (roleId || "").toLowerCase();
    if (normalizedId.includes("admin")) return "/admin";
    if (normalizedId.includes("staff")) return "/staff";

    return roleService._mapRoleToRoute(roleName);
  },

  _mapRoleToRoute: (roleName) => {
    const normalized = (roleName || "").toLowerCase();

    if (normalized === "admin") return "/admin";
    if (normalized === "staff") return "/staff";
    return "/userPage";
  },

  clearCache: () => {},
};

export default roleService;
