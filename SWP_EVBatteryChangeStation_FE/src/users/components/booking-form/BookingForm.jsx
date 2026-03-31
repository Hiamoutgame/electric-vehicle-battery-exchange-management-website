import React, { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import bookingService from "@/api/bookingService";
import carService from "@/api/carService";
import batteryService from "@/api/batteryService";
import paymentService from "@/api/paymentService";
import stationService from "@/api/stationService";
import {
  notifyWarning,
  notifySuccess,
  notifyError,
} from "@/components/notification/notification";

const BookingForm = ({ onSuccess, onCancel }) => {
  const navigate = useNavigate();
  const [bookingForm, setBookingForm] = useState({
    vehicleId: "",
    stationId: "",
    dateTime: "",
    notes: "Battery transfer",
  });
  const [cars, setCars] = useState([]);
  const [stations, setStations] = useState([]);
  const [submitting, setSubmitting] = useState(false);
  const [batteryPreview, setBatteryPreview] = useState(null);
  const [loadingPreview, setLoadingPreview] = useState(false);

  const updateField = (field) => {
    setBookingForm((prev) => ({ ...prev, ...field }));
  };

  useEffect(() => {
    const load = async () => {
      try {
        const [vehicleList, stationList] = await Promise.all([
          carService.getAllCars(),
          stationService.getStationList(),
        ]);

        const mappedCars = vehicleList.map((car) => ({
          id: car.vehicleId,
          label: car.licensePlate
            ? `${car.model} - ${car.licensePlate}`
            : car.model,
        }));

        const mappedStations = stationList.map((station) => ({
          id: station.stationId,
          label: station.stationName || station.address,
        }));

        setCars(mappedCars);
        setStations(mappedStations);
        setBookingForm((prev) => ({
          ...prev,
          vehicleId: prev.vehicleId || mappedCars[0]?.id || "",
          stationId: prev.stationId || mappedStations[0]?.id || "",
        }));
      } catch {
        notifyError("Không thể tải dữ liệu xe hoặc trạm.");
      }
    };

    load();
  }, []);

  useEffect(() => {
    const previewBattery = async () => {
      if (!bookingForm.vehicleId || !bookingForm.stationId) {
        setBatteryPreview(null);
        return;
      }

      setLoadingPreview(true);
      try {
        const preview = await batteryService.previewForBooking(
          bookingForm.stationId,
          bookingForm.vehicleId
        );
        setBatteryPreview(preview);
      } catch {
        setBatteryPreview(null);
      } finally {
        setLoadingPreview(false);
      }
    };

    const timer = setTimeout(previewBattery, 250);
    return () => clearTimeout(timer);
  }, [bookingForm.stationId, bookingForm.vehicleId]);

  const canSubmit = useMemo(
    () =>
      bookingForm.vehicleId &&
      bookingForm.stationId &&
      bookingForm.dateTime,
    [bookingForm]
  );

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (!canSubmit) {
      notifyWarning("Vui lòng điền đầy đủ thông tin bắt buộc.");
      return;
    }

    if (!batteryPreview?.isAvailable) {
      notifyWarning("Trạm này hiện không còn pin khả dụng để đặt lịch.");
      return;
    }

    try {
      setSubmitting(true);

      const subscriptionCheck = await paymentService.checkSubscriptionStatus();
      if (!subscriptionCheck.hasActiveSubscription) {
        notifyWarning("Bạn chưa có gói subscription đang hoạt động.");
        navigate("/userPage/subscriptions");
        return;
      }

      await bookingService.createBooking({
        dateTime: bookingForm.dateTime,
        notes: bookingForm.notes,
        stationId: bookingForm.stationId,
        vehicleId: bookingForm.vehicleId,
      });

      notifySuccess("Đặt lịch thành công. Vui lòng chờ staff xác nhận.");
      onSuccess?.();
    } catch (error) {
      notifyError(
        error?.response?.data?.message ||
          "Không thể tạo booking. Vui lòng thử lại."
      );
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-3">
      <div>
        <label className="block text-sm font-medium mb-1 text-[#001f54]">
          Chọn xe
        </label>
        <select
          className="border p-2 rounded-lg w-full"
          value={bookingForm.vehicleId}
          onChange={(event) => updateField({ vehicleId: event.target.value })}
        >
          {cars.map((car) => (
            <option key={car.id} value={car.id}>
              {car.label}
            </option>
          ))}
        </select>
      </div>

      <div>
        <label className="block text-sm font-medium mb-1 text-[#001f54]">
          Chọn trạm
        </label>
        <select
          className="border p-2 rounded-lg w-full"
          value={bookingForm.stationId}
          onChange={(event) => updateField({ stationId: event.target.value })}
        >
          {stations.map((station) => (
            <option key={station.id} value={station.id}>
              {station.label}
            </option>
          ))}
        </select>
      </div>

      <div>
        <label className="block text-sm font-medium mb-1 text-[#001f54]">
          Ngày giờ
        </label>
        <input
          className="border p-2 rounded-lg w-full"
          type="datetime-local"
          value={bookingForm.dateTime}
          onChange={(event) => updateField({ dateTime: event.target.value })}
        />
      </div>

      <div>
        <label className="block text-sm font-medium mb-1 text-[#001f54]">
          Ghi chú
        </label>
        <textarea
          className="border p-2 rounded-lg w-full"
          rows={3}
          value={bookingForm.notes}
          onChange={(event) => updateField({ notes: event.target.value })}
          placeholder="Ghi chú thêm..."
        />
      </div>

      {bookingForm.vehicleId && bookingForm.stationId && (
        <div className="border rounded-lg p-4 bg-gray-50">
          <h3 className="text-sm font-semibold mb-2 text-[#001f54]">
            Trạng thái pin khả dụng
          </h3>
          {loadingPreview ? (
            <p className="text-sm text-gray-500">Đang kiểm tra...</p>
          ) : batteryPreview?.isAvailable ? (
            <div className="space-y-1 text-sm">
              <p>
                <span className="font-medium">Loại pin:</span>{" "}
                {batteryPreview.batteryType || "N/A"}
              </p>
              <p className="text-green-600">Có pin khả dụng tại trạm</p>
            </div>
          ) : (
            <p className="text-sm text-red-600">
              Trạm này hiện không còn pin khả dụng.
            </p>
          )}
        </div>
      )}

      <div className="flex justify-end gap-3 pt-2">
        {onCancel && (
          <button
            type="button"
            className="px-4 py-2 bg-gray-200 rounded-xl hover:bg-gray-300"
            onClick={onCancel}
          >
            Hủy
          </button>
        )}
        <button
          type="submit"
          className="px-4 py-2 bg-[#00b894] text-white rounded-xl hover:bg-[#009874] disabled:opacity-60"
          disabled={!canSubmit || submitting}
        >
          {submitting ? "Đang gửi..." : "Xác nhận"}
        </button>
      </div>
    </form>
  );
};

export default BookingForm;
