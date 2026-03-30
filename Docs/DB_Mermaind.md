# EVBatterySwap Mermaid ERD

Source SQL schema: `../SWP_EVBatteryChangeStation_BE/Database/EVBatterySwap.sql`

```mermaid
erDiagram
    ROLE {
        UNIQUEIDENTIFIER RoleID PK
        NVARCHAR RoleName
        BIT Status
        DATETIME CreateDate
    }

    STATION {
        UNIQUEIDENTIFIER StationID PK
        NVARCHAR StationName
        NVARCHAR Address
        NVARCHAR PhoneNumber
        NVARCHAR Status
        INT MaxCapacity
        INT CurrentBatteryCount
    }

    ACCOUNT {
        UNIQUEIDENTIFIER AccountID PK
        NVARCHAR Username
        NVARCHAR PasswordHash
        NVARCHAR FullName
        NVARCHAR Email
        NVARCHAR PhoneNumber
        UNIQUEIDENTIFIER RoleID FK
        UNIQUEIDENTIFIER StationID FK
        BIT Status
        DATETIME CreateDate
    }

    VEHICLE_MODEL {
        UNIQUEIDENTIFIER ModelID PK
        NVARCHAR ModelName
        NVARCHAR Producer
        NVARCHAR CompatibleBatteryType
    }

    VEHICLE {
        UNIQUEIDENTIFIER VehicleID PK
        NVARCHAR LicensePlate
        UNIQUEIDENTIFIER ModelID FK
        UNIQUEIDENTIFIER OwnerID FK
        UNIQUEIDENTIFIER CurrentBatteryID
        NVARCHAR Status
    }

    BATTERY {
        UNIQUEIDENTIFIER BatteryID PK
        NVARCHAR SerialNumber
        NVARCHAR TypeBattery
        DECIMAL Capacity_kWh
        DECIMAL StateOfHealth
        DECIMAL CurrentChargeLevel
        NVARCHAR Status
        UNIQUEIDENTIFIER StationID FK
        DATE InsuranceDate
        DATETIME CreateDate
    }

    BATTERY_HISTORY {
        UNIQUEIDENTIFIER HistoryID PK
        UNIQUEIDENTIFIER BatteryID FK
        UNIQUEIDENTIFIER FromStationID FK
        UNIQUEIDENTIFIER ToStationID FK
        NVARCHAR ActionType
        DATETIME EventDate
        DECIMAL SoH_AtTime
        NVARCHAR Note
    }

    SUBSCRIPTION_PLAN {
        UNIQUEIDENTIFIER PlanID PK
        NVARCHAR PlanName
        DECIMAL BasePrice
        INT SwapLimitPerMonth
        NVARCHAR Description
        BIT IsActive
    }

    USER_SUBSCRIPTION {
        UNIQUEIDENTIFIER UserSubID PK
        UNIQUEIDENTIFIER AccountID FK
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
        DATETIME TargetTime
        NVARCHAR Status
        NVARCHAR Notes
    }

    SWAPPING_TRANSACTION {
        UNIQUEIDENTIFIER TransactionID PK
        UNIQUEIDENTIFIER BookingID FK
        UNIQUEIDENTIFIER VehicleID FK
        UNIQUEIDENTIFIER StaffID FK
        UNIQUEIDENTIFIER StationID FK
        UNIQUEIDENTIFIER ReturnedBatteryID FK
        DECIMAL ReturnedBatterySoH
        DECIMAL ReturnedBatteryCharge
        UNIQUEIDENTIFIER ReleasedBatteryID FK
        DECIMAL ReleasedBatterySoH
        DECIMAL SwapFee
        DATETIME CreateDate
    }

    STATION_INVENTORY_LOG {
        UNIQUEIDENTIFIER LogID PK
        UNIQUEIDENTIFIER StationID FK
        DATETIME LogTime
        INT AvailableBatteries
        INT ChargingBatteries
        INT MaintenanceBatteries
        DECIMAL AvgChargeLevel
    }

    PAYMENT {
        UNIQUEIDENTIFIER PaymentID PK
        UNIQUEIDENTIFIER AccountID FK
        DECIMAL Amount
        NVARCHAR PaymentType
        NVARCHAR PaymentMethod
        NVARCHAR Status
        NVARCHAR TransactionReference
        DATETIME CreateDate
    }

    SUPPORT_REQUEST {
        UNIQUEIDENTIFIER RequestID PK
        UNIQUEIDENTIFIER AccountID FK
        NVARCHAR IssueType
        NVARCHAR Description
        NVARCHAR Status
        DATETIME CreateDate
    }

    FEEDBACK {
        UNIQUEIDENTIFIER FeedbackID PK
        UNIQUEIDENTIFIER AccountID FK
        UNIQUEIDENTIFIER TransactionID FK
        INT Rating
        NVARCHAR Comment
        DATETIME CreateDate
    }

    ROLE ||--o{ ACCOUNT : grants
    STATION ||--o{ ACCOUNT : assigns

    VEHICLE_MODEL ||--o{ VEHICLE : defines
    ACCOUNT ||--o{ VEHICLE : owns
    BATTERY o|--o| VEHICLE : current_battery_logical

    STATION ||--o{ BATTERY : stores
    BATTERY ||--o{ BATTERY_HISTORY : tracks
    STATION ||--o{ BATTERY_HISTORY : from_station
    STATION ||--o{ BATTERY_HISTORY : to_station

    ACCOUNT ||--o{ USER_SUBSCRIPTION : has
    SUBSCRIPTION_PLAN ||--o{ USER_SUBSCRIPTION : selected

    ACCOUNT ||--o{ BOOKING : creates
    VEHICLE ||--o{ BOOKING : books_for
    STATION ||--o{ BOOKING : targets

    BOOKING ||--o{ SWAPPING_TRANSACTION : produces
    VEHICLE ||--o{ SWAPPING_TRANSACTION : swaps_for
    ACCOUNT ||--o{ SWAPPING_TRANSACTION : handled_by
    STATION ||--o{ SWAPPING_TRANSACTION : occurs_at
    BATTERY ||--o{ SWAPPING_TRANSACTION : returned_battery
    BATTERY ||--o{ SWAPPING_TRANSACTION : released_battery

    STATION ||--o{ STATION_INVENTORY_LOG : snapshots

    ACCOUNT ||--o{ PAYMENT : pays
    ACCOUNT ||--o{ SUPPORT_REQUEST : raises
    ACCOUNT ||--o{ FEEDBACK : writes
    SWAPPING_TRANSACTION ||--o{ FEEDBACK : receives
```

## Notes

- `Vehicle.CurrentBatteryID` is shown as a logical relation for the ERD. In the current SQL, it is not declared as a foreign key.
- `BatteryHistory` references `Station` twice to represent move origin and move destination.
- `SwappingTransaction` references `Battery` twice to separate returned battery and released battery.
- Trigger logic such as `trg_Battery_LocationChange` is not represented in Mermaid ERD and should stay in SQL documentation.
