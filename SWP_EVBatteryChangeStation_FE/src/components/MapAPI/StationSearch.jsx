import React, { useEffect, useRef, useState } from "react";
import stationService from "@/api/stationService";
import { vietmapService } from "@/api/vietmapService";
import { MapPin, Search, X } from "lucide-react";
import { notifyError, notifyWarning } from "@/components/notification/notification";

export default function StationSearch({
  onStationSelect,
  API_KEY,
  userLocation,
  className = "",
}) {
  const [searchTerm, setSearchTerm] = useState("");
  const [searchResults, setSearchResults] = useState([]);
  const [isSearching, setIsSearching] = useState(false);
  const [showResults, setShowResults] = useState(false);
  const searchRef = useRef(null);
  const resultsRef = useRef(null);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (
        searchRef.current &&
        !searchRef.current.contains(event.target) &&
        resultsRef.current &&
        !resultsRef.current.contains(event.target)
      ) {
        setShowResults(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

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

  const handleSearch = async () => {
    if (!searchTerm.trim()) {
      setSearchResults([]);
      setShowResults(false);
      return;
    }

    setIsSearching(true);
    setShowResults(true);

    try {
      const stations = await stationService.getStationsByName(searchTerm.trim());
      if (stations.length === 0) {
        notifyWarning("Không tìm thấy trạm nào phù hợp.");
        setSearchResults([]);
        return;
      }

      const stationsWithCoords = await Promise.all(
        stations.map(async (station) => {
          const coords = await vietmapService.geocodeAddress(
            API_KEY,
            station.address,
            userLocation ? { lat: userLocation[1], lng: userLocation[0] } : null
          );

          return {
            ...station,
            lat: coords?.lat || null,
            lng: coords?.lng || null,
            distance:
              userLocation && coords
                ? calculateDistance(userLocation, [coords.lng, coords.lat])
                : null,
          };
        })
      );

      setSearchResults(stationsWithCoords);
    } catch {
      notifyError("Không thể tìm kiếm trạm. Vui lòng thử lại.");
      setSearchResults([]);
    } finally {
      setIsSearching(false);
    }
  };

  const handleStationClick = (station) => {
    if (!station.lat || !station.lng) {
      notifyWarning("Trạm này chưa có tọa độ để chỉ đường.");
      return;
    }

    onStationSelect?.(station);
    setShowResults(false);
    setSearchTerm("");
  };

  return (
    <div className={`relative ${className}`} ref={searchRef}>
      <div className="flex w-full">
        <div className="relative flex-grow">
          <Search className="absolute left-4 top-1/2 transform -translate-y-1/2 w-6 h-6 text-gray-400" />
          <input
            type="text"
            placeholder="Tìm kiếm trạm đổi pin"
            value={searchTerm}
            onChange={(event) => setSearchTerm(event.target.value)}
            onKeyDown={(event) => {
              if (event.key === "Enter") {
                handleSearch();
              }
            }}
            onFocus={() => searchResults.length > 0 && setShowResults(true)}
            className="w-full pl-12 pr-12 py-5 text-2xl outline-none text-gray-700 border border-gray-300 rounded-l-full focus:ring-2 focus:ring-blue-400"
          />
          {searchTerm && (
            <button
              onClick={() => {
                setSearchTerm("");
                setSearchResults([]);
                setShowResults(false);
              }}
              className="absolute right-4 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
            >
              <X className="w-6 h-6" />
            </button>
          )}
        </div>
        <button
          onClick={handleSearch}
          disabled={isSearching}
          className="bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white px-10 py-5 text-2xl font-medium transition rounded-r-full"
        >
          {isSearching ? "Đang tìm..." : "Tìm kiếm"}
        </button>
      </div>

      {showResults && searchResults.length > 0 && (
        <div
          ref={resultsRef}
          className="absolute z-50 w-full mt-2 bg-white rounded-lg shadow-lg border border-gray-200 max-h-96 overflow-y-auto"
        >
          <div className="p-2">
            {searchResults.map((station) => (
              <div
                key={station.stationId}
                onClick={() => handleStationClick(station)}
                className="p-4 cursor-pointer hover:bg-blue-50 transition rounded-lg border-b border-gray-100 last:border-b-0"
              >
                <div className="flex items-start justify-between mb-2">
                  <h3 className="font-semibold text-gray-900 text-xl flex-1">
                    {station.stationName || station.address}
                  </h3>
                  <span
                    className={`px-2 py-1 rounded-full text-xl font-medium ml-2 ${
                      station.status
                        ? "bg-green-100 text-green-700"
                        : "bg-red-100 text-red-700"
                    }`}
                  >
                    {station.status ? "Hoạt động" : "Ngừng"}
                  </span>
                </div>

                <div className="flex items-start gap-2 mb-2">
                  <MapPin className="w-4 h-4 text-gray-400 mt-1 flex-shrink-0" />
                  <p className="text-base text-gray-600 leading-relaxed">
                    {station.address}
                  </p>
                </div>

                {station.distance && (
                  <div className="text-xl text-blue-600 font-medium mt-2">
                    Cách {station.distance} km
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
