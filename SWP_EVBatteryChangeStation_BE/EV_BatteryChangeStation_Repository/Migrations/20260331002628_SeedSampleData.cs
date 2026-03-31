using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EV_BatteryChangeStation_Repository.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BatteryType",
                columns: new[] { "BatteryTypeID", "BatteryTypeCode", "BatteryTypeName", "Capacity_kWh", "CreateDate", "Description", "Status", "UpdateDate", "Voltage" },
                values: new object[,]
                {
                    { new Guid("14000000-0000-0000-0000-000000000001"), "BT-STD-72V", "72V Standard Pack", 3.50m, new DateTime(2026, 1, 7, 7, 30, 0, 0, DateTimeKind.Utc), "Standard battery pack for city scooters.", "ACTIVE", null, 72m },
                    { new Guid("14000000-0000-0000-0000-000000000002"), "BT-FAST-84V", "84V FastCharge Pack", 4.20m, new DateTime(2026, 1, 7, 7, 35, 0, 0, DateTimeKind.Utc), "High-capacity battery pack for delivery vehicles.", "ACTIVE", null, 84m }
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "PermissionID", "CreateDate", "ModuleName", "PermissionCode", "PermissionName", "Status" },
                values: new object[,]
                {
                    { new Guid("11000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 5, 8, 15, 0, 0, DateTimeKind.Utc), "BOOKING", "BOOKING_MANAGE", "Manage booking records", "ACTIVE" },
                    { new Guid("11000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 5, 8, 20, 0, 0, DateTimeKind.Utc), "STATION", "STATION_MANAGE", "Manage station operations", "ACTIVE" },
                    { new Guid("11000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 5, 8, 25, 0, 0, DateTimeKind.Utc), "PAYMENT", "PAYMENT_MANAGE", "Manage payment records", "ACTIVE" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleID", "CreateDate", "RoleName", "Status", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 5, 8, 0, 0, 0, DateTimeKind.Utc), "ADMIN", "ACTIVE", null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 5, 8, 5, 0, 0, DateTimeKind.Utc), "STAFF", "ACTIVE", null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 5, 8, 10, 0, 0, DateTimeKind.Utc), "CUSTOMER", "ACTIVE", null }
                });

            migrationBuilder.InsertData(
                table: "Station",
                columns: new[] { "StationID", "Address", "Area", "CreateDate", "CurrentBatteryCount", "Latitude", "Longitude", "MaxCapacity", "OperatingHours", "PhoneNumber", "StationName", "Status", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("13000000-0000-0000-0000-000000000001"), "12 Vo Van Ngan, Thu Duc, Ho Chi Minh City", "Ho Chi Minh City", new DateTime(2026, 1, 7, 7, 0, 0, 0, DateTimeKind.Utc), 1, 10.850632m, 106.771876m, 20, "06:00-22:00", "02873001001", "EV Swap Thu Duc", "ACTIVE", null },
                    { new Guid("13000000-0000-0000-0000-000000000002"), "88 Tran Thai Tong, Cau Giay, Ha Noi", "Ha Noi", new DateTime(2026, 1, 7, 7, 15, 0, 0, DateTimeKind.Utc), 1, 21.036812m, 105.790310m, 18, "06:00-22:00", "02473001002", "EV Swap Cau Giay", "ACTIVE", null }
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlan",
                columns: new[] { "PlanID", "BasePrice", "CreateDate", "Currency", "Description", "DurationDays", "ExtraFeePerSwap", "PlanCode", "PlanName", "Status", "SwapLimitPerMonth", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("19000000-0000-0000-0000-000000000001"), 199000m, new DateTime(2026, 1, 12, 10, 0, 0, 0, DateTimeKind.Utc), "VND", "Entry package for casual EV users.", 30, 25000m, "PLAN-BASIC", "Basic Monthly", "ACTIVE", 4, null },
                    { new Guid("19000000-0000-0000-0000-000000000002"), 349000m, new DateTime(2026, 1, 12, 10, 15, 0, 0, DateTimeKind.Utc), "VND", "Best for heavy riders and delivery use.", 30, 10000m, "PLAN-PREMIUM", "Premium Monthly", "ACTIVE", 10, null }
                });

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "AccountID", "Address", "CreateDate", "DateOfBirth", "Email", "FullName", "Gender", "LastLoginAt", "PasswordHash", "PhoneNumber", "RoleID", "Status", "UpdateDate", "Username" },
                values: new object[,]
                {
                    { new Guid("12000000-0000-0000-0000-000000000001"), "District 1, Ho Chi Minh City", new DateTime(2026, 1, 6, 8, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1990, 1, 10), "admin.seed@evswap.local", "Seed Admin", "MALE", new DateTime(2026, 3, 30, 18, 0, 0, 0, DateTimeKind.Utc), "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==", "0900000001", new Guid("10000000-0000-0000-0000-000000000001"), "ACTIVE", null, "admin.seed" },
                    { new Guid("12000000-0000-0000-0000-000000000002"), "Thu Duc, Ho Chi Minh City", new DateTime(2026, 1, 6, 8, 15, 0, 0, DateTimeKind.Utc), new DateOnly(1996, 4, 12), "staff.hcm@evswap.local", "Nguyen Van Huy", "MALE", new DateTime(2026, 3, 30, 12, 30, 0, 0, DateTimeKind.Utc), "AAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQ==", "0900000002", new Guid("10000000-0000-0000-0000-000000000002"), "ACTIVE", null, "staff.hcm" },
                    { new Guid("12000000-0000-0000-0000-000000000003"), "Cau Giay, Ha Noi", new DateTime(2026, 1, 6, 8, 30, 0, 0, DateTimeKind.Utc), new DateOnly(1997, 9, 20), "staff.hanoi@evswap.local", "Tran Thu Trang", "FEMALE", new DateTime(2026, 3, 29, 17, 45, 0, 0, DateTimeKind.Utc), "AAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQ==", "0900000003", new Guid("10000000-0000-0000-0000-000000000002"), "ACTIVE", null, "staff.hanoi" },
                    { new Guid("12000000-0000-0000-0000-000000000004"), "Binh Thanh, Ho Chi Minh City", new DateTime(2026, 1, 6, 9, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2000, 5, 18), "pham.minh@evswap.local", "Pham Quoc Minh", "MALE", new DateTime(2026, 3, 31, 7, 20, 0, 0, DateTimeKind.Utc), "AAICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAg==", "0900000004", new Guid("10000000-0000-0000-0000-000000000003"), "ACTIVE", null, "pham.minh" },
                    { new Guid("12000000-0000-0000-0000-000000000005"), "Dong Da, Ha Noi", new DateTime(2026, 1, 6, 9, 15, 0, 0, DateTimeKind.Utc), new DateOnly(1999, 11, 3), "le.lan@evswap.local", "Le Thu Lan", "FEMALE", new DateTime(2026, 3, 31, 8, 10, 0, 0, DateTimeKind.Utc), "AAICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAg==", "0900000005", new Guid("10000000-0000-0000-0000-000000000003"), "ACTIVE", null, "le.lan" }
                });

            migrationBuilder.InsertData(
                table: "Battery",
                columns: new[] { "BatteryID", "BatteryTypeID", "Capacity_kWh", "CreateDate", "CurrentChargeLevel", "InsuranceDate", "LastChargedAt", "LastUsedAt", "SerialNumber", "StateOfHealth", "StationID", "Status", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("16000000-0000-0000-0000-000000000001"), new Guid("14000000-0000-0000-0000-000000000001"), 3.50m, new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), 24.00m, new DateOnly(2025, 6, 1), new DateTime(2026, 3, 28, 17, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 30, 9, 5, 0, 0, DateTimeKind.Utc), "BAT-HCM-0001", 88.50m, new Guid("13000000-0000-0000-0000-000000000001"), "INSPECTING", null },
                    { new Guid("16000000-0000-0000-0000-000000000002"), new Guid("14000000-0000-0000-0000-000000000001"), 3.50m, new DateTime(2026, 1, 10, 8, 15, 0, 0, DateTimeKind.Utc), 91.00m, new DateOnly(2025, 7, 15), new DateTime(2026, 3, 30, 8, 30, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 31, 6, 45, 0, 0, DateTimeKind.Utc), "BAT-HCM-0002", 95.20m, null, "IN_USE", null },
                    { new Guid("16000000-0000-0000-0000-000000000003"), new Guid("14000000-0000-0000-0000-000000000002"), 4.20m, new DateTime(2026, 1, 10, 8, 30, 0, 0, DateTimeKind.Utc), 38.00m, new DateOnly(2025, 5, 20), new DateTime(2026, 3, 29, 22, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 30, 14, 0, 0, 0, DateTimeKind.Utc), "BAT-HNI-0001", 90.10m, new Guid("13000000-0000-0000-0000-000000000002"), "CHARGING", null },
                    { new Guid("16000000-0000-0000-0000-000000000004"), new Guid("14000000-0000-0000-0000-000000000002"), 4.20m, new DateTime(2026, 1, 10, 8, 45, 0, 0, DateTimeKind.Utc), 87.50m, new DateOnly(2025, 8, 1), new DateTime(2026, 3, 30, 13, 20, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 31, 7, 40, 0, 0, DateTimeKind.Utc), "BAT-HNI-0002", 96.40m, null, "IN_USE", null }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "RolePermissionID", "CreateDate", "PermissionID", "RoleID" },
                values: new object[,]
                {
                    { new Guid("11100000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 5, 8, 30, 0, 0, DateTimeKind.Utc), new Guid("11000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("11100000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 5, 8, 31, 0, 0, DateTimeKind.Utc), new Guid("11000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("11100000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 5, 8, 32, 0, 0, DateTimeKind.Utc), new Guid("11000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("11100000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 5, 8, 33, 0, 0, DateTimeKind.Utc), new Guid("11000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("11100000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 5, 8, 34, 0, 0, DateTimeKind.Utc), new Guid("11000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("11100000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 5, 8, 35, 0, 0, DateTimeKind.Utc), new Guid("11000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000003") }
                });

            migrationBuilder.InsertData(
                table: "StationBatteryType",
                columns: new[] { "StationBatteryTypeID", "BatteryTypeID", "CreateDate", "StationID", "Status" },
                values: new object[,]
                {
                    { new Guid("14100000-0000-0000-0000-000000000001"), new Guid("14000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 7, 7, 45, 0, 0, DateTimeKind.Utc), new Guid("13000000-0000-0000-0000-000000000001"), "ACTIVE" },
                    { new Guid("14100000-0000-0000-0000-000000000002"), new Guid("14000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 7, 7, 46, 0, 0, DateTimeKind.Utc), new Guid("13000000-0000-0000-0000-000000000001"), "ACTIVE" },
                    { new Guid("14100000-0000-0000-0000-000000000003"), new Guid("14000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 7, 7, 47, 0, 0, DateTimeKind.Utc), new Guid("13000000-0000-0000-0000-000000000002"), "ACTIVE" }
                });

            migrationBuilder.InsertData(
                table: "StationInventoryLog",
                columns: new[] { "LogID", "AvailableBatteries", "AvgChargeLevel", "BatteryTypeID", "ChargingBatteries", "InVehicleBatteries", "LogTime", "MaintenanceBatteries", "ReservedBatteries", "StationID" },
                values: new object[,]
                {
                    { new Guid("23000000-0000-0000-0000-000000000001"), 0, 57.50m, new Guid("14000000-0000-0000-0000-000000000001"), 0, 1, new DateTime(2026, 3, 30, 18, 0, 0, 0, DateTimeKind.Utc), 1, 0, new Guid("13000000-0000-0000-0000-000000000001") },
                    { new Guid("23000000-0000-0000-0000-000000000002"), 0, 62.75m, new Guid("14000000-0000-0000-0000-000000000002"), 1, 1, new DateTime(2026, 3, 30, 18, 0, 0, 0, DateTimeKind.Utc), 0, 0, new Guid("13000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "VehicleModel",
                columns: new[] { "ModelID", "BatteryTypeID", "CreateDate", "Description", "ModelName", "Producer", "Status", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("15000000-0000-0000-0000-000000000001"), new Guid("14000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 9, 9, 0, 0, 0, DateTimeKind.Utc), "Compact electric scooter for daily commuting.", "E-Scoot S1", "VinFast", "ACTIVE", null },
                    { new Guid("15000000-0000-0000-0000-000000000002"), new Guid("14000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 9, 9, 10, 0, 0, DateTimeKind.Utc), "Utility model for delivery fleets.", "E-Delivery X", "Selex", "ACTIVE", null }
                });

            migrationBuilder.InsertData(
                table: "StationStaffAssignment",
                columns: new[] { "AssignmentID", "CreateDate", "EffectiveFrom", "EffectiveTo", "StaffID", "StationID", "Status", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("14200000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 8, 8, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 1, 8), null, new Guid("12000000-0000-0000-0000-000000000002"), new Guid("13000000-0000-0000-0000-000000000001"), "ACTIVE", null },
                    { new Guid("14200000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 8, 8, 10, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 1, 8), null, new Guid("12000000-0000-0000-0000-000000000003"), new Guid("13000000-0000-0000-0000-000000000002"), "ACTIVE", null }
                });

            migrationBuilder.InsertData(
                table: "Vehicle",
                columns: new[] { "VehicleID", "CreateDate", "CurrentBatteryID", "LicensePlate", "ModelID", "OwnerID", "Status", "UpdateDate", "VIN" },
                values: new object[,]
                {
                    { new Guid("17000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 11, 9, 0, 0, 0, DateTimeKind.Utc), new Guid("16000000-0000-0000-0000-000000000002"), "59A3-12345", new Guid("15000000-0000-0000-0000-000000000001"), new Guid("12000000-0000-0000-0000-000000000004"), "ACTIVE", null, "VIN-HCM-0000000001" },
                    { new Guid("17000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 11, 9, 15, 0, 0, DateTimeKind.Utc), new Guid("16000000-0000-0000-0000-000000000004"), "29B1-67890", new Guid("15000000-0000-0000-0000-000000000002"), new Guid("12000000-0000-0000-0000-000000000005"), "ACTIVE", null, "VIN-HNI-0000000002" }
                });

            migrationBuilder.InsertData(
                table: "BatteryHistory",
                columns: new[] { "HistoryID", "ActionType", "ActorAccountID", "BatteryID", "ChargeLevelAtTime", "EventDate", "FromStationID", "FromStatus", "FromVehicleID", "Note", "SoHAtTime", "ToStationID", "ToStatus", "ToVehicleID" },
                values: new object[,]
                {
                    { new Guid("18000000-0000-0000-0000-000000000001"), "RETURNED", new Guid("12000000-0000-0000-0000-000000000002"), new Guid("16000000-0000-0000-0000-000000000001"), 24.00m, new DateTime(2026, 3, 30, 9, 10, 0, 0, DateTimeKind.Utc), null, "IN_USE", new Guid("17000000-0000-0000-0000-000000000001"), "Battery returned during completed swap booking.", 88.50m, new Guid("13000000-0000-0000-0000-000000000001"), "INSPECTING", null },
                    { new Guid("18000000-0000-0000-0000-000000000002"), "RELEASED", new Guid("12000000-0000-0000-0000-000000000002"), new Guid("16000000-0000-0000-0000-000000000002"), 91.00m, new DateTime(2026, 3, 30, 9, 12, 0, 0, DateTimeKind.Utc), new Guid("13000000-0000-0000-0000-000000000001"), "AVAILABLE", null, "Released to customer after booking approval.", 95.20m, null, "IN_USE", new Guid("17000000-0000-0000-0000-000000000001") },
                    { new Guid("18000000-0000-0000-0000-000000000003"), "RELEASED", new Guid("12000000-0000-0000-0000-000000000003"), new Guid("16000000-0000-0000-0000-000000000004"), 87.50m, new DateTime(2026, 3, 30, 14, 5, 0, 0, DateTimeKind.Utc), new Guid("13000000-0000-0000-0000-000000000002"), "AVAILABLE", null, "Released to vehicle after successful inspection.", 96.40m, null, "IN_USE", new Guid("17000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "Booking",
                columns: new[] { "BookingID", "AccountID", "ApprovedBy", "ApprovedDate", "CancelledDate", "CreateDate", "Notes", "RequestedBatteryTypeID", "StaffNote", "StationID", "Status", "TargetTime", "UpdateDate", "VehicleID" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("12000000-0000-0000-0000-000000000004"), new Guid("12000000-0000-0000-0000-000000000002"), new DateTime(2026, 3, 30, 8, 40, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 29, 20, 0, 0, 0, DateTimeKind.Utc), "Customer requested morning battery swap.", new Guid("14000000-0000-0000-0000-000000000001"), "Completed on time.", new Guid("13000000-0000-0000-0000-000000000001"), "COMPLETED", new DateTime(2026, 3, 30, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 30, 9, 15, 0, 0, DateTimeKind.Utc), new Guid("17000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("12000000-0000-0000-0000-000000000005"), new Guid("12000000-0000-0000-0000-000000000003"), new DateTime(2026, 3, 30, 13, 30, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 29, 21, 0, 0, 0, DateTimeKind.Utc), "Afternoon swap for delivery route.", new Guid("14000000-0000-0000-0000-000000000002"), "Battery health checked before release.", new Guid("13000000-0000-0000-0000-000000000002"), "COMPLETED", new DateTime(2026, 3, 30, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 30, 14, 20, 0, 0, DateTimeKind.Utc), new Guid("17000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "UserSubscription",
                columns: new[] { "UserSubscriptionID", "AccountID", "AutoRenew", "CreateDate", "EndDate", "PlanID", "RemainingSwaps", "StartDate", "Status", "UpdateDate", "VehicleID" },
                values: new object[,]
                {
                    { new Guid("19100000-0000-0000-0000-000000000001"), new Guid("12000000-0000-0000-0000-000000000004"), true, new DateTime(2026, 3, 1, 7, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 31, 23, 59, 0, 0, DateTimeKind.Utc), new Guid("19000000-0000-0000-0000-000000000001"), 2, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ACTIVE", null, new Guid("17000000-0000-0000-0000-000000000001") },
                    { new Guid("19100000-0000-0000-0000-000000000002"), new Guid("12000000-0000-0000-0000-000000000005"), true, new DateTime(2026, 3, 5, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 4, 23, 59, 0, 0, DateTimeKind.Utc), new Guid("19000000-0000-0000-0000-000000000002"), 8, new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), "ACTIVE", null, new Guid("17000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "BatteryReturnInspection",
                columns: new[] { "InspectionID", "BatteryID", "BookingID", "CreateDate", "InspectionNote", "NextStatus", "PhysicalCondition", "SoHPercent", "StaffID", "StationID" },
                values: new object[,]
                {
                    { new Guid("22000000-0000-0000-0000-000000000001"), new Guid("16000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 3, 30, 9, 8, 0, 0, DateTimeKind.Utc), "No exterior damage detected.", "INSPECTING", "GOOD", 88.50m, new Guid("12000000-0000-0000-0000-000000000002"), new Guid("13000000-0000-0000-0000-000000000001") },
                    { new Guid("22000000-0000-0000-0000-000000000002"), new Guid("16000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2026, 3, 30, 14, 2, 0, 0, DateTimeKind.Utc), "Battery moved to charging after inspection.", "CHARGING", "GOOD", 90.10m, new Guid("12000000-0000-0000-0000-000000000003"), new Guid("13000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "Payment",
                columns: new[] { "PaymentID", "AccountID", "Amount", "BookingID", "CreateDate", "Currency", "PaidAt", "PaymentGatewayId", "PaymentMethod", "PaymentType", "RecordedByAccountID", "Status", "TransactionID", "TransactionReference", "UserSubscriptionID" },
                values: new object[,]
                {
                    { new Guid("24000000-0000-0000-0000-000000000001"), new Guid("12000000-0000-0000-0000-000000000004"), 199000m, null, new DateTime(2026, 3, 1, 7, 0, 0, 0, DateTimeKind.Utc), "VND", new DateTime(2026, 3, 1, 7, 5, 0, 0, DateTimeKind.Utc), 100001L, "VNPAY", "SUBSCRIPTION", new Guid("12000000-0000-0000-0000-000000000001"), "PAID", null, "SUB-20260301-001", new Guid("19100000-0000-0000-0000-000000000001") },
                    { new Guid("24000000-0000-0000-0000-000000000002"), new Guid("12000000-0000-0000-0000-000000000005"), 349000m, null, new DateTime(2026, 3, 5, 8, 0, 0, 0, DateTimeKind.Utc), "VND", new DateTime(2026, 3, 5, 8, 5, 0, 0, DateTimeKind.Utc), 100002L, "VNPAY", "SUBSCRIPTION", new Guid("12000000-0000-0000-0000-000000000001"), "PAID", null, "SUB-20260305-001", new Guid("19100000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "SwappingTransaction",
                columns: new[] { "TransactionID", "BookingID", "CreateDate", "Notes", "ReleasedBatteryCharge", "ReleasedBatteryID", "ReleasedBatterySoH", "ReturnedBatteryCharge", "ReturnedBatteryCondition", "ReturnedBatteryID", "ReturnedBatterySoH", "StaffID", "StationID", "SwapFee", "UsedSubscription", "VehicleID" },
                values: new object[,]
                {
                    { new Guid("21000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 3, 30, 9, 12, 0, 0, DateTimeKind.Utc), "Covered by active subscription plan.", 91.00m, new Guid("16000000-0000-0000-0000-000000000002"), 95.20m, 24.00m, "GOOD", new Guid("16000000-0000-0000-0000-000000000001"), 88.50m, new Guid("12000000-0000-0000-0000-000000000002"), new Guid("13000000-0000-0000-0000-000000000001"), 0m, true, new Guid("17000000-0000-0000-0000-000000000001") },
                    { new Guid("21000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2026, 3, 30, 14, 5, 0, 0, DateTimeKind.Utc), "Walk-in swap payment collected via VNPay.", 87.50m, new Guid("16000000-0000-0000-0000-000000000004"), 96.40m, 38.00m, "GOOD", new Guid("16000000-0000-0000-0000-000000000003"), 90.10m, new Guid("12000000-0000-0000-0000-000000000003"), new Guid("13000000-0000-0000-0000-000000000002"), 80000m, false, new Guid("17000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "Feedback",
                columns: new[] { "FeedbackID", "AccountID", "BookingID", "Comment", "CreateDate", "Rating", "StationID", "TransactionID" },
                values: new object[,]
                {
                    { new Guid("26000000-0000-0000-0000-000000000001"), new Guid("12000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000001"), "Fast swap process and staff explained the battery condition clearly.", new DateTime(2026, 3, 30, 12, 0, 0, 0, DateTimeKind.Utc), 5, new Guid("13000000-0000-0000-0000-000000000001"), new Guid("21000000-0000-0000-0000-000000000001") },
                    { new Guid("26000000-0000-0000-0000-000000000002"), new Guid("12000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000002"), "Swap completed smoothly, waiting on the invoice response.", new DateTime(2026, 3, 30, 18, 0, 0, 0, DateTimeKind.Utc), 4, new Guid("13000000-0000-0000-0000-000000000002"), new Guid("21000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "Payment",
                columns: new[] { "PaymentID", "AccountID", "Amount", "BookingID", "CreateDate", "Currency", "PaidAt", "PaymentGatewayId", "PaymentMethod", "PaymentType", "RecordedByAccountID", "Status", "TransactionID", "TransactionReference", "UserSubscriptionID" },
                values: new object[] { new Guid("24000000-0000-0000-0000-000000000003"), new Guid("12000000-0000-0000-0000-000000000005"), 80000m, new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2026, 3, 30, 14, 6, 0, 0, DateTimeKind.Utc), "VND", new DateTime(2026, 3, 30, 14, 10, 0, 0, DateTimeKind.Utc), 200001L, "VNPAY", "SWAP_FEE", new Guid("12000000-0000-0000-0000-000000000003"), "PAID", new Guid("21000000-0000-0000-0000-000000000002"), "SWAP-20260330-001", null });

            migrationBuilder.InsertData(
                table: "SupportRequest",
                columns: new[] { "RequestID", "AccountID", "BookingID", "ClosedDate", "CreateDate", "Description", "IssueType", "Priority", "StationID", "Status", "Subject", "TransactionID", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("25000000-0000-0000-0000-000000000001"), new Guid("12000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 3, 30, 11, 20, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 30, 11, 0, 0, 0, DateTimeKind.Utc), "Customer asked whether the returned battery can still be reused after inspection.", "BATTERY_CHECK", "LOW", new Guid("13000000-0000-0000-0000-000000000001"), "RESOLVED", "Need update on returned battery condition", new Guid("21000000-0000-0000-0000-000000000001"), new DateTime(2026, 3, 30, 11, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("25000000-0000-0000-0000-000000000002"), new Guid("12000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000002"), null, new DateTime(2026, 3, 30, 16, 0, 0, 0, DateTimeKind.Utc), "Customer requested a payment invoice after completing the swap transaction.", "PAYMENT", "MEDIUM", new Guid("13000000-0000-0000-0000-000000000002"), "IN_PROGRESS", "Need invoice for VNPay payment", new Guid("21000000-0000-0000-0000-000000000002"), new DateTime(2026, 3, 30, 16, 15, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "SupportRequestResponse",
                columns: new[] { "ResponseID", "RequestID", "RespondedAt", "ResponseMessage", "StaffID", "StatusAfterResponse" },
                values: new object[,]
                {
                    { new Guid("25100000-0000-0000-0000-000000000001"), new Guid("25000000-0000-0000-0000-000000000001"), new DateTime(2026, 3, 30, 11, 20, 0, 0, DateTimeKind.Utc), "Returned battery passed the initial inspection and is queued for further testing.", new Guid("12000000-0000-0000-0000-000000000002"), "RESOLVED" },
                    { new Guid("25100000-0000-0000-0000-000000000002"), new Guid("25000000-0000-0000-0000-000000000002"), new DateTime(2026, 3, 30, 16, 15, 0, 0, DateTimeKind.Utc), "Invoice request received. Staff is preparing a downloadable receipt for the customer.", new Guid("12000000-0000-0000-0000-000000000003"), "IN_PROGRESS" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BatteryHistory",
                keyColumn: "HistoryID",
                keyValue: new Guid("18000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "BatteryHistory",
                keyColumn: "HistoryID",
                keyValue: new Guid("18000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "BatteryHistory",
                keyColumn: "HistoryID",
                keyValue: new Guid("18000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "BatteryReturnInspection",
                keyColumn: "InspectionID",
                keyValue: new Guid("22000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "BatteryReturnInspection",
                keyColumn: "InspectionID",
                keyValue: new Guid("22000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: new Guid("26000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: new Guid("26000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Payment",
                keyColumn: "PaymentID",
                keyValue: new Guid("24000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Payment",
                keyColumn: "PaymentID",
                keyValue: new Guid("24000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Payment",
                keyColumn: "PaymentID",
                keyValue: new Guid("24000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "RolePermissionID",
                keyValue: new Guid("11100000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "RolePermissionID",
                keyValue: new Guid("11100000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "RolePermissionID",
                keyValue: new Guid("11100000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "RolePermissionID",
                keyValue: new Guid("11100000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "RolePermissionID",
                keyValue: new Guid("11100000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "RolePermissionID",
                keyValue: new Guid("11100000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "StationBatteryType",
                keyColumn: "StationBatteryTypeID",
                keyValue: new Guid("14100000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "StationBatteryType",
                keyColumn: "StationBatteryTypeID",
                keyValue: new Guid("14100000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "StationBatteryType",
                keyColumn: "StationBatteryTypeID",
                keyValue: new Guid("14100000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "StationInventoryLog",
                keyColumn: "LogID",
                keyValue: new Guid("23000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "StationInventoryLog",
                keyColumn: "LogID",
                keyValue: new Guid("23000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "StationStaffAssignment",
                keyColumn: "AssignmentID",
                keyValue: new Guid("14200000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "StationStaffAssignment",
                keyColumn: "AssignmentID",
                keyValue: new Guid("14200000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "SupportRequestResponse",
                keyColumn: "ResponseID",
                keyValue: new Guid("25100000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SupportRequestResponse",
                keyColumn: "ResponseID",
                keyValue: new Guid("25100000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "AccountID",
                keyValue: new Guid("12000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "PermissionID",
                keyValue: new Guid("11000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "PermissionID",
                keyValue: new Guid("11000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "PermissionID",
                keyValue: new Guid("11000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "SupportRequest",
                keyColumn: "RequestID",
                keyValue: new Guid("25000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SupportRequest",
                keyColumn: "RequestID",
                keyValue: new Guid("25000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "UserSubscription",
                keyColumn: "UserSubscriptionID",
                keyValue: new Guid("19100000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "UserSubscription",
                keyColumn: "UserSubscriptionID",
                keyValue: new Guid("19100000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleID",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SubscriptionPlan",
                keyColumn: "PlanID",
                keyValue: new Guid("19000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SubscriptionPlan",
                keyColumn: "PlanID",
                keyValue: new Guid("19000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "SwappingTransaction",
                keyColumn: "TransactionID",
                keyValue: new Guid("21000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SwappingTransaction",
                keyColumn: "TransactionID",
                keyValue: new Guid("21000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Battery",
                keyColumn: "BatteryID",
                keyValue: new Guid("16000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Battery",
                keyColumn: "BatteryID",
                keyValue: new Guid("16000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Booking",
                keyColumn: "BookingID",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Booking",
                keyColumn: "BookingID",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "AccountID",
                keyValue: new Guid("12000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "AccountID",
                keyValue: new Guid("12000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Station",
                keyColumn: "StationID",
                keyValue: new Guid("13000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Station",
                keyColumn: "StationID",
                keyValue: new Guid("13000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Vehicle",
                keyColumn: "VehicleID",
                keyValue: new Guid("17000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Vehicle",
                keyColumn: "VehicleID",
                keyValue: new Guid("17000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "AccountID",
                keyValue: new Guid("12000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "AccountID",
                keyValue: new Guid("12000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Battery",
                keyColumn: "BatteryID",
                keyValue: new Guid("16000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Battery",
                keyColumn: "BatteryID",
                keyValue: new Guid("16000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleID",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "VehicleModel",
                keyColumn: "ModelID",
                keyValue: new Guid("15000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "VehicleModel",
                keyColumn: "ModelID",
                keyValue: new Guid("15000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "BatteryType",
                keyColumn: "BatteryTypeID",
                keyValue: new Guid("14000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "BatteryType",
                keyColumn: "BatteryTypeID",
                keyValue: new Guid("14000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleID",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));
        }
    }
}
