import React, { useEffect, useState } from "react";
import batteryService from "../../api/batteryService";
import { notifySuccess, notifyError } from "../../components/notification/notification";

const StaffBatteryManagement = () => {
  const [batteries, setBatteries] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchBatteries = async () => {
    setLoading(true);
    try {
      setBatteries(await batteryService.getMyStationBatteries());
    } catch (error) {
      setBatteries([]);
      notifyError(error?.response?.data?.message || "Không thể tải danh sách pin.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchBatteries();
  }, []);

  const toggleBatteryStatus = async (battery) => {
    try {
      await batteryService.updateBattery(battery.batteryId, {
        status: !battery.status,
        reason: "Updated by station staff",
      });
      notifySuccess("Cập nhật trạng thái pin thành công.");
      fetchBatteries();
    } catch (error) {
      notifyError(error?.response?.data?.message || "Cập nhật pin thất bại.");
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 p-8">
      <div className="max-w-7xl mx-auto">
        <h1 className="text-4xl font-bold text-gray-900 mb-3">
          Pin của trạm được gán
        </h1>
        <p className="text-gray-600 mb-8">
          Theo tài liệu backend hiện tại, staff chỉ có thể xem inventory và đổi
          trạng thái pin.
        </p>

        {loading ? (
          <div className="bg-white rounded-2xl border border-gray-200 p-16 text-center">
            Đang tải danh sách pin...
          </div>
        ) : batteries.length === 0 ? (
          <div className="bg-white rounded-2xl border border-gray-200 p-16 text-center">
            Không có pin nào trong inventory của trạm hiện tại.
          </div>
        ) : (
          <div className="grid gap-4">
            {batteries.map((battery) => (
              <div
                key={battery.batteryId}
                className="bg-white rounded-2xl border border-gray-200 p-6 flex flex-col md:flex-row md:items-center md:justify-between gap-4"
              >
                <div>
                  <h2 className="text-2xl font-semibold text-gray-900">
                    {battery.typeBattery || "Battery"}
                  </h2>
                  <p className="text-gray-600 mt-1">Battery ID: {battery.batteryId}</p>
                  <p className="text-gray-600 mt-1">Station ID: {battery.stationId}</p>
                </div>

                <div className="flex items-center gap-4">
                  <span
                    className={`px-4 py-2 rounded-full font-semibold ${
                      battery.status
                        ? "bg-green-100 text-green-700"
                        : "bg-red-100 text-red-700"
                    }`}
                  >
                    {battery.status ? "Available" : "Unavailable"}
                  </span>
                  <button
                    className="px-4 py-2 rounded-lg border border-gray-300 hover:bg-gray-50"
                    onClick={() => toggleBatteryStatus(battery)}
                  >
                    Đổi trạng thái
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default StaffBatteryManagement;
