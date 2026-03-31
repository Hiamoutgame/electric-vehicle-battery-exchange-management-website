import React, { useEffect, useRef, useState } from "react";
import VietMapPlaces from "@/components/MapAPI/VietMapPlaces";
import StationSearch from "@/components/MapAPI/StationSearch";
import stationService from "@/api/stationService";
import { vietmapService } from "@/api/vietmapService";
import { MapPin, Navigation, Battery } from "lucide-react";
import { notifyWarning } from "@/components/notification/notification";

const Stations = () => {
  const API_KEY = import.meta.env.VITE_APP_VIETMAP_API_KEY;
  const [userLocation, setUserLocation] = useState(null);
  const [stations, setStations] = useState([]);
  const [stationsWithCoords, setStationsWithCoords] = useState([]);
  const [route, setRoute] = useState(null);
  const [routeInfo, setRouteInfo] = useState(null);
  const [selectedStation, setSelectedStation] = useState(null);
  const destRef = useRef(null);

  useEffect(() => {
    if (!("geolocation" in navigator)) {
      setUserLocation([106.7, 10.77]);
      return;
    }

    const watchId = navigator.geolocation.watchPosition(
      (position) => {
        const location = [position.coords.longitude, position.coords.latitude];
        setUserLocation(location);

        if (destRef.current) {
          updateRoute(location, destRef.current);
        }
      },
      () => setUserLocation([106.7, 10.77]),
      { enableHighAccuracy: true, maximumAge: 10000, timeout: 20000 }
    );

    return () => navigator.geolocation.clearWatch(watchId);
  }, []);

  useEffect(() => {
    const loadStations = async () => {
      try {
        const stationList = await stationService.getStationList();
        setStations(stationList);

        const withCoords = await Promise.all(
          stationList.map(async (station) => {
            const coords = await vietmapService.geocodeAddress(API_KEY, station.address);
            if (!coords) return null;

            return {
              ...station,
              lat: coords.lat,
              lng: coords.lng,
              distance: userLocation
                ? calculateDistance(userLocation, [coords.lng, coords.lat])
                : null,
            };
          })
        );

        setStationsWithCoords(withCoords.filter(Boolean));
      } catch {
        setStations([]);
        setStationsWithCoords([]);
      }
    };

    if (API_KEY) {
      loadStations();
    }
  }, [API_KEY, userLocation]);

  const calculateDistance = (from, to) => {
    const [lon1, lat1] = from;
    const [lon2, lat2] = to;
    const R = 6371;
    const dLat = ((lat2 - lat1) * Math.PI) / 180;
    const dLon = ((lon2 - lon1) * Math.PI) / 180;
    const a =
      Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos((lat1 * Math.PI) / 180) *
        Math.cos((lat2 * Math.PI) / 180) *
        Math.sin(dLon / 2) *
        Math.sin(dLon / 2);
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    return (R * c).toFixed(1);
  };

  const handleStationClick = (station) => {
    setSelectedStation(station);
    if (userLocation && station.lat && station.lng) {
      destRef.current = { lat: station.lat, lng: station.lng };
      updateRoute(userLocation, destRef.current);
    }
  };

  const handleStationSelectFromSearch = (station) => {
    setSelectedStation(station);
    if (userLocation && station.lat && station.lng) {
      destRef.current = { lat: station.lat, lng: station.lng };
      updateRoute(userLocation, destRef.current);
      return;
    }

    notifyWarning("Trạm này chưa có tọa độ để chỉ đường.");
  };

  const updateRoute = async (start, dest) => {
    try {
      const [userLat, userLng] = [start[1], start[0]];
      const routeUrl = `https://maps.vietmap.vn/api/route?api-version=1.1&apikey=${API_KEY}&point=${userLat},${userLng}&point=${dest.lat},${dest.lng}&points_encoded=false&vehicle=car`;
      const response = await fetch(routeUrl);
      const json = await response.json();
      const path = json?.paths?.[0];
      if (!path) return;

      setRoute({
        type: "Feature",
        geometry: path.points,
      });

      setRouteInfo({
        distance: (path.distance / 1000).toFixed(1),
        time: (path.time / 60000).toFixed(1),
        dest,
      });
    } catch {}
  };

  return (
    <div className="w-full min-h-screen bg-gray-50 mt-[7rem]">
      <div className="bg-white border-b border-gray-200 px-6 py-10">
        <div className="max-w-7xl mx-auto">
          <div className="text-center mb-8">
            <h1 className="text-6xl font-bold text-gray-900 mb-4">Trạm Đổi Pin</h1>
            <p className="text-3xl text-gray-600">
              Danh sách trạm dựa trên API public `/api/v1/stations`
            </p>
          </div>

          <div className="w-full max-w-4xl mx-auto">
            <StationSearch
              onStationSelect={handleStationSelectFromSearch}
              API_KEY={API_KEY}
              userLocation={userLocation}
            />
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-6 py-10">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2">
            <div className="bg-white rounded-lg shadow-md overflow-hidden relative">
              <VietMapPlaces
                stations={stationsWithCoords}
                route={route}
                routeInfo={routeInfo}
                userLocation={userLocation}
                API_KEY={API_KEY}
                mode="route"
              />

              {routeInfo && (
                <div className="absolute top-6 left-6 bg-white rounded-lg shadow-lg px-6 py-4 z-10">
                  <div className="flex items-center gap-3 text-lg">
                    <Navigation className="w-6 h-6 text-blue-600" />
                    <span className="font-semibold">{routeInfo.distance} km</span>
                    <span className="text-gray-400">•</span>
                    <span className="text-gray-600">{routeInfo.time} phút</span>
                  </div>
                </div>
              )}
            </div>
          </div>

          <div className="lg:col-span-1">
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="bg-blue-600 px-6 py-5">
                <h2 className="text-white font-semibold text-3xl">
                  Danh sách trạm ({stations.length})
                </h2>
              </div>

              <div className="overflow-y-auto" style={{ maxHeight: "680px" }}>
                {stationsWithCoords.length === 0 ? (
                  <div className="p-10 text-center text-gray-500 text-2xl">
                    Đang tải danh sách trạm...
                  </div>
                ) : (
                  <div className="divide-y divide-gray-200">
                    {stationsWithCoords.map((station) => (
                      <div
                        key={station.stationId}
                        onClick={() => handleStationClick(station)}
                        className={`p-6 cursor-pointer transition hover:bg-blue-50 ${
                          selectedStation?.stationId === station.stationId
                            ? "bg-blue-50 border-l-4 border-blue-600"
                            : ""
                        }`}
                      >
                        <div className="flex items-start justify-between mb-3">
                          <h3 className="font-semibold text-gray-900 text-2xl">
                            {station.stationName || station.address}
                          </h3>
                          <span
                            className={`px-3 py-2 rounded-full text-xl font-medium ${
                              station.status
                                ? "bg-green-100 text-green-700"
                                : "bg-red-100 text-red-700"
                            }`}
                          >
                            {station.status ? "Hoạt động" : "Ngừng"}
                          </span>
                        </div>

                        <div className="flex items-start gap-3 mb-3">
                          <MapPin className="w-6 h-6 text-gray-400 mt-1 flex-shrink-0" />
                          <p className="text-xl text-gray-600 leading-relaxed">
                            {station.address}
                          </p>
                        </div>

                        <div className="flex items-center gap-6 text-xl text-gray-500 mt-4">
                          <div className="flex items-center gap-2">
                            <Battery className="w-5 h-5" />
                            <span>{station.batteryQuantity ?? 0} pin</span>
                          </div>
                          {station.distance && (
                            <div className="flex items-center gap-2">
                              <Navigation className="w-5 h-5" />
                              <span>{station.distance} km</span>
                            </div>
                          )}
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Stations;
