import Button from "@/components/button";
import React, { useEffect } from "react";
import TabSection from "./../../../components/tab-section/TabSection";
import ContentArea from "@/components/content-area/ContentArea";
import * as Yup from "yup";
import {
  SupportRequestProvider,
  useSupportRequest,
} from "@/context/SupportRequestContext";
import ModalForm from "@/components/modalForm/ModalForm";
import supportRequestService from "@/api/supportRequestService";
import { notifySuccess, notifyError } from "@/components/notification/notification";

const SupportRequestContent = () => {
  const { setIsModalOpen, isModalOpen, addRequest, fetchRequests, loading, setActiveTab } =
    useSupportRequest();

  useEffect(() => {
    fetchRequests();
  }, [fetchRequests]);

  const initialValues = {
    issueType: "",
    description: "",
  };

  const validationSchema = Yup.object({
    issueType: Yup.string().required("Vui lòng chọn loại vấn đề"),
    description: Yup.string()
      .required("Vui lòng nhập mô tả chi tiết")
      .min(10, "Mô tả phải có ít nhất 10 ký tự"),
  });

  const formFields = [
    {
      name: "issueType",
      label: "Loại vấn đề",
      type: "select",
      as: "select",
      placeholder: "Chọn loại vấn đề",
      options: [
        "",
        "Vấn đề kỹ thuật",
        "Thanh toán",
        "Dịch vụ khách hàng",
        "Hỗ trợ dịch vụ Xe",
        "Hỗ trợ dịch vụ Sạc",
        "Khác",
      ],
    },
    {
      name: "description",
      label: "Mô tả chi tiết",
      as: "textarea",
      placeholder: "Nhập mô tả chi tiết về vấn đề của bạn...",
      rows: 5,
    },
  ];

  const handleSubmit = async (values) => {
    try {
      const response = await supportRequestService.createSupportRequest({
        issueType: values.issueType,
        description: values.description,
      });

      addRequest(response);
      setActiveTab("pending");
      setIsModalOpen(false);
      notifySuccess("Yêu cầu hỗ trợ đã được gửi thành công.");
    } catch (error) {
      notifyError("Có lỗi xảy ra khi gửi yêu cầu.");
      throw error;
    }
  };

  return (
    <div className="max-h-screen bg-gray-50">
      <div className="max-w-5xl mx-auto">
        <div className="bg-white rounded-lg shadow-sm overflow-hidden">
          <TabSection />
          {loading ? (
            <div className="flex items-center justify-center py-20">
              <div className="text-lg text-gray-600">Đang tải dữ liệu...</div>
            </div>
          ) : (
            <ContentArea />
          )}
        </div>

        <div className="fixed bottom-8 right-8">
          <Button onclick={() => setIsModalOpen(true)}>Tạo yêu cầu mới</Button>
        </div>

        {isModalOpen && (
          <ModalForm
            title="Yêu cầu hỗ trợ mới"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onSubmit={handleSubmit}
            onClose={() => setIsModalOpen(false)}
            fields={formFields}
          />
        )}
      </div>
    </div>
  );
};

const SupportRequest = () => {
  return (
    <SupportRequestProvider>
      <SupportRequestContent />
    </SupportRequestProvider>
  );
};

export default SupportRequest;
