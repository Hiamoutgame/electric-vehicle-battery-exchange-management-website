-- ==========================================================
-- DATABASE: EVBatterySwap_v2
-- PURPOSE:
--   Refactored target schema aligned with:
--   - Docs/BE_document_UserStory.md
--   - Docs/BE_document_APIResult.md
--   - Docs/BE_document_Structure.md
--
-- NOTES:
--   1. This file is the target schema for refactor work.
--   2. The legacy file EVBatterySwap.sql is kept unchanged so the
--      current codebase is not broken immediately.
--   3. The next step after this schema is code and migration alignment.
-- ==========================================================

CREATE DATABASE EVBatterySwap_v2;
GO

USE EVBatterySwap_v2;
GO

-- ==========================================================
-- 1. ACCESS CONTROL
-- ==========================================================

CREATE TABLE Role (
    RoleID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    RoleName NVARCHAR(100) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    CONSTRAINT UQ_Role_RoleName UNIQUE (RoleName),
    CONSTRAINT CK_Role_Status CHECK (Status IN ('ACTIVE', 'INACTIVE'))
);
GO

CREATE TABLE Permission (
    PermissionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PermissionCode NVARCHAR(100) NOT NULL,
    PermissionName NVARCHAR(150) NOT NULL,
    ModuleName NVARCHAR(100) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Permission_Code UNIQUE (PermissionCode),
    CONSTRAINT CK_Permission_Status CHECK (Status IN ('ACTIVE', 'INACTIVE'))
);
GO

CREATE TABLE RolePermission (
    RolePermissionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    RoleID UNIQUEIDENTIFIER NOT NULL,
    PermissionID UNIQUEIDENTIFIER NOT NULL,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_RolePermission_Role FOREIGN KEY (RoleID) REFERENCES Role(RoleID),
    CONSTRAINT FK_RolePermission_Permission FOREIGN KEY (PermissionID) REFERENCES Permission(PermissionID),
    CONSTRAINT UQ_RolePermission UNIQUE (RoleID, PermissionID)
);
GO

-- ==========================================================
-- 2. STATION AND BATTERY TYPE
-- ==========================================================

CREATE TABLE Station (
    StationID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    StationName NVARCHAR(150) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    Area NVARCHAR(100) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Latitude DECIMAL(9, 6) NULL,
    Longitude DECIMAL(9, 6) NULL,
    OperatingHours NVARCHAR(100) NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    MaxCapacity INT NULL,
    CurrentBatteryCount INT NOT NULL DEFAULT 0,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    CONSTRAINT CK_Station_Status CHECK (Status IN ('ACTIVE', 'INACTIVE', 'MAINTENANCE', 'OFFLINE'))
);
GO

CREATE TABLE BatteryType (
    BatteryTypeID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    BatteryTypeCode NVARCHAR(100) NOT NULL,
    BatteryTypeName NVARCHAR(150) NOT NULL,
    Voltage DECIMAL(10, 2) NULL,
    Capacity_kWh DECIMAL(10, 2) NULL,
    Description NVARCHAR(500) NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    CONSTRAINT UQ_BatteryType_Code UNIQUE (BatteryTypeCode),
    CONSTRAINT CK_BatteryType_Status CHECK (Status IN ('ACTIVE', 'INACTIVE'))
);
GO

CREATE TABLE StationBatteryType (
    StationBatteryTypeID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    StationID UNIQUEIDENTIFIER NOT NULL,
    BatteryTypeID UNIQUEIDENTIFIER NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_StationBatteryType_Station FOREIGN KEY (StationID) REFERENCES Station(StationID),
    CONSTRAINT FK_StationBatteryType_BatteryType FOREIGN KEY (BatteryTypeID) REFERENCES BatteryType(BatteryTypeID),
    CONSTRAINT UQ_StationBatteryType UNIQUE (StationID, BatteryTypeID),
    CONSTRAINT CK_StationBatteryType_Status CHECK (Status IN ('ACTIVE', 'INACTIVE'))
);
GO

-- ==========================================================
-- 3. ACCOUNT AND STAFF ASSIGNMENT
-- ==========================================================

CREATE TABLE Account (
    AccountID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Username NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(150) NULL,
    Email NVARCHAR(150) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Gender NVARCHAR(20) NULL,
    Address NVARCHAR(255) NULL,
    DateOfBirth DATE NULL,
    RoleID UNIQUEIDENTIFIER NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    LastLoginAt DATETIME NULL,
    CONSTRAINT FK_Account_Role FOREIGN KEY (RoleID) REFERENCES Role(RoleID),
    CONSTRAINT UQ_Account_Username UNIQUE (Username),
    CONSTRAINT UQ_Account_Email UNIQUE (Email),
    CONSTRAINT CK_Account_Status CHECK (Status IN ('ACTIVE', 'INACTIVE', 'LOCKED'))
);
GO

CREATE TABLE StationStaffAssignment (
    AssignmentID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    StaffID UNIQUEIDENTIFIER NOT NULL,
    StationID UNIQUEIDENTIFIER NOT NULL,
    EffectiveFrom DATE NOT NULL,
    EffectiveTo DATE NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    CONSTRAINT FK_StationStaffAssignment_Staff FOREIGN KEY (StaffID) REFERENCES Account(AccountID),
    CONSTRAINT FK_StationStaffAssignment_Station FOREIGN KEY (StationID) REFERENCES Station(StationID),
    CONSTRAINT CK_StationStaffAssignment_Status CHECK (Status IN ('ACTIVE', 'ENDED', 'CANCELLED')),
    CONSTRAINT CK_StationStaffAssignment_Date CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_StationStaffAssignment_ActiveStaff
ON StationStaffAssignment (StaffID)
WHERE Status = 'ACTIVE';
GO

-- ==========================================================
-- 4. VEHICLE AND BATTERY
-- ==========================================================

CREATE TABLE VehicleModel (
    ModelID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    ModelName NVARCHAR(150) NOT NULL,
    Producer NVARCHAR(150) NULL,
    BatteryTypeID UNIQUEIDENTIFIER NOT NULL,
    Description NVARCHAR(500) NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    CONSTRAINT FK_VehicleModel_BatteryType FOREIGN KEY (BatteryTypeID) REFERENCES BatteryType(BatteryTypeID),
    CONSTRAINT UQ_VehicleModel UNIQUE (ModelName, Producer),
    CONSTRAINT CK_VehicleModel_Status CHECK (Status IN ('ACTIVE', 'INACTIVE'))
);
GO

CREATE TABLE Vehicle (
    VehicleID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    VIN NVARCHAR(50) NULL,
    LicensePlate NVARCHAR(20) NOT NULL,
    ModelID UNIQUEIDENTIFIER NULL,
    OwnerID UNIQUEIDENTIFIER NOT NULL,
    CurrentBatteryID UNIQUEIDENTIFIER NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    CONSTRAINT FK_Vehicle_Model FOREIGN KEY (ModelID) REFERENCES VehicleModel(ModelID),
    CONSTRAINT FK_Vehicle_Owner FOREIGN KEY (OwnerID) REFERENCES Account(AccountID),
    CONSTRAINT UQ_Vehicle_LicensePlate UNIQUE (LicensePlate),
    CONSTRAINT CK_Vehicle_Status CHECK (Status IN ('ACTIVE', 'INACTIVE', 'MAINTENANCE'))
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_Vehicle_VIN
ON Vehicle (VIN)
WHERE VIN IS NOT NULL;
GO

CREATE TABLE Battery (
    BatteryID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    SerialNumber NVARCHAR(100) NOT NULL,
    BatteryTypeID UNIQUEIDENTIFIER NOT NULL,
    Capacity_kWh DECIMAL(10, 2) NULL,
    StateOfHealth DECIMAL(5, 2) NULL,
    CurrentChargeLevel DECIMAL(5, 2) NULL,
    Status NVARCHAR(20) NOT NULL,
    StationID UNIQUEIDENTIFIER NULL,
    InsuranceDate DATE NULL,
    LastChargedAt DATETIME NULL,
    LastUsedAt DATETIME NULL,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    CONSTRAINT FK_Battery_BatteryType FOREIGN KEY (BatteryTypeID) REFERENCES BatteryType(BatteryTypeID),
    CONSTRAINT FK_Battery_Station FOREIGN KEY (StationID) REFERENCES Station(StationID),
    CONSTRAINT UQ_Battery_SerialNumber UNIQUE (SerialNumber),
    CONSTRAINT CK_Battery_Status CHECK (Status IN ('AVAILABLE', 'RESERVED', 'IN_VEHICLE', 'CHARGING', 'MAINTENANCE', 'FAULTY')),
    CONSTRAINT CK_Battery_SoH CHECK (StateOfHealth IS NULL OR (StateOfHealth >= 0 AND StateOfHealth <= 100)),
    CONSTRAINT CK_Battery_Charge CHECK (CurrentChargeLevel IS NULL OR (CurrentChargeLevel >= 0 AND CurrentChargeLevel <= 100))
);
GO

ALTER TABLE Vehicle
ADD CONSTRAINT FK_Vehicle_CurrentBattery FOREIGN KEY (CurrentBatteryID) REFERENCES Battery(BatteryID);
GO

CREATE TABLE BatteryHistory (
    HistoryID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    BatteryID UNIQUEIDENTIFIER NOT NULL,
    FromStationID UNIQUEIDENTIFIER NULL,
    ToStationID UNIQUEIDENTIFIER NULL,
    FromVehicleID UNIQUEIDENTIFIER NULL,
    ToVehicleID UNIQUEIDENTIFIER NULL,
    ActionType NVARCHAR(50) NOT NULL,
    FromStatus NVARCHAR(20) NULL,
    ToStatus NVARCHAR(20) NULL,
    EventDate DATETIME NOT NULL DEFAULT GETDATE(),
    SoHAtTime DECIMAL(5, 2) NULL,
    ChargeLevelAtTime DECIMAL(5, 2) NULL,
    Note NVARCHAR(255) NULL,
    ActorAccountID UNIQUEIDENTIFIER NULL,
    CONSTRAINT FK_BatteryHistory_Battery FOREIGN KEY (BatteryID) REFERENCES Battery(BatteryID),
    CONSTRAINT FK_BatteryHistory_FromStation FOREIGN KEY (FromStationID) REFERENCES Station(StationID),
    CONSTRAINT FK_BatteryHistory_ToStation FOREIGN KEY (ToStationID) REFERENCES Station(StationID),
    CONSTRAINT FK_BatteryHistory_FromVehicle FOREIGN KEY (FromVehicleID) REFERENCES Vehicle(VehicleID),
    CONSTRAINT FK_BatteryHistory_ToVehicle FOREIGN KEY (ToVehicleID) REFERENCES Vehicle(VehicleID),
    CONSTRAINT FK_BatteryHistory_Actor FOREIGN KEY (ActorAccountID) REFERENCES Account(AccountID)
);
GO

-- ==========================================================
-- 5. SUBSCRIPTION
-- ==========================================================

CREATE TABLE SubscriptionPlan (
    PlanID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PlanCode NVARCHAR(100) NOT NULL,
    PlanName NVARCHAR(150) NOT NULL,
    BasePrice DECIMAL(18, 2) NOT NULL,
    Currency NVARCHAR(10) NOT NULL DEFAULT 'VND',
    SwapLimitPerMonth INT NULL,
    DurationDays INT NOT NULL,
    ExtraFeePerSwap DECIMAL(18, 2) NULL,
    Description NVARCHAR(1000) NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    CONSTRAINT UQ_SubscriptionPlan_PlanCode UNIQUE (PlanCode),
    CONSTRAINT CK_SubscriptionPlan_Status CHECK (Status IN ('ACTIVE', 'INACTIVE'))
);
GO

CREATE TABLE UserSubscription (
    UserSubscriptionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    AccountID UNIQUEIDENTIFIER NOT NULL,
    VehicleID UNIQUEIDENTIFIER NOT NULL,
    PlanID UNIQUEIDENTIFIER NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    RemainingSwaps INT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    AutoRenew BIT NOT NULL DEFAULT 0,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    CONSTRAINT FK_UserSubscription_Account FOREIGN KEY (AccountID) REFERENCES Account(AccountID),
    CONSTRAINT FK_UserSubscription_Vehicle FOREIGN KEY (VehicleID) REFERENCES Vehicle(VehicleID),
    CONSTRAINT FK_UserSubscription_Plan FOREIGN KEY (PlanID) REFERENCES SubscriptionPlan(PlanID),
    CONSTRAINT CK_UserSubscription_Status CHECK (Status IN ('ACTIVE', 'EXPIRED', 'CANCELLED')),
    CONSTRAINT CK_UserSubscription_Date CHECK (EndDate > StartDate)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_UserSubscription_ActiveVehicle
ON UserSubscription (VehicleID)
WHERE Status = 'ACTIVE';
GO

-- ==========================================================
-- 6. BOOKING, SWAP, BATTERY INSPECTION
-- ==========================================================

CREATE TABLE Booking (
    BookingID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    AccountID UNIQUEIDENTIFIER NOT NULL,
    VehicleID UNIQUEIDENTIFIER NOT NULL,
    StationID UNIQUEIDENTIFIER NOT NULL,
    RequestedBatteryTypeID UNIQUEIDENTIFIER NOT NULL,
    TargetTime DATETIME NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'PENDING',
    Notes NVARCHAR(255) NULL,
    StaffNote NVARCHAR(255) NULL,
    ApprovedBy UNIQUEIDENTIFIER NULL,
    ApprovedDate DATETIME NULL,
    CancelledDate DATETIME NULL,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    CONSTRAINT FK_Booking_Account FOREIGN KEY (AccountID) REFERENCES Account(AccountID),
    CONSTRAINT FK_Booking_Vehicle FOREIGN KEY (VehicleID) REFERENCES Vehicle(VehicleID),
    CONSTRAINT FK_Booking_Station FOREIGN KEY (StationID) REFERENCES Station(StationID),
    CONSTRAINT FK_Booking_RequestedBatteryType FOREIGN KEY (RequestedBatteryTypeID) REFERENCES BatteryType(BatteryTypeID),
    CONSTRAINT FK_Booking_ApprovedBy FOREIGN KEY (ApprovedBy) REFERENCES Account(AccountID),
    CONSTRAINT CK_Booking_Status CHECK (Status IN ('PENDING', 'APPROVED', 'REJECTED', 'COMPLETED', 'CANCELLED', 'EXPIRED'))
);
GO

CREATE TABLE SwappingTransaction (
    TransactionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    BookingID UNIQUEIDENTIFIER NOT NULL,
    VehicleID UNIQUEIDENTIFIER NOT NULL,
    StaffID UNIQUEIDENTIFIER NOT NULL,
    StationID UNIQUEIDENTIFIER NOT NULL,
    ReturnedBatteryID UNIQUEIDENTIFIER NOT NULL,
    ReturnedBatterySoH DECIMAL(5, 2) NULL,
    ReturnedBatteryCharge DECIMAL(5, 2) NULL,
    ReturnedBatteryCondition NVARCHAR(20) NULL,
    ReleasedBatteryID UNIQUEIDENTIFIER NOT NULL,
    ReleasedBatterySoH DECIMAL(5, 2) NULL,
    ReleasedBatteryCharge DECIMAL(5, 2) NULL,
    SwapFee DECIMAL(18, 2) NOT NULL DEFAULT 0,
    UsedSubscription BIT NOT NULL DEFAULT 0,
    Notes NVARCHAR(255) NULL,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_SwappingTransaction_Booking FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
    CONSTRAINT FK_SwappingTransaction_Vehicle FOREIGN KEY (VehicleID) REFERENCES Vehicle(VehicleID),
    CONSTRAINT FK_SwappingTransaction_Staff FOREIGN KEY (StaffID) REFERENCES Account(AccountID),
    CONSTRAINT FK_SwappingTransaction_Station FOREIGN KEY (StationID) REFERENCES Station(StationID),
    CONSTRAINT FK_SwappingTransaction_ReturnedBattery FOREIGN KEY (ReturnedBatteryID) REFERENCES Battery(BatteryID),
    CONSTRAINT FK_SwappingTransaction_ReleasedBattery FOREIGN KEY (ReleasedBatteryID) REFERENCES Battery(BatteryID),
    CONSTRAINT CK_SwappingTransaction_BatteryCondition CHECK (
        ReturnedBatteryCondition IS NULL OR ReturnedBatteryCondition IN ('NORMAL', 'DAMAGED', 'FAULTY')
    )
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_SwappingTransaction_Booking
ON SwappingTransaction (BookingID);
GO

CREATE TABLE BatteryReturnInspection (
    InspectionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    BookingID UNIQUEIDENTIFIER NOT NULL,
    BatteryID UNIQUEIDENTIFIER NOT NULL,
    StationID UNIQUEIDENTIFIER NOT NULL,
    StaffID UNIQUEIDENTIFIER NOT NULL,
    SoHPercent DECIMAL(5, 2) NULL,
    PhysicalCondition NVARCHAR(20) NOT NULL,
    InspectionNote NVARCHAR(255) NULL,
    NextStatus NVARCHAR(20) NOT NULL,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_BatteryReturnInspection_Booking FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
    CONSTRAINT FK_BatteryReturnInspection_Battery FOREIGN KEY (BatteryID) REFERENCES Battery(BatteryID),
    CONSTRAINT FK_BatteryReturnInspection_Station FOREIGN KEY (StationID) REFERENCES Station(StationID),
    CONSTRAINT FK_BatteryReturnInspection_Staff FOREIGN KEY (StaffID) REFERENCES Account(AccountID),
    CONSTRAINT CK_BatteryReturnInspection_PhysicalCondition CHECK (PhysicalCondition IN ('NORMAL', 'DAMAGED', 'FAULTY')),
    CONSTRAINT CK_BatteryReturnInspection_NextStatus CHECK (NextStatus IN ('AVAILABLE', 'CHARGING', 'MAINTENANCE', 'FAULTY'))
);
GO

-- ==========================================================
-- 7. INVENTORY SNAPSHOT FOR AI / REPORTS
-- ==========================================================

CREATE TABLE StationInventoryLog (
    LogID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    StationID UNIQUEIDENTIFIER NOT NULL,
    BatteryTypeID UNIQUEIDENTIFIER NULL,
    LogTime DATETIME NOT NULL DEFAULT GETDATE(),
    AvailableBatteries INT NOT NULL DEFAULT 0,
    ReservedBatteries INT NOT NULL DEFAULT 0,
    ChargingBatteries INT NOT NULL DEFAULT 0,
    InVehicleBatteries INT NOT NULL DEFAULT 0,
    MaintenanceBatteries INT NOT NULL DEFAULT 0,
    AvgChargeLevel DECIMAL(5, 2) NULL,
    CONSTRAINT FK_StationInventoryLog_Station FOREIGN KEY (StationID) REFERENCES Station(StationID),
    CONSTRAINT FK_StationInventoryLog_BatteryType FOREIGN KEY (BatteryTypeID) REFERENCES BatteryType(BatteryTypeID)
);
GO

-- ==========================================================
-- 8. PAYMENT
-- ==========================================================

CREATE TABLE Payment (
    PaymentID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    AccountID UNIQUEIDENTIFIER NOT NULL,
    BookingID UNIQUEIDENTIFIER NULL,
    UserSubscriptionID UNIQUEIDENTIFIER NULL,
    TransactionID UNIQUEIDENTIFIER NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    Currency NVARCHAR(10) NOT NULL DEFAULT 'VND',
    PaymentType NVARCHAR(30) NOT NULL,
    PaymentMethod NVARCHAR(30) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'PENDING',
    TransactionReference NVARCHAR(100) NULL,
    PaymentGatewayId BIGINT NULL,
    RecordedByAccountID UNIQUEIDENTIFIER NULL,
    PaidAt DATETIME NULL,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Payment_Account FOREIGN KEY (AccountID) REFERENCES Account(AccountID),
    CONSTRAINT FK_Payment_Booking FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
    CONSTRAINT FK_Payment_UserSubscription FOREIGN KEY (UserSubscriptionID) REFERENCES UserSubscription(UserSubscriptionID),
    CONSTRAINT FK_Payment_Transaction FOREIGN KEY (TransactionID) REFERENCES SwappingTransaction(TransactionID),
    CONSTRAINT FK_Payment_RecordedBy FOREIGN KEY (RecordedByAccountID) REFERENCES Account(AccountID),
    CONSTRAINT CK_Payment_Type CHECK (PaymentType IN ('SUBSCRIPTION_PURCHASE', 'SWAP_EXTRA_FEE')),
    CONSTRAINT CK_Payment_Method CHECK (PaymentMethod IN ('CASH', 'MOMO', 'VNPAY', 'CREDIT_CARD')),
    CONSTRAINT CK_Payment_Status CHECK (Status IN ('PENDING', 'PAID', 'FAILED', 'CANCELLED'))
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_Payment_TransactionReference
ON Payment (TransactionReference)
WHERE TransactionReference IS NOT NULL;
GO

-- ==========================================================
-- 9. SUPPORT AND FEEDBACK
-- ==========================================================

CREATE TABLE SupportRequest (
    RequestID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    AccountID UNIQUEIDENTIFIER NOT NULL,
    StationID UNIQUEIDENTIFIER NULL,
    BookingID UNIQUEIDENTIFIER NULL,
    TransactionID UNIQUEIDENTIFIER NULL,
    IssueType NVARCHAR(100) NOT NULL,
    Subject NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Priority NVARCHAR(20) NOT NULL DEFAULT 'MEDIUM',
    Status NVARCHAR(20) NOT NULL DEFAULT 'OPEN',
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    ClosedDate DATETIME NULL,
    CONSTRAINT FK_SupportRequest_Account FOREIGN KEY (AccountID) REFERENCES Account(AccountID),
    CONSTRAINT FK_SupportRequest_Station FOREIGN KEY (StationID) REFERENCES Station(StationID),
    CONSTRAINT FK_SupportRequest_Booking FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
    CONSTRAINT FK_SupportRequest_Transaction FOREIGN KEY (TransactionID) REFERENCES SwappingTransaction(TransactionID),
    CONSTRAINT CK_SupportRequest_Priority CHECK (Priority IN ('LOW', 'MEDIUM', 'HIGH', 'URGENT')),
    CONSTRAINT CK_SupportRequest_Status CHECK (Status IN ('OPEN', 'IN_PROGRESS', 'RESOLVED', 'CLOSED'))
);
GO

CREATE TABLE SupportRequestResponse (
    ResponseID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    RequestID UNIQUEIDENTIFIER NOT NULL,
    StaffID UNIQUEIDENTIFIER NOT NULL,
    ResponseMessage NVARCHAR(MAX) NOT NULL,
    StatusAfterResponse NVARCHAR(20) NOT NULL,
    RespondedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_SupportRequestResponse_Request FOREIGN KEY (RequestID) REFERENCES SupportRequest(RequestID),
    CONSTRAINT FK_SupportRequestResponse_Staff FOREIGN KEY (StaffID) REFERENCES Account(AccountID),
    CONSTRAINT CK_SupportRequestResponse_Status CHECK (StatusAfterResponse IN ('OPEN', 'IN_PROGRESS', 'RESOLVED', 'CLOSED'))
);
GO

CREATE TABLE Feedback (
    FeedbackID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    AccountID UNIQUEIDENTIFIER NOT NULL,
    StationID UNIQUEIDENTIFIER NOT NULL,
    BookingID UNIQUEIDENTIFIER NULL,
    TransactionID UNIQUEIDENTIFIER NULL,
    Rating INT NOT NULL,
    Comment NVARCHAR(MAX) NULL,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Feedback_Account FOREIGN KEY (AccountID) REFERENCES Account(AccountID),
    CONSTRAINT FK_Feedback_Station FOREIGN KEY (StationID) REFERENCES Station(StationID),
    CONSTRAINT FK_Feedback_Booking FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
    CONSTRAINT FK_Feedback_Transaction FOREIGN KEY (TransactionID) REFERENCES SwappingTransaction(TransactionID),
    CONSTRAINT CK_Feedback_Rating CHECK (Rating BETWEEN 1 AND 5)
);
GO

-- ==========================================================
-- 10. INDEXES
-- ==========================================================

CREATE NONCLUSTERED INDEX IX_Account_RoleID ON Account(RoleID);
CREATE NONCLUSTERED INDEX IX_Station_Status ON Station(Status);
CREATE NONCLUSTERED INDEX IX_Battery_Station_Status ON Battery(StationID, Status);
CREATE NONCLUSTERED INDEX IX_Battery_BatteryType_Status ON Battery(BatteryTypeID, Status);
CREATE NONCLUSTERED INDEX IX_Booking_Station_Status_TargetTime ON Booking(StationID, Status, TargetTime);
CREATE NONCLUSTERED INDEX IX_Booking_Account_CreateDate ON Booking(AccountID, CreateDate);
CREATE NONCLUSTERED INDEX IX_Swap_Station_CreateDate ON SwappingTransaction(StationID, CreateDate);
CREATE NONCLUSTERED INDEX IX_Payment_Account_CreateDate ON Payment(AccountID, CreateDate);
CREATE NONCLUSTERED INDEX IX_SupportRequest_Station_Status ON SupportRequest(StationID, Status);
CREATE NONCLUSTERED INDEX IX_SupportRequest_Account_CreateDate ON SupportRequest(AccountID, CreateDate);
GO

-- ==========================================================
-- 11. TRIGGER: REFRESH CURRENT BATTERY COUNT
-- ==========================================================

CREATE TRIGGER trg_Battery_RefreshStationCounts
ON Battery
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH ImpactedStations AS (
        SELECT StationID FROM inserted WHERE StationID IS NOT NULL
        UNION
        SELECT StationID FROM deleted WHERE StationID IS NOT NULL
    ),
    BatteryCounts AS (
        SELECT b.StationID, COUNT(*) AS TotalCount
        FROM Battery b
        INNER JOIN ImpactedStations i ON i.StationID = b.StationID
        GROUP BY b.StationID
    )
    UPDATE s
    SET s.CurrentBatteryCount = ISNULL(bc.TotalCount, 0)
    FROM Station s
    INNER JOIN ImpactedStations i ON i.StationID = s.StationID
    LEFT JOIN BatteryCounts bc ON bc.StationID = s.StationID;
END
GO

-- ==========================================================
-- 12. STARTER DATA
-- ==========================================================

INSERT INTO Role (RoleName) VALUES
    ('ADMIN'),
    ('STAFF'),
    ('DRIVER');
GO

INSERT INTO Permission (PermissionCode, PermissionName, ModuleName) VALUES
    ('station.view', 'View station summary', 'Station'),
    ('station.manage', 'Manage station profile', 'Station'),
    ('inventory.view', 'View inventory summary', 'Inventory'),
    ('staff.assign', 'Assign staff to station', 'StationStaffAssignment'),
    ('report.view', 'View reports', 'Report');
GO
