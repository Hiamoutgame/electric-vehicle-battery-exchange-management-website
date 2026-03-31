using EV_BatteryChangeStation_Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Repository.DBContext;

internal static class AppDbContextSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        static DateTime Utc(int year, int month, int day, int hour = 0, int minute = 0)
            => new(year, month, day, hour, minute, 0, DateTimeKind.Utc);

        var roleAdminId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var roleStaffId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var roleCustomerId = Guid.Parse("10000000-0000-0000-0000-000000000003");

        var permissionBookingId = Guid.Parse("11000000-0000-0000-0000-000000000001");
        var permissionStationId = Guid.Parse("11000000-0000-0000-0000-000000000002");
        var permissionPaymentId = Guid.Parse("11000000-0000-0000-0000-000000000003");

        var adminAccountId = Guid.Parse("12000000-0000-0000-0000-000000000001");
        var staffHcmAccountId = Guid.Parse("12000000-0000-0000-0000-000000000002");
        var staffHaNoiAccountId = Guid.Parse("12000000-0000-0000-0000-000000000003");
        var customerMinhAccountId = Guid.Parse("12000000-0000-0000-0000-000000000004");
        var customerLanAccountId = Guid.Parse("12000000-0000-0000-0000-000000000005");

        var stationHcmId = Guid.Parse("13000000-0000-0000-0000-000000000001");
        var stationHaNoiId = Guid.Parse("13000000-0000-0000-0000-000000000002");

        var standardBatteryTypeId = Guid.Parse("14000000-0000-0000-0000-000000000001");
        var fastChargeBatteryTypeId = Guid.Parse("14000000-0000-0000-0000-000000000002");

        var stationBatteryType1Id = Guid.Parse("14100000-0000-0000-0000-000000000001");
        var stationBatteryType2Id = Guid.Parse("14100000-0000-0000-0000-000000000002");
        var stationBatteryType3Id = Guid.Parse("14100000-0000-0000-0000-000000000003");

        var assignmentHcmId = Guid.Parse("14200000-0000-0000-0000-000000000001");
        var assignmentHaNoiId = Guid.Parse("14200000-0000-0000-0000-000000000002");

        var scooterModelId = Guid.Parse("15000000-0000-0000-0000-000000000001");
        var deliveryModelId = Guid.Parse("15000000-0000-0000-0000-000000000002");

        var batteryReturnedHcmId = Guid.Parse("16000000-0000-0000-0000-000000000001");
        var batteryInUseHcmId = Guid.Parse("16000000-0000-0000-0000-000000000002");
        var batteryReturnedHaNoiId = Guid.Parse("16000000-0000-0000-0000-000000000003");
        var batteryInUseHaNoiId = Guid.Parse("16000000-0000-0000-0000-000000000004");

        var vehicleMinhId = Guid.Parse("17000000-0000-0000-0000-000000000001");
        var vehicleLanId = Guid.Parse("17000000-0000-0000-0000-000000000002");

        var batteryHistory1Id = Guid.Parse("18000000-0000-0000-0000-000000000001");
        var batteryHistory2Id = Guid.Parse("18000000-0000-0000-0000-000000000002");
        var batteryHistory3Id = Guid.Parse("18000000-0000-0000-0000-000000000003");

        var basicPlanId = Guid.Parse("19000000-0000-0000-0000-000000000001");
        var premiumPlanId = Guid.Parse("19000000-0000-0000-0000-000000000002");

        var minhSubscriptionId = Guid.Parse("19100000-0000-0000-0000-000000000001");
        var lanSubscriptionId = Guid.Parse("19100000-0000-0000-0000-000000000002");

        var bookingMinhId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var bookingLanId = Guid.Parse("20000000-0000-0000-0000-000000000002");

        var swappingMinhId = Guid.Parse("21000000-0000-0000-0000-000000000001");
        var swappingLanId = Guid.Parse("21000000-0000-0000-0000-000000000002");

        var inspectionMinhId = Guid.Parse("22000000-0000-0000-0000-000000000001");
        var inspectionLanId = Guid.Parse("22000000-0000-0000-0000-000000000002");

        var inventoryLogHcmId = Guid.Parse("23000000-0000-0000-0000-000000000001");
        var inventoryLogHaNoiId = Guid.Parse("23000000-0000-0000-0000-000000000002");

        var paymentSubscriptionMinhId = Guid.Parse("24000000-0000-0000-0000-000000000001");
        var paymentSubscriptionLanId = Guid.Parse("24000000-0000-0000-0000-000000000002");
        var paymentSwapLanId = Guid.Parse("24000000-0000-0000-0000-000000000003");

        var supportRequestMinhId = Guid.Parse("25000000-0000-0000-0000-000000000001");
        var supportRequestLanId = Guid.Parse("25000000-0000-0000-0000-000000000002");

        var supportResponseMinhId = Guid.Parse("25100000-0000-0000-0000-000000000001");
        var supportResponseLanId = Guid.Parse("25100000-0000-0000-0000-000000000002");

        var feedbackMinhId = Guid.Parse("26000000-0000-0000-0000-000000000001");
        var feedbackLanId = Guid.Parse("26000000-0000-0000-0000-000000000002");

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = roleAdminId,
                RoleName = "ADMIN",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 5, 8, 0)
            },
            new Role
            {
                RoleId = roleStaffId,
                RoleName = "STAFF",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 5, 8, 5)
            },
            new Role
            {
                RoleId = roleCustomerId,
                RoleName = "CUSTOMER",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 5, 8, 10)
            });

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = permissionBookingId,
                PermissionCode = "BOOKING_MANAGE",
                PermissionName = "Manage booking records",
                ModuleName = "BOOKING",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 5, 8, 15)
            },
            new Permission
            {
                PermissionId = permissionStationId,
                PermissionCode = "STATION_MANAGE",
                PermissionName = "Manage station operations",
                ModuleName = "STATION",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 5, 8, 20)
            },
            new Permission
            {
                PermissionId = permissionPaymentId,
                PermissionCode = "PAYMENT_MANAGE",
                PermissionName = "Manage payment records",
                ModuleName = "PAYMENT",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 5, 8, 25)
            });

        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission
            {
                RolePermissionId = Guid.Parse("11100000-0000-0000-0000-000000000001"),
                RoleId = roleAdminId,
                PermissionId = permissionBookingId,
                CreateDate = Utc(2026, 1, 5, 8, 30)
            },
            new RolePermission
            {
                RolePermissionId = Guid.Parse("11100000-0000-0000-0000-000000000002"),
                RoleId = roleAdminId,
                PermissionId = permissionStationId,
                CreateDate = Utc(2026, 1, 5, 8, 31)
            },
            new RolePermission
            {
                RolePermissionId = Guid.Parse("11100000-0000-0000-0000-000000000003"),
                RoleId = roleAdminId,
                PermissionId = permissionPaymentId,
                CreateDate = Utc(2026, 1, 5, 8, 32)
            },
            new RolePermission
            {
                RolePermissionId = Guid.Parse("11100000-0000-0000-0000-000000000004"),
                RoleId = roleStaffId,
                PermissionId = permissionBookingId,
                CreateDate = Utc(2026, 1, 5, 8, 33)
            },
            new RolePermission
            {
                RolePermissionId = Guid.Parse("11100000-0000-0000-0000-000000000005"),
                RoleId = roleStaffId,
                PermissionId = permissionStationId,
                CreateDate = Utc(2026, 1, 5, 8, 34)
            },
            new RolePermission
            {
                RolePermissionId = Guid.Parse("11100000-0000-0000-0000-000000000006"),
                RoleId = roleCustomerId,
                PermissionId = permissionBookingId,
                CreateDate = Utc(2026, 1, 5, 8, 35)
            });

        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                AccountId = adminAccountId,
                Username = "admin.seed",
                PasswordHash = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==",
                FullName = "Seed Admin",
                Email = "admin.seed@evswap.local",
                PhoneNumber = "0900000001",
                Gender = "MALE",
                Address = "District 1, Ho Chi Minh City",
                DateOfBirth = new DateOnly(1990, 1, 10),
                RoleId = roleAdminId,
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 6, 8, 0),
                LastLoginAt = Utc(2026, 3, 30, 18, 0)
            },
            new Account
            {
                AccountId = staffHcmAccountId,
                Username = "staff.hcm",
                PasswordHash = "AAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQ==",
                FullName = "Nguyen Van Huy",
                Email = "staff.hcm@evswap.local",
                PhoneNumber = "0900000002",
                Gender = "MALE",
                Address = "Thu Duc, Ho Chi Minh City",
                DateOfBirth = new DateOnly(1996, 4, 12),
                RoleId = roleStaffId,
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 6, 8, 15),
                LastLoginAt = Utc(2026, 3, 30, 12, 30)
            },
            new Account
            {
                AccountId = staffHaNoiAccountId,
                Username = "staff.hanoi",
                PasswordHash = "AAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQ==",
                FullName = "Tran Thu Trang",
                Email = "staff.hanoi@evswap.local",
                PhoneNumber = "0900000003",
                Gender = "FEMALE",
                Address = "Cau Giay, Ha Noi",
                DateOfBirth = new DateOnly(1997, 9, 20),
                RoleId = roleStaffId,
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 6, 8, 30),
                LastLoginAt = Utc(2026, 3, 29, 17, 45)
            },
            new Account
            {
                AccountId = customerMinhAccountId,
                Username = "pham.minh",
                PasswordHash = "AAICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAg==",
                FullName = "Pham Quoc Minh",
                Email = "pham.minh@evswap.local",
                PhoneNumber = "0900000004",
                Gender = "MALE",
                Address = "Binh Thanh, Ho Chi Minh City",
                DateOfBirth = new DateOnly(2000, 5, 18),
                RoleId = roleCustomerId,
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 6, 9, 0),
                LastLoginAt = Utc(2026, 3, 31, 7, 20)
            },
            new Account
            {
                AccountId = customerLanAccountId,
                Username = "le.lan",
                PasswordHash = "AAICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAg==",
                FullName = "Le Thu Lan",
                Email = "le.lan@evswap.local",
                PhoneNumber = "0900000005",
                Gender = "FEMALE",
                Address = "Dong Da, Ha Noi",
                DateOfBirth = new DateOnly(1999, 11, 3),
                RoleId = roleCustomerId,
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 6, 9, 15),
                LastLoginAt = Utc(2026, 3, 31, 8, 10)
            });

        modelBuilder.Entity<Station>().HasData(
            new Station
            {
                StationId = stationHcmId,
                StationName = "EV Swap Thu Duc",
                Address = "12 Vo Van Ngan, Thu Duc, Ho Chi Minh City",
                Area = "Ho Chi Minh City",
                PhoneNumber = "02873001001",
                Latitude = 10.850632m,
                Longitude = 106.771876m,
                OperatingHours = "06:00-22:00",
                Status = "ACTIVE",
                MaxCapacity = 20,
                CurrentBatteryCount = 1,
                CreateDate = Utc(2026, 1, 7, 7, 0)
            },
            new Station
            {
                StationId = stationHaNoiId,
                StationName = "EV Swap Cau Giay",
                Address = "88 Tran Thai Tong, Cau Giay, Ha Noi",
                Area = "Ha Noi",
                PhoneNumber = "02473001002",
                Latitude = 21.036812m,
                Longitude = 105.790310m,
                OperatingHours = "06:00-22:00",
                Status = "ACTIVE",
                MaxCapacity = 18,
                CurrentBatteryCount = 1,
                CreateDate = Utc(2026, 1, 7, 7, 15)
            });

        modelBuilder.Entity<BatteryType>().HasData(
            new BatteryType
            {
                BatteryTypeId = standardBatteryTypeId,
                BatteryTypeCode = "BT-STD-72V",
                BatteryTypeName = "72V Standard Pack",
                Voltage = 72m,
                CapacityKwh = 3.50m,
                Description = "Standard battery pack for city scooters.",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 7, 7, 30)
            },
            new BatteryType
            {
                BatteryTypeId = fastChargeBatteryTypeId,
                BatteryTypeCode = "BT-FAST-84V",
                BatteryTypeName = "84V FastCharge Pack",
                Voltage = 84m,
                CapacityKwh = 4.20m,
                Description = "High-capacity battery pack for delivery vehicles.",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 7, 7, 35)
            });

        modelBuilder.Entity<StationBatteryType>().HasData(
            new StationBatteryType
            {
                StationBatteryTypeId = stationBatteryType1Id,
                StationId = stationHcmId,
                BatteryTypeId = standardBatteryTypeId,
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 7, 7, 45)
            },
            new StationBatteryType
            {
                StationBatteryTypeId = stationBatteryType2Id,
                StationId = stationHcmId,
                BatteryTypeId = fastChargeBatteryTypeId,
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 7, 7, 46)
            },
            new StationBatteryType
            {
                StationBatteryTypeId = stationBatteryType3Id,
                StationId = stationHaNoiId,
                BatteryTypeId = fastChargeBatteryTypeId,
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 7, 7, 47)
            });

        modelBuilder.Entity<StationStaffAssignment>().HasData(
            new StationStaffAssignment
            {
                AssignmentId = assignmentHcmId,
                StaffId = staffHcmAccountId,
                StationId = stationHcmId,
                EffectiveFrom = new DateOnly(2026, 1, 8),
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 8, 8, 0)
            },
            new StationStaffAssignment
            {
                AssignmentId = assignmentHaNoiId,
                StaffId = staffHaNoiAccountId,
                StationId = stationHaNoiId,
                EffectiveFrom = new DateOnly(2026, 1, 8),
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 8, 8, 10)
            });

        modelBuilder.Entity<VehicleModel>().HasData(
            new VehicleModel
            {
                ModelId = scooterModelId,
                ModelName = "E-Scoot S1",
                Producer = "VinFast",
                BatteryTypeId = standardBatteryTypeId,
                Description = "Compact electric scooter for daily commuting.",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 9, 9, 0)
            },
            new VehicleModel
            {
                ModelId = deliveryModelId,
                ModelName = "E-Delivery X",
                Producer = "Selex",
                BatteryTypeId = fastChargeBatteryTypeId,
                Description = "Utility model for delivery fleets.",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 9, 9, 10)
            });

        modelBuilder.Entity<Battery>().HasData(
            new Battery
            {
                BatteryId = batteryReturnedHcmId,
                SerialNumber = "BAT-HCM-0001",
                BatteryTypeId = standardBatteryTypeId,
                CapacityKwh = 3.50m,
                StateOfHealth = 88.50m,
                CurrentChargeLevel = 24.00m,
                Status = "INSPECTING",
                StationId = stationHcmId,
                InsuranceDate = new DateOnly(2025, 6, 1),
                LastChargedAt = Utc(2026, 3, 28, 17, 0),
                LastUsedAt = Utc(2026, 3, 30, 9, 5),
                CreateDate = Utc(2026, 1, 10, 8, 0)
            },
            new Battery
            {
                BatteryId = batteryInUseHcmId,
                SerialNumber = "BAT-HCM-0002",
                BatteryTypeId = standardBatteryTypeId,
                CapacityKwh = 3.50m,
                StateOfHealth = 95.20m,
                CurrentChargeLevel = 91.00m,
                Status = "IN_USE",
                StationId = null,
                InsuranceDate = new DateOnly(2025, 7, 15),
                LastChargedAt = Utc(2026, 3, 30, 8, 30),
                LastUsedAt = Utc(2026, 3, 31, 6, 45),
                CreateDate = Utc(2026, 1, 10, 8, 15)
            },
            new Battery
            {
                BatteryId = batteryReturnedHaNoiId,
                SerialNumber = "BAT-HNI-0001",
                BatteryTypeId = fastChargeBatteryTypeId,
                CapacityKwh = 4.20m,
                StateOfHealth = 90.10m,
                CurrentChargeLevel = 38.00m,
                Status = "CHARGING",
                StationId = stationHaNoiId,
                InsuranceDate = new DateOnly(2025, 5, 20),
                LastChargedAt = Utc(2026, 3, 29, 22, 0),
                LastUsedAt = Utc(2026, 3, 30, 14, 0),
                CreateDate = Utc(2026, 1, 10, 8, 30)
            },
            new Battery
            {
                BatteryId = batteryInUseHaNoiId,
                SerialNumber = "BAT-HNI-0002",
                BatteryTypeId = fastChargeBatteryTypeId,
                CapacityKwh = 4.20m,
                StateOfHealth = 96.40m,
                CurrentChargeLevel = 87.50m,
                Status = "IN_USE",
                StationId = null,
                InsuranceDate = new DateOnly(2025, 8, 1),
                LastChargedAt = Utc(2026, 3, 30, 13, 20),
                LastUsedAt = Utc(2026, 3, 31, 7, 40),
                CreateDate = Utc(2026, 1, 10, 8, 45)
            });

        modelBuilder.Entity<Vehicle>().HasData(
            new Vehicle
            {
                VehicleId = vehicleMinhId,
                Vin = "VIN-HCM-0000000001",
                LicensePlate = "59A3-12345",
                ModelId = scooterModelId,
                OwnerId = customerMinhAccountId,
                CurrentBatteryId = batteryInUseHcmId,
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 11, 9, 0)
            },
            new Vehicle
            {
                VehicleId = vehicleLanId,
                Vin = "VIN-HNI-0000000002",
                LicensePlate = "29B1-67890",
                ModelId = deliveryModelId,
                OwnerId = customerLanAccountId,
                CurrentBatteryId = batteryInUseHaNoiId,
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 11, 9, 15)
            });

        modelBuilder.Entity<BatteryHistory>().HasData(
            new BatteryHistory
            {
                HistoryId = batteryHistory1Id,
                BatteryId = batteryReturnedHcmId,
                FromVehicleId = vehicleMinhId,
                ToStationId = stationHcmId,
                ActionType = "RETURNED",
                FromStatus = "IN_USE",
                ToStatus = "INSPECTING",
                EventDate = Utc(2026, 3, 30, 9, 10),
                SoHAtTime = 88.50m,
                ChargeLevelAtTime = 24.00m,
                Note = "Battery returned during completed swap booking.",
                ActorAccountId = staffHcmAccountId
            },
            new BatteryHistory
            {
                HistoryId = batteryHistory2Id,
                BatteryId = batteryInUseHcmId,
                FromStationId = stationHcmId,
                ToVehicleId = vehicleMinhId,
                ActionType = "RELEASED",
                FromStatus = "AVAILABLE",
                ToStatus = "IN_USE",
                EventDate = Utc(2026, 3, 30, 9, 12),
                SoHAtTime = 95.20m,
                ChargeLevelAtTime = 91.00m,
                Note = "Released to customer after booking approval.",
                ActorAccountId = staffHcmAccountId
            },
            new BatteryHistory
            {
                HistoryId = batteryHistory3Id,
                BatteryId = batteryInUseHaNoiId,
                FromStationId = stationHaNoiId,
                ToVehicleId = vehicleLanId,
                ActionType = "RELEASED",
                FromStatus = "AVAILABLE",
                ToStatus = "IN_USE",
                EventDate = Utc(2026, 3, 30, 14, 5),
                SoHAtTime = 96.40m,
                ChargeLevelAtTime = 87.50m,
                Note = "Released to vehicle after successful inspection.",
                ActorAccountId = staffHaNoiAccountId
            });

        modelBuilder.Entity<SubscriptionPlan>().HasData(
            new SubscriptionPlan
            {
                PlanId = basicPlanId,
                PlanCode = "PLAN-BASIC",
                PlanName = "Basic Monthly",
                BasePrice = 199000m,
                Currency = "VND",
                SwapLimitPerMonth = 4,
                DurationDays = 30,
                ExtraFeePerSwap = 25000m,
                Description = "Entry package for casual EV users.",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 12, 10, 0)
            },
            new SubscriptionPlan
            {
                PlanId = premiumPlanId,
                PlanCode = "PLAN-PREMIUM",
                PlanName = "Premium Monthly",
                BasePrice = 349000m,
                Currency = "VND",
                SwapLimitPerMonth = 10,
                DurationDays = 30,
                ExtraFeePerSwap = 10000m,
                Description = "Best for heavy riders and delivery use.",
                Status = "ACTIVE",
                CreateDate = Utc(2026, 1, 12, 10, 15)
            });

        modelBuilder.Entity<UserSubscription>().HasData(
            new UserSubscription
            {
                UserSubscriptionId = minhSubscriptionId,
                AccountId = customerMinhAccountId,
                VehicleId = vehicleMinhId,
                PlanId = basicPlanId,
                StartDate = Utc(2026, 3, 1, 0, 0),
                EndDate = Utc(2026, 3, 31, 23, 59),
                RemainingSwaps = 2,
                Status = "ACTIVE",
                AutoRenew = true,
                CreateDate = Utc(2026, 3, 1, 7, 0)
            },
            new UserSubscription
            {
                UserSubscriptionId = lanSubscriptionId,
                AccountId = customerLanAccountId,
                VehicleId = vehicleLanId,
                PlanId = premiumPlanId,
                StartDate = Utc(2026, 3, 5, 0, 0),
                EndDate = Utc(2026, 4, 4, 23, 59),
                RemainingSwaps = 8,
                Status = "ACTIVE",
                AutoRenew = true,
                CreateDate = Utc(2026, 3, 5, 8, 0)
            });

        modelBuilder.Entity<Booking>().HasData(
            new Booking
            {
                BookingId = bookingMinhId,
                AccountId = customerMinhAccountId,
                VehicleId = vehicleMinhId,
                StationId = stationHcmId,
                RequestedBatteryTypeId = standardBatteryTypeId,
                TargetTime = Utc(2026, 3, 30, 9, 0),
                Status = "COMPLETED",
                Notes = "Customer requested morning battery swap.",
                StaffNote = "Completed on time.",
                ApprovedBy = staffHcmAccountId,
                ApprovedDate = Utc(2026, 3, 30, 8, 40),
                CreateDate = Utc(2026, 3, 29, 20, 0),
                UpdateDate = Utc(2026, 3, 30, 9, 15)
            },
            new Booking
            {
                BookingId = bookingLanId,
                AccountId = customerLanAccountId,
                VehicleId = vehicleLanId,
                StationId = stationHaNoiId,
                RequestedBatteryTypeId = fastChargeBatteryTypeId,
                TargetTime = Utc(2026, 3, 30, 14, 0),
                Status = "COMPLETED",
                Notes = "Afternoon swap for delivery route.",
                StaffNote = "Battery health checked before release.",
                ApprovedBy = staffHaNoiAccountId,
                ApprovedDate = Utc(2026, 3, 30, 13, 30),
                CreateDate = Utc(2026, 3, 29, 21, 0),
                UpdateDate = Utc(2026, 3, 30, 14, 20)
            });

        modelBuilder.Entity<SwappingTransaction>().HasData(
            new SwappingTransaction
            {
                TransactionId = swappingMinhId,
                BookingId = bookingMinhId,
                VehicleId = vehicleMinhId,
                StaffId = staffHcmAccountId,
                StationId = stationHcmId,
                ReturnedBatteryId = batteryReturnedHcmId,
                ReturnedBatterySoH = 88.50m,
                ReturnedBatteryCharge = 24.00m,
                ReturnedBatteryCondition = "GOOD",
                ReleasedBatteryId = batteryInUseHcmId,
                ReleasedBatterySoH = 95.20m,
                ReleasedBatteryCharge = 91.00m,
                SwapFee = 0m,
                UsedSubscription = true,
                Notes = "Covered by active subscription plan.",
                CreateDate = Utc(2026, 3, 30, 9, 12)
            },
            new SwappingTransaction
            {
                TransactionId = swappingLanId,
                BookingId = bookingLanId,
                VehicleId = vehicleLanId,
                StaffId = staffHaNoiAccountId,
                StationId = stationHaNoiId,
                ReturnedBatteryId = batteryReturnedHaNoiId,
                ReturnedBatterySoH = 90.10m,
                ReturnedBatteryCharge = 38.00m,
                ReturnedBatteryCondition = "GOOD",
                ReleasedBatteryId = batteryInUseHaNoiId,
                ReleasedBatterySoH = 96.40m,
                ReleasedBatteryCharge = 87.50m,
                SwapFee = 80000m,
                UsedSubscription = false,
                Notes = "Walk-in swap payment collected via VNPay.",
                CreateDate = Utc(2026, 3, 30, 14, 5)
            });

        modelBuilder.Entity<BatteryReturnInspection>().HasData(
            new BatteryReturnInspection
            {
                InspectionId = inspectionMinhId,
                BookingId = bookingMinhId,
                BatteryId = batteryReturnedHcmId,
                StationId = stationHcmId,
                StaffId = staffHcmAccountId,
                SoHPercent = 88.50m,
                PhysicalCondition = "GOOD",
                InspectionNote = "No exterior damage detected.",
                NextStatus = "INSPECTING",
                CreateDate = Utc(2026, 3, 30, 9, 8)
            },
            new BatteryReturnInspection
            {
                InspectionId = inspectionLanId,
                BookingId = bookingLanId,
                BatteryId = batteryReturnedHaNoiId,
                StationId = stationHaNoiId,
                StaffId = staffHaNoiAccountId,
                SoHPercent = 90.10m,
                PhysicalCondition = "GOOD",
                InspectionNote = "Battery moved to charging after inspection.",
                NextStatus = "CHARGING",
                CreateDate = Utc(2026, 3, 30, 14, 2)
            });

        modelBuilder.Entity<StationInventoryLog>().HasData(
            new StationInventoryLog
            {
                LogId = inventoryLogHcmId,
                StationId = stationHcmId,
                BatteryTypeId = standardBatteryTypeId,
                LogTime = Utc(2026, 3, 30, 18, 0),
                AvailableBatteries = 0,
                ReservedBatteries = 0,
                ChargingBatteries = 0,
                InVehicleBatteries = 1,
                MaintenanceBatteries = 1,
                AvgChargeLevel = 57.50m
            },
            new StationInventoryLog
            {
                LogId = inventoryLogHaNoiId,
                StationId = stationHaNoiId,
                BatteryTypeId = fastChargeBatteryTypeId,
                LogTime = Utc(2026, 3, 30, 18, 0),
                AvailableBatteries = 0,
                ReservedBatteries = 0,
                ChargingBatteries = 1,
                InVehicleBatteries = 1,
                MaintenanceBatteries = 0,
                AvgChargeLevel = 62.75m
            });

        modelBuilder.Entity<Payment>().HasData(
            new Payment
            {
                PaymentId = paymentSubscriptionMinhId,
                AccountId = customerMinhAccountId,
                UserSubscriptionId = minhSubscriptionId,
                Amount = 199000m,
                Currency = "VND",
                PaymentType = "SUBSCRIPTION",
                PaymentMethod = "VNPAY",
                Status = "PAID",
                TransactionReference = "SUB-20260301-001",
                PaymentGatewayId = 100001,
                RecordedByAccountId = adminAccountId,
                PaidAt = Utc(2026, 3, 1, 7, 5),
                CreateDate = Utc(2026, 3, 1, 7, 0)
            },
            new Payment
            {
                PaymentId = paymentSubscriptionLanId,
                AccountId = customerLanAccountId,
                UserSubscriptionId = lanSubscriptionId,
                Amount = 349000m,
                Currency = "VND",
                PaymentType = "SUBSCRIPTION",
                PaymentMethod = "VNPAY",
                Status = "PAID",
                TransactionReference = "SUB-20260305-001",
                PaymentGatewayId = 100002,
                RecordedByAccountId = adminAccountId,
                PaidAt = Utc(2026, 3, 5, 8, 5),
                CreateDate = Utc(2026, 3, 5, 8, 0)
            },
            new Payment
            {
                PaymentId = paymentSwapLanId,
                AccountId = customerLanAccountId,
                BookingId = bookingLanId,
                TransactionId = swappingLanId,
                Amount = 80000m,
                Currency = "VND",
                PaymentType = "SWAP_FEE",
                PaymentMethod = "VNPAY",
                Status = "PAID",
                TransactionReference = "SWAP-20260330-001",
                PaymentGatewayId = 200001,
                RecordedByAccountId = staffHaNoiAccountId,
                PaidAt = Utc(2026, 3, 30, 14, 10),
                CreateDate = Utc(2026, 3, 30, 14, 6)
            });

        modelBuilder.Entity<SupportRequest>().HasData(
            new SupportRequest
            {
                RequestId = supportRequestMinhId,
                AccountId = customerMinhAccountId,
                StationId = stationHcmId,
                BookingId = bookingMinhId,
                TransactionId = swappingMinhId,
                IssueType = "BATTERY_CHECK",
                Subject = "Need update on returned battery condition",
                Description = "Customer asked whether the returned battery can still be reused after inspection.",
                Priority = "LOW",
                Status = "RESOLVED",
                CreateDate = Utc(2026, 3, 30, 11, 0),
                UpdateDate = Utc(2026, 3, 30, 11, 20),
                ClosedDate = Utc(2026, 3, 30, 11, 20)
            },
            new SupportRequest
            {
                RequestId = supportRequestLanId,
                AccountId = customerLanAccountId,
                StationId = stationHaNoiId,
                BookingId = bookingLanId,
                TransactionId = swappingLanId,
                IssueType = "PAYMENT",
                Subject = "Need invoice for VNPay payment",
                Description = "Customer requested a payment invoice after completing the swap transaction.",
                Priority = "MEDIUM",
                Status = "IN_PROGRESS",
                CreateDate = Utc(2026, 3, 30, 16, 0),
                UpdateDate = Utc(2026, 3, 30, 16, 15)
            });

        modelBuilder.Entity<SupportRequestResponse>().HasData(
            new SupportRequestResponse
            {
                ResponseId = supportResponseMinhId,
                RequestId = supportRequestMinhId,
                StaffId = staffHcmAccountId,
                ResponseMessage = "Returned battery passed the initial inspection and is queued for further testing.",
                StatusAfterResponse = "RESOLVED",
                RespondedAt = Utc(2026, 3, 30, 11, 20)
            },
            new SupportRequestResponse
            {
                ResponseId = supportResponseLanId,
                RequestId = supportRequestLanId,
                StaffId = staffHaNoiAccountId,
                ResponseMessage = "Invoice request received. Staff is preparing a downloadable receipt for the customer.",
                StatusAfterResponse = "IN_PROGRESS",
                RespondedAt = Utc(2026, 3, 30, 16, 15)
            });

        modelBuilder.Entity<Feedback>().HasData(
            new Feedback
            {
                FeedbackId = feedbackMinhId,
                AccountId = customerMinhAccountId,
                StationId = stationHcmId,
                BookingId = bookingMinhId,
                TransactionId = swappingMinhId,
                Rating = 5,
                Comment = "Fast swap process and staff explained the battery condition clearly.",
                CreateDate = Utc(2026, 3, 30, 12, 0)
            },
            new Feedback
            {
                FeedbackId = feedbackLanId,
                AccountId = customerLanAccountId,
                StationId = stationHaNoiId,
                BookingId = bookingLanId,
                TransactionId = swappingLanId,
                Rating = 4,
                Comment = "Swap completed smoothly, waiting on the invoice response.",
                CreateDate = Utc(2026, 3, 30, 18, 0)
            });
    }
}
