import React, { useState } from "react";
import "./AdminNav.css";
import { Link, useNavigate } from "react-router-dom";
import authService from "@/api/authService";
import tokenUtils from "@/utils/tokenUtils";

const StaffNav = () => {
  const [isSidebar] = useState(false);
  const navigate = useNavigate();

  const handleLogout = async (event) => {
    event.preventDefault();
    try {
      await authService.logout();
    } finally {
      tokenUtils.clearUserData();
      navigate("/login");
    }
  };

  return (
    <nav className={`admin-nav ${isSidebar ? "sidebar" : "topbar"} `}>
      <ul className="navbar-links">
        <h1 style={{ fontSize: "20px", fontWeight: "bold" }}>Xin chào staff</h1>
        <li><Link to="/staff">Booking của trạm</Link></li>
        <li><Link to="/staff/battery">Inventory pin</Link></li>
        <li><Link to="/staff/support">Hỗ trợ</Link></li>
        <li>
          <Link to="#" onClick={handleLogout}>
            Đăng xuất
          </Link>
        </li>
      </ul>
    </nav>
  );
};

export default StaffNav;
