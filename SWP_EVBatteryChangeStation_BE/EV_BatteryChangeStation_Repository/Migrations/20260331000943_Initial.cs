using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_BatteryChangeStation_Repository.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BatteryType",
                columns: table => new
                {
                    BatteryTypeID = table.Column<Guid>(type: "uuid", nullable: false),
                    BatteryTypeCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BatteryTypeName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Voltage = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    Capacity_kWh = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryType", x => x.BatteryTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    PermissionID = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PermissionName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ModuleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.PermissionID);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Station",
                columns: table => new
                {
                    StationID = table.Column<Guid>(type: "uuid", nullable: false),
                    StationName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    OperatingHours = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: true),
                    CurrentBatteryCount = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Station", x => x.StationID);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlan",
                columns: table => new
                {
                    PlanID = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PlanName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SwapLimitPerMonth = table.Column<int>(type: "integer", nullable: true),
                    DurationDays = table.Column<int>(type: "integer", nullable: false),
                    ExtraFeePerSwap = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlan", x => x.PlanID);
                });

            migrationBuilder.CreateTable(
                name: "VehicleModel",
                columns: table => new
                {
                    ModelID = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Producer = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    BatteryTypeID = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleModel", x => x.ModelID);
                    table.ForeignKey(
                        name: "FK_VehicleModel_BatteryType_BatteryTypeID",
                        column: x => x.BatteryTypeID,
                        principalTable: "BatteryType",
                        principalColumn: "BatteryTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    RoleID = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK_Account_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    RolePermissionID = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleID = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.RolePermissionID);
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "Permission",
                        principalColumn: "PermissionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Battery",
                columns: table => new
                {
                    BatteryID = table.Column<Guid>(type: "uuid", nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BatteryTypeID = table.Column<Guid>(type: "uuid", nullable: false),
                    Capacity_kWh = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    StateOfHealth = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    CurrentChargeLevel = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StationID = table.Column<Guid>(type: "uuid", nullable: true),
                    InsuranceDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LastChargedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Battery", x => x.BatteryID);
                    table.ForeignKey(
                        name: "FK_Battery_BatteryType_BatteryTypeID",
                        column: x => x.BatteryTypeID,
                        principalTable: "BatteryType",
                        principalColumn: "BatteryTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Battery_Station_StationID",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID");
                });

            migrationBuilder.CreateTable(
                name: "StationBatteryType",
                columns: table => new
                {
                    StationBatteryTypeID = table.Column<Guid>(type: "uuid", nullable: false),
                    StationID = table.Column<Guid>(type: "uuid", nullable: false),
                    BatteryTypeID = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationBatteryType", x => x.StationBatteryTypeID);
                    table.ForeignKey(
                        name: "FK_StationBatteryType_BatteryType_BatteryTypeID",
                        column: x => x.BatteryTypeID,
                        principalTable: "BatteryType",
                        principalColumn: "BatteryTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StationBatteryType_Station_StationID",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StationInventoryLog",
                columns: table => new
                {
                    LogID = table.Column<Guid>(type: "uuid", nullable: false),
                    StationID = table.Column<Guid>(type: "uuid", nullable: false),
                    BatteryTypeID = table.Column<Guid>(type: "uuid", nullable: true),
                    LogTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AvailableBatteries = table.Column<int>(type: "integer", nullable: false),
                    ReservedBatteries = table.Column<int>(type: "integer", nullable: false),
                    ChargingBatteries = table.Column<int>(type: "integer", nullable: false),
                    InVehicleBatteries = table.Column<int>(type: "integer", nullable: false),
                    MaintenanceBatteries = table.Column<int>(type: "integer", nullable: false),
                    AvgChargeLevel = table.Column<decimal>(type: "numeric(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationInventoryLog", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_StationInventoryLog_BatteryType_BatteryTypeID",
                        column: x => x.BatteryTypeID,
                        principalTable: "BatteryType",
                        principalColumn: "BatteryTypeID");
                    table.ForeignKey(
                        name: "FK_StationInventoryLog_Station_StationID",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StationStaffAssignment",
                columns: table => new
                {
                    AssignmentID = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffID = table.Column<Guid>(type: "uuid", nullable: false),
                    StationID = table.Column<Guid>(type: "uuid", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationStaffAssignment", x => x.AssignmentID);
                    table.ForeignKey(
                        name: "FK_StationStaffAssignment_Account_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StationStaffAssignment_Station_StationID",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    VehicleID = table.Column<Guid>(type: "uuid", nullable: false),
                    VIN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LicensePlate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ModelID = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerID = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentBatteryID = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.VehicleID);
                    table.ForeignKey(
                        name: "FK_Vehicle_Account_OwnerID",
                        column: x => x.OwnerID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vehicle_Battery_CurrentBatteryID",
                        column: x => x.CurrentBatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID");
                    table.ForeignKey(
                        name: "FK_Vehicle_VehicleModel_ModelID",
                        column: x => x.ModelID,
                        principalTable: "VehicleModel",
                        principalColumn: "ModelID");
                });

            migrationBuilder.CreateTable(
                name: "BatteryHistory",
                columns: table => new
                {
                    HistoryID = table.Column<Guid>(type: "uuid", nullable: false),
                    BatteryID = table.Column<Guid>(type: "uuid", nullable: false),
                    FromStationID = table.Column<Guid>(type: "uuid", nullable: true),
                    ToStationID = table.Column<Guid>(type: "uuid", nullable: true),
                    FromVehicleID = table.Column<Guid>(type: "uuid", nullable: true),
                    ToVehicleID = table.Column<Guid>(type: "uuid", nullable: true),
                    ActionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FromStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ToStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SoHAtTime = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ChargeLevelAtTime = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    Note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ActorAccountID = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryHistory", x => x.HistoryID);
                    table.ForeignKey(
                        name: "FK_BatteryHistory_Account_ActorAccountID",
                        column: x => x.ActorAccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK_BatteryHistory_Battery_BatteryID",
                        column: x => x.BatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatteryHistory_Station_FromStationID",
                        column: x => x.FromStationID,
                        principalTable: "Station",
                        principalColumn: "StationID");
                    table.ForeignKey(
                        name: "FK_BatteryHistory_Station_ToStationID",
                        column: x => x.ToStationID,
                        principalTable: "Station",
                        principalColumn: "StationID");
                    table.ForeignKey(
                        name: "FK_BatteryHistory_Vehicle_FromVehicleID",
                        column: x => x.FromVehicleID,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleID");
                    table.ForeignKey(
                        name: "FK_BatteryHistory_Vehicle_ToVehicleID",
                        column: x => x.ToVehicleID,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleID");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    BookingID = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleID = table.Column<Guid>(type: "uuid", nullable: false),
                    StationID = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedBatteryTypeID = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StaffNote = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.BookingID);
                    table.ForeignKey(
                        name: "FK_Booking_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_Account_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK_Booking_BatteryType_RequestedBatteryTypeID",
                        column: x => x.RequestedBatteryTypeID,
                        principalTable: "BatteryType",
                        principalColumn: "BatteryTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_Station_StationID",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_Vehicle_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscription",
                columns: table => new
                {
                    UserSubscriptionID = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleID = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanID = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemainingSwaps = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscription", x => x.UserSubscriptionID);
                    table.ForeignKey(
                        name: "FK_UserSubscription_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSubscription_SubscriptionPlan_PlanID",
                        column: x => x.PlanID,
                        principalTable: "SubscriptionPlan",
                        principalColumn: "PlanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSubscription_Vehicle_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatteryReturnInspection",
                columns: table => new
                {
                    InspectionID = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingID = table.Column<Guid>(type: "uuid", nullable: false),
                    BatteryID = table.Column<Guid>(type: "uuid", nullable: false),
                    StationID = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffID = table.Column<Guid>(type: "uuid", nullable: false),
                    SoHPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    PhysicalCondition = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InspectionNote = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    NextStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryReturnInspection", x => x.InspectionID);
                    table.ForeignKey(
                        name: "FK_BatteryReturnInspection_Account_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatteryReturnInspection_Battery_BatteryID",
                        column: x => x.BatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatteryReturnInspection_Booking_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Booking",
                        principalColumn: "BookingID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatteryReturnInspection_Station_StationID",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SwappingTransaction",
                columns: table => new
                {
                    TransactionID = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingID = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleID = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffID = table.Column<Guid>(type: "uuid", nullable: false),
                    StationID = table.Column<Guid>(type: "uuid", nullable: false),
                    ReturnedBatteryID = table.Column<Guid>(type: "uuid", nullable: false),
                    ReturnedBatterySoH = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ReturnedBatteryCharge = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ReturnedBatteryCondition = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ReleasedBatteryID = table.Column<Guid>(type: "uuid", nullable: false),
                    ReleasedBatterySoH = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ReleasedBatteryCharge = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    SwapFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UsedSubscription = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwappingTransaction", x => x.TransactionID);
                    table.ForeignKey(
                        name: "FK_SwappingTransaction_Account_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SwappingTransaction_Battery_ReleasedBatteryID",
                        column: x => x.ReleasedBatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SwappingTransaction_Battery_ReturnedBatteryID",
                        column: x => x.ReturnedBatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SwappingTransaction_Booking_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Booking",
                        principalColumn: "BookingID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SwappingTransaction_Station_StationID",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SwappingTransaction_Vehicle_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    FeedbackID = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    StationID = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingID = table.Column<Guid>(type: "uuid", nullable: true),
                    TransactionID = table.Column<Guid>(type: "uuid", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.FeedbackID);
                    table.ForeignKey(
                        name: "FK_Feedback_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedback_Booking_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Booking",
                        principalColumn: "BookingID");
                    table.ForeignKey(
                        name: "FK_Feedback_Station_StationID",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedback_SwappingTransaction_TransactionID",
                        column: x => x.TransactionID,
                        principalTable: "SwappingTransaction",
                        principalColumn: "TransactionID");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentID = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingID = table.Column<Guid>(type: "uuid", nullable: true),
                    UserSubscriptionID = table.Column<Guid>(type: "uuid", nullable: true),
                    TransactionID = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PaymentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TransactionReference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentGatewayId = table.Column<long>(type: "bigint", nullable: true),
                    RecordedByAccountID = table.Column<Guid>(type: "uuid", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_Payment_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_Account_RecordedByAccountID",
                        column: x => x.RecordedByAccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK_Payment_Booking_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Booking",
                        principalColumn: "BookingID");
                    table.ForeignKey(
                        name: "FK_Payment_SwappingTransaction_TransactionID",
                        column: x => x.TransactionID,
                        principalTable: "SwappingTransaction",
                        principalColumn: "TransactionID");
                    table.ForeignKey(
                        name: "FK_Payment_UserSubscription_UserSubscriptionID",
                        column: x => x.UserSubscriptionID,
                        principalTable: "UserSubscription",
                        principalColumn: "UserSubscriptionID");
                });

            migrationBuilder.CreateTable(
                name: "SupportRequest",
                columns: table => new
                {
                    RequestID = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    StationID = table.Column<Guid>(type: "uuid", nullable: true),
                    BookingID = table.Column<Guid>(type: "uuid", nullable: true),
                    TransactionID = table.Column<Guid>(type: "uuid", nullable: true),
                    IssueType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportRequest", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_SupportRequest_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportRequest_Booking_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Booking",
                        principalColumn: "BookingID");
                    table.ForeignKey(
                        name: "FK_SupportRequest_Station_StationID",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID");
                    table.ForeignKey(
                        name: "FK_SupportRequest_SwappingTransaction_TransactionID",
                        column: x => x.TransactionID,
                        principalTable: "SwappingTransaction",
                        principalColumn: "TransactionID");
                });

            migrationBuilder.CreateTable(
                name: "SupportRequestResponse",
                columns: table => new
                {
                    ResponseID = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestID = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffID = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponseMessage = table.Column<string>(type: "text", nullable: false),
                    StatusAfterResponse = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportRequestResponse", x => x.ResponseID);
                    table.ForeignKey(
                        name: "FK_SupportRequestResponse_Account_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportRequestResponse_SupportRequest_RequestID",
                        column: x => x.RequestID,
                        principalTable: "SupportRequest",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Email",
                table: "Account",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_RoleID",
                table: "Account",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Account_Username",
                table: "Account",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Battery_BatteryTypeID",
                table: "Battery",
                column: "BatteryTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Battery_SerialNumber",
                table: "Battery",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Battery_StationID",
                table: "Battery",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryHistory_ActorAccountID",
                table: "BatteryHistory",
                column: "ActorAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryHistory_BatteryID",
                table: "BatteryHistory",
                column: "BatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryHistory_FromStationID",
                table: "BatteryHistory",
                column: "FromStationID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryHistory_FromVehicleID",
                table: "BatteryHistory",
                column: "FromVehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryHistory_ToStationID",
                table: "BatteryHistory",
                column: "ToStationID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryHistory_ToVehicleID",
                table: "BatteryHistory",
                column: "ToVehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryReturnInspection_BatteryID",
                table: "BatteryReturnInspection",
                column: "BatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryReturnInspection_BookingID",
                table: "BatteryReturnInspection",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryReturnInspection_StaffID",
                table: "BatteryReturnInspection",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryReturnInspection_StationID",
                table: "BatteryReturnInspection",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryType_BatteryTypeCode",
                table: "BatteryType",
                column: "BatteryTypeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_AccountID",
                table: "Booking",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_ApprovedBy",
                table: "Booking",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_RequestedBatteryTypeID",
                table: "Booking",
                column: "RequestedBatteryTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_StationID",
                table: "Booking",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_VehicleID",
                table: "Booking",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_AccountID",
                table: "Feedback",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_BookingID",
                table: "Feedback",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_StationID",
                table: "Feedback",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_TransactionID",
                table: "Feedback",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_AccountID",
                table: "Payment",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_BookingID",
                table: "Payment",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_RecordedByAccountID",
                table: "Payment",
                column: "RecordedByAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_TransactionID",
                table: "Payment",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_TransactionReference",
                table: "Payment",
                column: "TransactionReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_UserSubscriptionID",
                table: "Payment",
                column: "UserSubscriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_PermissionCode",
                table: "Permission",
                column: "PermissionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_RoleName",
                table: "Role",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionID",
                table: "RolePermission",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleID_PermissionID",
                table: "RolePermission",
                columns: new[] { "RoleID", "PermissionID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StationBatteryType_BatteryTypeID",
                table: "StationBatteryType",
                column: "BatteryTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StationBatteryType_StationID_BatteryTypeID",
                table: "StationBatteryType",
                columns: new[] { "StationID", "BatteryTypeID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StationInventoryLog_BatteryTypeID",
                table: "StationInventoryLog",
                column: "BatteryTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StationInventoryLog_StationID",
                table: "StationInventoryLog",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_StationStaffAssignment_StaffID",
                table: "StationStaffAssignment",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_StationStaffAssignment_StationID",
                table: "StationStaffAssignment",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlan_PlanCode",
                table: "SubscriptionPlan",
                column: "PlanCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequest_AccountID",
                table: "SupportRequest",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequest_BookingID",
                table: "SupportRequest",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequest_StationID",
                table: "SupportRequest",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequest_TransactionID",
                table: "SupportRequest",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequestResponse_RequestID",
                table: "SupportRequestResponse",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequestResponse_StaffID",
                table: "SupportRequestResponse",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_SwappingTransaction_BookingID",
                table: "SwappingTransaction",
                column: "BookingID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SwappingTransaction_ReleasedBatteryID",
                table: "SwappingTransaction",
                column: "ReleasedBatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_SwappingTransaction_ReturnedBatteryID",
                table: "SwappingTransaction",
                column: "ReturnedBatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_SwappingTransaction_StaffID",
                table: "SwappingTransaction",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_SwappingTransaction_StationID",
                table: "SwappingTransaction",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_SwappingTransaction_VehicleID",
                table: "SwappingTransaction",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscription_AccountID",
                table: "UserSubscription",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscription_PlanID",
                table: "UserSubscription",
                column: "PlanID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscription_VehicleID",
                table: "UserSubscription",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_CurrentBatteryID",
                table: "Vehicle",
                column: "CurrentBatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_LicensePlate",
                table: "Vehicle",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_ModelID",
                table: "Vehicle",
                column: "ModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_OwnerID",
                table: "Vehicle",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_VIN",
                table: "Vehicle",
                column: "VIN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModel_BatteryTypeID",
                table: "VehicleModel",
                column: "BatteryTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModel_ModelName_Producer",
                table: "VehicleModel",
                columns: new[] { "ModelName", "Producer" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatteryHistory");

            migrationBuilder.DropTable(
                name: "BatteryReturnInspection");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "StationBatteryType");

            migrationBuilder.DropTable(
                name: "StationInventoryLog");

            migrationBuilder.DropTable(
                name: "StationStaffAssignment");

            migrationBuilder.DropTable(
                name: "SupportRequestResponse");

            migrationBuilder.DropTable(
                name: "UserSubscription");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "SupportRequest");

            migrationBuilder.DropTable(
                name: "SubscriptionPlan");

            migrationBuilder.DropTable(
                name: "SwappingTransaction");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Battery");

            migrationBuilder.DropTable(
                name: "VehicleModel");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Station");

            migrationBuilder.DropTable(
                name: "BatteryType");
        }
    }
}
