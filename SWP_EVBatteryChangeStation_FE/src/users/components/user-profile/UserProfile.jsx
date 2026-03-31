import { useEffect, useState } from "react";
import ProfileHeader from "./ProfileHeader";
import PersonalInfoCard from "./PersonalInfoCard";
import AddressCard from "./AddressCard";
import tokenUtils from "@/utils/tokenUtils";

const UserProfile = () => {
  const [profile, setProfile] = useState({});
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadUserProfile = async () => {
      const userProfile = await tokenUtils.getUserProfile();
      if (userProfile) {
        setProfile({
          ...userProfile,
          fullName: userProfile.fullName || userProfile.name || "",
          gender: userProfile.gender || "",
        });
      }
      setLoading(false);
    };

    loadUserProfile();
  }, []);

  if (loading) {
    return (
      <div className="bg-gray-50 min-h-screen w-full flex items-center justify-center">
        <div className="animate-spin rounded-full h-20 w-20 border-b-2 border-orange-500" />
      </div>
    );
  }

  const showUnsupportedMessage = () => {
    alert("Backend hiện chỉ hỗ trợ xem hồ sơ qua /api/v1/auth/me. Chưa có API cập nhật profile.");
  };

  return (
    <div className="bg-gray-50 min-h-screen w-full flex flex-col items-center py-10 px-4">
      <div className="w-full max-w-5xl mb-6 rounded-2xl border border-amber-200 bg-amber-50 p-4 text-amber-800">
        Hồ sơ hiện đang ở chế độ chỉ xem vì backend chưa có API cập nhật profile.
      </div>

      <h1 className="text-orange-500 text-3xl font-bold mb-8">My Profile</h1>

      <ProfileHeader
        profile={profile}
        tempProfile={profile}
        isEditing={false}
        onChange={() => {}}
      />

      <PersonalInfoCard
        profile={profile}
        tempProfile={profile}
        isEditing={false}
        onEdit={showUnsupportedMessage}
        onCancel={() => {}}
        onChange={() => {}}
      />

      <AddressCard
        address={profile.address}
        tempAddress={profile.address}
        isEditing={false}
        onEdit={showUnsupportedMessage}
        onCancel={() => {}}
        onChange={() => {}}
      />
    </div>
  );
};

export default UserProfile;
