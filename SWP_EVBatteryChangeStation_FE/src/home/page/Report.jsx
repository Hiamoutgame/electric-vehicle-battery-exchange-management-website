import React, { useEffect, useState } from "react";
import axios from "axios";
import "../components/AdminStyle.css";

const RevenueReport = () => {
  const [payments, setPayments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [monthlyData, setMonthlyData] = useState({});

  useEffect(() => {
    const fetchData = async () => {
      try {
        const paymentRes = await axios.get("http://localhost:5204/api/Payment/get-all");
        
        const data = paymentRes.data.data || [];

        // Group payments by month
        const groupedByMonth = groupPaymentsByMonth(data);

        setPayments(data);
        setMonthlyData(groupedByMonth);
        setLoading(false);
      } catch (error) {
        console.error(error);
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const formatMonth = (key) => {
    const [year, month] = key.split("-");
    return `Tháng: ${parseInt(month)}, ${year}`;
  };

  // Function to group payments by month
  const groupPaymentsByMonth = (payments) => {
    return payments.reduce((acc, payment) => {
      const date = new Date(payment.createDate);
      const month = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}`;
  
      if (!acc[month]) {
        acc[month] = [];
      }
      acc[month].push(payment);
  
      return acc;
    }, {});
  };

  if (loading) return <p>Loading...</p>;

  return (
    <div className="admin-dashboard">
      <h1 className="dashboard-title">Revenue Report</h1>

      <div className="summary-section">
      {Object.keys(monthlyData).map((month) => (
  <div key={month} className="summary-card">
    <h3>{formatMonth(month)}</h3>

    {/* Payment Count */}
    {/* <p>Số lần đổi pin: <strong>{monthlyData[month].length}</strong></p> */}

    <p>
  Tổng doanh thu:{" "}
  <strong>
    {monthlyData[month]
      .reduce((sum, payment) => sum + payment.price, 0)
      .toLocaleString()}
  </strong>
</p>
  </div>
))}

      </div>
    </div>
  );
};

export default RevenueReport;
