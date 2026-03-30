# EVBatterySwap v2 Mermaid ERD

Source SQL schema: `../SWP_EVBatteryChangeStation_BE/Database/EVBatterySwap_v2.sql`

This file is split by module so it is easier to preview and zoom.

## 1. Access Control

```mermaid
erDiagram
    ROLE {
        UNIQUEIDENTIFIER RoleID PK
        NVARCHAR RoleName
        NVARCHAR Status
    }

    PERMISSION {
        UNIQUEIDENTIFIER PermissionID PK
        NVARCHAR PermissionCode
        NVARCHAR PermissionName
        NVARCHAR ModuleName
        NVARCHAR Status
    }

    ROLE_PERMISSION {
        UNIQUEIDENTIFIER RolePermissionID PK
        UNIQUEIDENTIFIER RoleID FK
        UNIQUEIDENTIFIER PermissionID FK
    }

    ACCOUNT {
        UNIQUEIDENTIFIER AccountID PK
        NVARCHAR Username
        NVARCHAR PasswordHash
        NVARCHAR FullName
        NVARCHAR Email
        NVARCHAR PhoneNumber
        UNIQUEIDENTIFIER RoleID FK
        NVARCHAR Status
    }

    STATION {
        UNIQUEIDENTIFIER StationID PK
        NVARCHAR StationName
        NVARCHAR Status
    }

    STATION_STAFF_ASSIGNMENT {
        UNIQUEIDENTIFIER AssignmentID PK
        UNIQUEIDENTIFIER StaffID FK
        UNIQUEIDENTIFIER StationID FK
        DATE EffectiveFrom
        DATE EffectiveTo
        NVARCHAR Status
    }

    ROLE ||--o{ ACCOUNT : grants
    ROLE ||--o{ ROLE_PERMISSION : owns
    PERMISSION ||--o{ ROLE_PERMISSION : maps
    ACCOUNT ||--o{ STATION_STAFF_ASSIGNMENT : assigned
    STATION ||--o{ STATION_STAFF_ASSIGNMENT : hosts
```

## 2. Station, Battery, Inventory

```mermaid
erDiagram
    STATION {
        UNIQUEIDENTIFIER StationID PK
        NVARCHAR StationName
        NVARCHAR Address
        NVARCHAR Area
        NVARCHAR PhoneNumber
        DECIMAL Latitude
        DECIMAL Longitude
        NVARCHAR OperatingHours
        NVARCHAR Status
        INT MaxCapacity
        INT CurrentBatteryCount
    }

    BATTERY_TYPE {
        UNIQUEIDENTIFIER BatteryTypeID PK
        NVARCHAR BatteryTypeCode
        NVARCHAR BatteryTypeName
        DECIMAL Voltage
        DECIMAL Capacity_kWh
        NVARCHAR Status
    }

    STATION_BATTERY_TYPE {
        UNIQUEIDENTIFIER StationBatteryTypeID PK
        UNIQUEIDENTIFIER StationID FK
        UNIQUEIDENTIFIER BatteryTypeID FK
        NVARCHAR Status
    }

    BATTERY {
        UNIQUEIDENTIFIER BatteryID PK
        NVARCHAR SerialNumber
        UNIQUEIDENTIFIER BatteryTypeID FK
        DECIMAL Capacity_kWh
        DECIMAL StateOfHealth
        DECIMAL CurrentChargeLevel
        NVARCHAR Status
        UNIQUEIDENTIFIER StationID FK
    }

    VEHICLE {
        UNIQUEIDENTIFIER VehicleID PK
        NVARCHAR LicensePlate
        UNIQUEIDENTIFIER CurrentBatteryID FK
        NVARCHAR Status
    }

    ACCOUNT {
        UNIQUEIDENTIFIER AccountID PK
        NVARCHAR Username
    }

    BATTERY_HISTORY {
        UNIQUEIDENTIFIER HistoryID PK
        UNIQUEIDENTIFIER BatteryID FK
        UNIQUEIDENTIFIER FromStationID FK
        UNIQUEIDENTIFIER ToStationID FK
        UNIQUEIDENTIFIER FromVehicleID FK
        UNIQUEIDENTIFIER ToVehicleID FK
        UNIQUEIDENTIFIER ActorAccountID FK
        NVARCHAR ActionType
        NVARCHAR FromStatus
        NVARCHAR ToStatus
    }

    BATTERY_RETURN_INSPECTION {
        UNIQUEIDENTIFIER InspectionID PK
        UNIQUEIDENTIFIER BatteryID FK
        UNIQUEIDENTIFIER StationID FK
        UNIQUEIDENTIFIER StaffID FK
        DECIMAL SoHPercent
        NVARCHAR PhysicalCondition
        NVARCHAR NextStatus
    }

    STATION_INVENTORY_LOG {
        UNIQUEIDENTIFIER LogID PK
        UNIQUEIDENTIFIER StationID FK
        UNIQUEIDENTIFIER BatteryTypeID FK
        DATETIME LogTime
    }

    STATION ||--o{ STATION_BATTERY_TYPE : supports
    BATTERY_TYPE ||--o{ STATION_BATTERY_TYPE : enabled_for
    BATTERY_TYPE ||--o{ BATTERY : classifies
    STATION ||--o{ BATTERY : stores
    BATTERY o|--o| VEHICLE : current_battery
    BATTERY ||--o{ BATTERY_HISTORY : tracked_by
    STATION ||--o{ BATTERY_HISTORY : from_station
    STATION ||--o{ BATTERY_HISTORY : to_station
    VEHICLE ||--o{ BATTERY_HISTORY : from_vehicle
    VEHICLE ||--o{ BATTERY_HISTORY : to_vehicle
    ACCOUNT ||--o{ BATTERY_HISTORY : acted_by
    BATTERY ||--o{ BATTERY_RETURN_INSPECTION : inspected_battery
    STATION ||--o{ BATTERY_RETURN_INSPECTION : inspected_at
    ACCOUNT ||--o{ BATTERY_RETURN_INSPECTION : inspected_by
    STATION ||--o{ STATION_INVENTORY_LOG : snapshots
    BATTERY_TYPE ||--o{ STATION_INVENTORY_LOG : by_type
```

## 3. Vehicle, Subscription, Booking, Swap, Payment

```mermaid
erDiagram
    ACCOUNT {
        UNIQUEIDENTIFIER AccountID PK
        NVARCHAR Username
        NVARCHAR FullName
        NVARCHAR Email
    }

    STATION {
        UNIQUEIDENTIFIER StationID PK
        NVARCHAR StationName
        NVARCHAR Status
    }

    BATTERY_TYPE {
        UNIQUEIDENTIFIER BatteryTypeID PK
        NVARCHAR BatteryTypeCode
        NVARCHAR BatteryTypeName
    }

    VEHICLE_MODEL {
        UNIQUEIDENTIFIER ModelID PK
        NVARCHAR ModelName
        NVARCHAR Producer
        UNIQUEIDENTIFIER BatteryTypeID FK
    }

    VEHICLE {
        UNIQUEIDENTIFIER VehicleID PK
        NVARCHAR VIN
        NVARCHAR LicensePlate
        UNIQUEIDENTIFIER ModelID FK
        UNIQUEIDENTIFIER OwnerID FK
        UNIQUEIDENTIFIER CurrentBatteryID FK
        NVARCHAR Status
    }

    BATTERY {
        UNIQUEIDENTIFIER BatteryID PK
        NVARCHAR SerialNumber
        UNIQUEIDENTIFIER BatteryTypeID FK
        NVARCHAR Status
    }

    SUBSCRIPTION_PLAN {
        UNIQUEIDENTIFIER PlanID PK
        NVARCHAR PlanCode
        NVARCHAR PlanName
        DECIMAL BasePrice
        INT SwapLimitPerMonth
        INT DurationDays
        DECIMAL ExtraFeePerSwap
        NVARCHAR Status
    }

    USER_SUBSCRIPTION {
        UNIQUEIDENTIFIER UserSubscriptionID PK
        UNIQUEIDENTIFIER AccountID FK
        UNIQUEIDENTIFIER VehicleID FK
        UNIQUEIDENTIFIER PlanID FK
        DATETIME StartDate
        DATETIME EndDate
        INT RemainingSwaps
        NVARCHAR Status
    }

    BOOKING {
        UNIQUEIDENTIFIER BookingID PK
        UNIQUEIDENTIFIER AccountID FK
        UNIQUEIDENTIFIER VehicleID FK
        UNIQUEIDENTIFIER StationID FK
        UNIQUEIDENTIFIER RequestedBatteryTypeID FK
        UNIQUEIDENTIFIER ApprovedBy FK
        DATETIME TargetTime
        NVARCHAR Status
    }

    SWAPPING_TRANSACTION {
        UNIQUEIDENTIFIER TransactionID PK
        UNIQUEIDENTIFIER BookingID FK
        UNIQUEIDENTIFIER VehicleID FK
        UNIQUEIDENTIFIER StaffID FK
        UNIQUEIDENTIFIER StationID FK
        UNIQUEIDENTIFIER ReturnedBatteryID FK
        UNIQUEIDENTIFIER ReleasedBatteryID FK
        DECIMAL SwapFee
        BIT UsedSubscription
    }

    PAYMENT {
        UNIQUEIDENTIFIER PaymentID PK
        UNIQUEIDENTIFIER AccountID FK
        UNIQUEIDENTIFIER BookingID FK
        UNIQUEIDENTIFIER UserSubscriptionID FK
        UNIQUEIDENTIFIER TransactionID FK
        UNIQUEIDENTIFIER RecordedByAccountID FK
        DECIMAL Amount
        NVARCHAR PaymentType
        NVARCHAR PaymentMethod
        NVARCHAR Status
    }

    BATTERY_TYPE ||--o{ VEHICLE_MODEL : powers
    VEHICLE_MODEL ||--o{ VEHICLE : defines
    ACCOUNT ||--o{ VEHICLE : owns
    ACCOUNT ||--o{ USER_SUBSCRIPTION : has
    VEHICLE ||--o{ USER_SUBSCRIPTION : covered_vehicle
    SUBSCRIPTION_PLAN ||--o{ USER_SUBSCRIPTION : chosen_plan
    ACCOUNT ||--o{ BOOKING : creates
    VEHICLE ||--o{ BOOKING : for_vehicle
    STATION ||--o{ BOOKING : targets
    BATTERY_TYPE ||--o{ BOOKING : requests
    ACCOUNT ||--o{ BOOKING : approves
    BOOKING ||--|| SWAPPING_TRANSACTION : results_in
    VEHICLE ||--o{ SWAPPING_TRANSACTION : swaps_for
    ACCOUNT ||--o{ SWAPPING_TRANSACTION : handled_by
    STATION ||--o{ SWAPPING_TRANSACTION : happens_at
    BATTERY ||--o{ SWAPPING_TRANSACTION : returned_battery
    BATTERY ||--o{ SWAPPING_TRANSACTION : released_battery
    ACCOUNT ||--o{ PAYMENT : pays
    BOOKING ||--o{ PAYMENT : charges
    USER_SUBSCRIPTION ||--o{ PAYMENT : subscription_payment
    SWAPPING_TRANSACTION ||--o{ PAYMENT : swap_payment
    ACCOUNT ||--o{ PAYMENT : recorded_by
```

## 4. Support, Response, Feedback

```mermaid
erDiagram
    ACCOUNT {
        UNIQUEIDENTIFIER AccountID PK
        NVARCHAR Username
        NVARCHAR FullName
        NVARCHAR Email
    }

    STATION {
        UNIQUEIDENTIFIER StationID PK
        NVARCHAR StationName
    }

    BOOKING {
        UNIQUEIDENTIFIER BookingID PK
        UNIQUEIDENTIFIER AccountID FK
        UNIQUEIDENTIFIER StationID FK
        NVARCHAR Status
    }

    SWAPPING_TRANSACTION {
        UNIQUEIDENTIFIER TransactionID PK
        UNIQUEIDENTIFIER BookingID FK
        UNIQUEIDENTIFIER StationID FK
    }

    SUPPORT_REQUEST {
        UNIQUEIDENTIFIER RequestID PK
        UNIQUEIDENTIFIER AccountID FK
        UNIQUEIDENTIFIER StationID FK
        UNIQUEIDENTIFIER BookingID FK
        UNIQUEIDENTIFIER TransactionID FK
        NVARCHAR IssueType
        NVARCHAR Subject
        NVARCHAR Priority
        NVARCHAR Status
    }

    SUPPORT_REQUEST_RESPONSE {
        UNIQUEIDENTIFIER ResponseID PK
        UNIQUEIDENTIFIER RequestID FK
        UNIQUEIDENTIFIER StaffID FK
        NVARCHAR StatusAfterResponse
    }

    FEEDBACK {
        UNIQUEIDENTIFIER FeedbackID PK
        UNIQUEIDENTIFIER AccountID FK
        UNIQUEIDENTIFIER StationID FK
        UNIQUEIDENTIFIER BookingID FK
        UNIQUEIDENTIFIER TransactionID FK
        INT Rating
    }

    ACCOUNT ||--o{ SUPPORT_REQUEST : raises
    STATION ||--o{ SUPPORT_REQUEST : related_station
    BOOKING ||--o{ SUPPORT_REQUEST : related_booking
    SWAPPING_TRANSACTION ||--o{ SUPPORT_REQUEST : related_swap
    SUPPORT_REQUEST ||--o{ SUPPORT_REQUEST_RESPONSE : has_responses
    ACCOUNT ||--o{ SUPPORT_REQUEST_RESPONSE : responded_by
    ACCOUNT ||--o{ FEEDBACK : writes
    STATION ||--o{ FEEDBACK : receives
    BOOKING ||--o{ FEEDBACK : from_booking
    SWAPPING_TRANSACTION ||--o{ FEEDBACK : from_swap
```

## Notes

- This ERD reflects the target schema `EVBatterySwap_v2.sql`, not the legacy `EVBatterySwap.sql`.
- `StationStaffAssignment` replaces a hard-coded station link on `Account`.
- `SupportRequestResponse` is separated so response history stays auditable.
- If this is still crowded, split each section into standalone files such as `DB_Station.md`, `DB_Swap.md`, and `DB_Support.md`.
