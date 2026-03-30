-- ==========================================================
-- DATABASE: EVBatterySwap (OPTIMIZED VERSION)
-- Đã giải quyết:
-- 1. Truy vết Pin cũ (Returned Battery)
-- 2. Chuẩn hóa Gói thuê pin (Subscription Normalization)
-- 3. Trạng thái Pin chi tiết (Charging, Maintenance, etc.)
-- 4. Dữ liệu Snapshot cho AI Forecasting
-- 5. Nhật ký hành trình Pin (Battery Lifecycle)
-- ==========================================================

CREATE DATABASE EVBatterySwap_v2;
GO
USE EVBatterySwap_v2;
GO

-- 1. PHÂN QUYỀN & NGƯỜI DÙNG
CREATE TABLE Role (
RoleID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
RoleName NVARCHAR(100) NOT NULL,
Status BIT NOT NULL DEFAULT 1,
CreateDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE Station (
StationID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
StationName NVARCHAR(100) NOT NULL,
Address NVARCHAR(255),
PhoneNumber NVARCHAR(20),
Status NVARCHAR(50) DEFAULT 'Active', -- Active, Maintenance, Offline
MaxCapacity INT, -- Sức chứa tối đa của trạm
CurrentBatteryCount INT DEFAULT 0
);

CREATE TABLE Account (
AccountID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
Username NVARCHAR(100) UNIQUE NOT NULL,
PasswordHash NVARCHAR(MAX) NOT NULL,
FullName NVARCHAR(150),
Email NVARCHAR(150) UNIQUE,
PhoneNumber NVARCHAR(20),
RoleID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Role(RoleID),
StationID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Station(StationID), -- Dành cho Staff trực trạm
Status BIT DEFAULT 1,
CreateDate DATETIME DEFAULT GETDATE(),
UpdateDate DATETIME NULL,
UpdatedBy UNIQUEIDENTIFIER NULL
);

-- 2. TÀI SẢN (ASSETS) & TƯƠNG THÍCH
CREATE TABLE VehicleModel (
ModelID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
ModelName NVARCHAR(100) NOT NULL,
Producer NVARCHAR(100),
CompatibleBatteryType NVARCHAR(100) -- Ràng buộc loại pin có thể dùng
);

CREATE TABLE Vehicle (
VehicleID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
LicensePlate NVARCHAR(20) UNIQUE,
ModelID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES VehicleModel(ModelID),
OwnerID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Account(AccountID),
CurrentBatteryID UNIQUEIDENTIFIER NULL, -- Pin hiện đang nằm trong xe
Status NVARCHAR(50) DEFAULT 'Active'
);

CREATE TABLE Battery (
BatteryID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
SerialNumber NVARCHAR(100) UNIQUE NOT NULL,
TypeBattery NVARCHAR(100) NOT NULL,
Capacity_kWh DECIMAL(10,2),
StateOfHealth DECIMAL(5,2), -- SoH (%)
CurrentChargeLevel DECIMAL(5,2), -- % Pin hiện tại
Status NVARCHAR(50) NOT NULL, -- Available, Charging, Maintenance, In-Vehicle, Faulty
StationID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Station(StationID), -- NULL nếu đang ở trong xe
InsuranceDate DATE,
CreateDate DATETIME DEFAULT GETDATE()
);

-- Bảng quan trọng: Lưu lại lịch sử di chuyển của Pin để AI phân tích vòng đời
CREATE TABLE BatteryHistory (
HistoryID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
BatteryID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Battery(BatteryID),
FromStationID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Station(StationID),
ToStationID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Station(StationID),
ActionType NVARCHAR(50), -- Swap_In, Swap_Out, Maintenance, Charging_Finish
EventDate DATETIME DEFAULT GETDATE(),
SoH_AtTime DECIMAL(5,2),
Note NVARCHAR(255)
);

-- 3. DỊCH VỤ & THUÊ BAO (NORMALIZED)
CREATE TABLE SubscriptionPlan (
PlanID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
PlanName NVARCHAR(100) NOT NULL,
BasePrice DECIMAL(18,2) NOT NULL,
SwapLimitPerMonth INT, -- NULL nếu không giới hạn
Description NVARCHAR(MAX),
IsActive BIT DEFAULT 1
);

CREATE TABLE UserSubscription (
UserSubID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
AccountID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Account(AccountID),
PlanID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES SubscriptionPlan(PlanID),
StartDate DATETIME NOT NULL,
EndDate DATETIME NOT NULL,
RemainingSwaps INT,
Status NVARCHAR(50) DEFAULT 'Active' -- Active, Expired, Cancelled
);

-- 4. VẬN HÀNH (OPERATIONS)
CREATE TABLE Booking (
BookingID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
AccountID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Account(AccountID),
VehicleID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Vehicle(VehicleID),
StationID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Station(StationID),
TargetTime DATETIME NOT NULL,
Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Completed, Cancelled, Expired
Notes NVARCHAR(255)
);

-- Cải tiến SwappingTransaction: Lưu cả pin trả và pin nhận
CREATE TABLE SwappingTransaction (
TransactionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
BookingID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Booking(BookingID),
VehicleID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Vehicle(VehicleID),
StaffID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Account(AccountID),
StationID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Station(StationID),

    ReturnedBatteryID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Battery(BatteryID), -- Pin khách trả lại
    ReturnedBatterySoH DECIMAL(5,2),
    ReturnedBatteryCharge DECIMAL(5,2),

    ReleasedBatteryID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Battery(BatteryID), -- Pin trạm cấp cho khách
    ReleasedBatterySoH DECIMAL(5,2),

    SwapFee DECIMAL(18,2) DEFAULT 0, -- Phí chênh lệch nếu có
    CreateDate DATETIME DEFAULT GETDATE()

);

-- 5. AI FORECASTING DATA (SNAPSHOTS)
-- Bảng này lưu trữ định kỳ trạng thái trạm để AI học nhu cầu theo giờ/ngày
CREATE TABLE StationInventoryLog (
LogID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
StationID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Station(StationID),
LogTime DATETIME DEFAULT GETDATE(),
AvailableBatteries INT,
ChargingBatteries INT,
MaintenanceBatteries INT,
AvgChargeLevel DECIMAL(5,2)
);

-- 6. THANH TOÁN & HỖ TRỢ
CREATE TABLE Payment (
PaymentID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
AccountID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Account(AccountID),
Amount DECIMAL(18,2) NOT NULL,
PaymentType NVARCHAR(50), -- Subscription_Buy, Swap_Extra_Fee
PaymentMethod NVARCHAR(50), -- VNPAY, MoMo, CreditCard
Status NVARCHAR(50) DEFAULT 'Pending',
TransactionReference NVARCHAR(100),
CreateDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE SupportRequest (
RequestID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
AccountID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Account(AccountID),
IssueType NVARCHAR(100),
Description NVARCHAR(MAX),
Status NVARCHAR(50) DEFAULT 'Open', -- Open, In_Progress, Resolved, Closed
CreateDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE Feedback (
FeedbackID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
AccountID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Account(AccountID),
TransactionID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES SwappingTransaction(TransactionID),
Rating INT CHECK (Rating BETWEEN 1 AND 5),
Comment NVARCHAR(MAX),
CreateDate DATETIME DEFAULT GETDATE()
);
GO

-- ==========================================================
-- TRIGGERS & LOGIC CƠ BẢN
-- ==========================================================

-- Trigger cập nhật số lượng pin tại trạm và ghi log lịch sử khi Pin đổi trạng thái
CREATE TRIGGER trg_Battery_LocationChange
ON Battery
AFTER UPDATE
AS
BEGIN
-- 1. Nếu StationID thay đổi, cập nhật CurrentBatteryCount ở trạm
IF UPDATE(StationID)
BEGIN
-- Giảm số lượng ở trạm cũ
UPDATE Station
SET CurrentBatteryCount = CurrentBatteryCount - 1
FROM Station s JOIN Deleted d ON s.StationID = d.StationID
WHERE d.StationID IS NOT NULL;

        -- Tăng số lượng ở trạm mới
        UPDATE Station
        SET CurrentBatteryCount = CurrentBatteryCount + 1
        FROM Station s JOIN Inserted i ON s.StationID = i.StationID
        WHERE i.StationID IS NOT NULL;
    END

END
GO
