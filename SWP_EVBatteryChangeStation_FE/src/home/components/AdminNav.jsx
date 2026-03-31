import React, { useState } from "react";
import "./AdminNav.css";
import { Link, useNavigate } from "react-router-dom";
import authService from "@/api/authService";
import tokenUtils from "@/utils/tokenUtils";

const AdminNav = () => {
  const [isSidebar] = useState(false);
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await authService.logout();
    } finally {
      tokenUtils.clearUserData();
      navigate("/login");
    }
  };

  return (
    <nav className={`admin-nav ${isSidebar ? "sidebar" : "topbar"}`}>
      <ul className="navbar-links">
        <h1 style={{ fontSize: "20px", fontWeight: "bold" }}>Xin chào Admin</h1>
        <li><Link to="/admin">Thống kê doanh thu</Link></li>
        <li><Link to="/admin/stations">Trạm</Link></li>
        <li><Link to="#" onClick={handleLogout}>Đăng xuất</Link></li>
      </ul>
    </nav>
  );
};

export default AdminNav;
