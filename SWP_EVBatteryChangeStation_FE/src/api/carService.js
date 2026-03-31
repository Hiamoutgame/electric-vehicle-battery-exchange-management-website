import axiosClient from "./axiosClient";
import { unsupportedOperation, unwrapApiData, unwrapArray } from "./apiHelpers";

const normalizeCar = (car) => ({
  ...car,
  vehicleId: car?.vehicleId || "",
  model: car?.model || car?.vehicleModel || "",
  vehicleModel: car?.vehicleModel || car?.model || "",
  batteryType: car?.batteryType || car?.batteryTypeId || "",
  vin: car?.vin || "",
  licensePlate: car?.licensePlate || "",
  producer: car?.producer || "",
  images: car?.images || null,
  status: car?.status ?? true,
  createDate: car?.createDate || null,
});

const carService = {
  createCar: async ({
    vin,
    licensePlate,
    model,
    batteryType,
    producer,
    createDate,
    images,
    status,
  }) => {
    const response = await axiosClient.post("/driver/vehicles", {
      vin: vin || "",
      licensePlate: licensePlate || "",
      model,
      batteryType,
      producer,
      createDate: createDate || new Date().toISOString(),
      images: images || null,
      status: status || "ACTIVE",
    });

    return normalizeCar(unwrapApiData(response.data));
  },

  getAllCars: async () => {
    const response = await axiosClient.get("/driver/vehicles");
    return unwrapArray(response.data).map(normalizeCar);
  },

  updateCar: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API cập nhật xe trong bộ route /api/v1."
    ),

  getCarById: async (vehicleId) => {
    const vehicles = await carService.getAllCars();
    return vehicles.find((vehicle) => vehicle.vehicleId === vehicleId) || null;
  },

  deleteCar: async () =>
    unsupportedOperation(
      "Backend hiện chưa có API xóa xe trong bộ route /api/v1."
    ),
};

export default carService;
