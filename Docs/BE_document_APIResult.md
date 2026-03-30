# Backend API Draft By Role

## Purpose

This document proposes the REST APIs needed for each role based on:

- `Docs/BE_document_UserStory.md`
- `Docs/image.png`

The goal is to give the team a backend-ready draft that can later be mapped to:

- controller routes
- request DTOs
- response DTOs
- validation rules
- RBAC rules

## Scope Assumptions

- Base path: `/api/v1`
- Auth method: `Bearer JWT`
- Main roles: `DRIVER`, `ADMIN`, `STAFF`
- Station staff can only access data of their assigned station
- Station delete should be implemented as soft delete or status change
- Booking, swap, payment, and support entities all have audit fields

## Common Response Format

Successful response:

```json
{
  "success": true,
  "code": "RESOURCE_FETCHED",
  "message": "Request processed successfully",
  "data": {}
}
```

Validation or business error:

```json
{
  "success": false,
  "code": "BUSINESS_RULE_VIOLATION",
  "message": "Request could not be processed",
  "errors": [
    {
      "field": "fieldName",
      "message": "Detailed error message"
    }
  ]
}
```

## Common Auth APIs

### 1. POST `/api/v1/auth/register`

Used by: Driver

Request body:

```json
{
  "fullName": "Nguyen Van A",
  "email": "driver01@example.com",
  "phone": "0901234567",
  "password": "StrongPass@123"
}
```

Response `201`:

```json
{
  "success": true,
  "code": "ACCOUNT_CREATED",
  "message": "Account registered successfully",
  "data": {
    "userId": "usr_001",
    "role": "DRIVER",
    "status": "ACTIVE"
  }
}
```

### 2. POST `/api/v1/auth/login`

Used by: Driver, Admin, Staff

Request body:

```json
{
  "email": "driver01@example.com",
  "password": "StrongPass@123"
}
```

Response `200`:

```json
{
  "success": true,
  "code": "LOGIN_SUCCESS",
  "message": "Login successful",
  "data": {
    "accessToken": "jwt-access-token",
    "refreshToken": "jwt-refresh-token",
    "user": {
      "userId": "usr_001",
      "fullName": "Nguyen Van A",
      "role": "DRIVER"
    }
  }
}
```

### 3. GET `/api/v1/auth/me`

Used by: Driver, Admin, Staff

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "PROFILE_FETCHED",
  "message": "Current user fetched successfully",
  "data": {
    "userId": "usr_001",
    "fullName": "Nguyen Van A",
    "email": "driver01@example.com",
    "phone": "0901234567",
    "role": "DRIVER",
    "assignedStationId": null
  }
}
```

## DRIVER APIs

### 1. POST `/api/v1/driver/vehicles`

User story mapping: image context - link vehicle and battery type

Request body:

```json
{
  "vin": "RLF12345678900001",
  "licensePlate": "59A3-12345",
  "vehicleModel": "VinFast Evo",
  "batteryTypeId": "bat_type_lfp_standard"
}
```

Response `201`:

```json
{
  "success": true,
  "code": "VEHICLE_LINKED",
  "message": "Vehicle linked successfully",
  "data": {
    "vehicleId": "veh_001",
    "vin": "RLF12345678900001",
    "batteryTypeId": "bat_type_lfp_standard",
    "ownerId": "usr_001"
  }
}
```

### 2. GET `/api/v1/driver/vehicles`

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "VEHICLE_LIST_FETCHED",
  "message": "Vehicles fetched successfully",
  "data": [
    {
      "vehicleId": "veh_001",
      "vin": "RLF12345678900001",
      "licensePlate": "59A3-12345",
      "vehicleModel": "VinFast Evo",
      "batteryTypeId": "bat_type_lfp_standard"
    }
  ]
}
```

### 3. GET `/api/v1/stations`

User story mapping: US-DRV-01, US-DRV-02, US-DRV-03, US-DRV-05

Query params:

- `keyword` optional
- `area` optional
- `batteryTypeId` optional
- `lat` optional
- `lng` optional
- `sort` optional: `nearest`, `name`
- `status` default: `ACTIVE`
- `page`, `size`

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "STATION_LIST_FETCHED",
  "message": "Stations fetched successfully",
  "data": {
    "items": [
      {
        "stationId": "st_001",
        "name": "District 7 Swap Hub",
        "address": "123 Nguyen Van Linh, District 7, HCMC",
        "phone": "02873001234",
        "status": "ACTIVE",
        "supportedBatteryTypes": [
          "bat_type_lfp_standard"
        ],
        "availableBatteryCount": 18,
        "latitude": 10.729,
        "longitude": 106.721,
        "distanceKm": 2.4
      }
    ],
    "page": 1,
    "size": 10,
    "totalItems": 1
  }
}
```

### 4. GET `/api/v1/stations/{stationId}`

User story mapping: US-DRV-04

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "STATION_DETAIL_FETCHED",
  "message": "Station detail fetched successfully",
  "data": {
    "stationId": "st_001",
    "name": "District 7 Swap Hub",
    "address": "123 Nguyen Van Linh, District 7, HCMC",
    "phone": "02873001234",
    "status": "ACTIVE",
    "operatingHours": "06:00-22:00",
    "supportedBatteryTypes": [
      {
        "batteryTypeId": "bat_type_lfp_standard",
        "name": "LFP Standard"
      }
    ],
    "inventorySummary": {
      "available": 18,
      "inUse": 7,
      "maintenance": 2
    }
  }
}
```

### 5. POST `/api/v1/driver/bookings`

User story mapping: US-DRV-06

Request body:

```json
{
  "stationId": "st_001",
  "vehicleId": "veh_001",
  "batteryTypeId": "bat_type_lfp_standard",
  "bookingTime": "2026-03-30T10:30:00+07:00",
  "note": "Need quick swap before work"
}
```

Response `201`:

```json
{
  "success": true,
  "code": "BOOKING_CREATED",
  "message": "Booking created successfully",
  "data": {
    "bookingId": "bk_001",
    "status": "PENDING",
    "stationId": "st_001",
    "vehicleId": "veh_001",
    "batteryTypeId": "bat_type_lfp_standard",
    "bookingTime": "2026-03-30T10:30:00+07:00",
    "subscriptionUsageReserved": 1
  }
}
```

Business error example `409`:

```json
{
  "success": false,
  "code": "BOOKING_SUBSCRIPTION_INVALID",
  "message": "Active subscription not found or no remaining swaps",
  "errors": []
}
```

### 6. GET `/api/v1/driver/bookings`

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "BOOKING_LIST_FETCHED",
  "message": "Bookings fetched successfully",
  "data": [
    {
      "bookingId": "bk_001",
      "stationName": "District 7 Swap Hub",
      "bookingTime": "2026-03-30T10:30:00+07:00",
      "status": "APPROVED",
      "batteryTypeId": "bat_type_lfp_standard"
    }
  ]
}
```

### 7. GET `/api/v1/driver/bookings/{bookingId}`

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "BOOKING_DETAIL_FETCHED",
  "message": "Booking detail fetched successfully",
  "data": {
    "bookingId": "bk_001",
    "stationId": "st_001",
    "stationName": "District 7 Swap Hub",
    "bookingTime": "2026-03-30T10:30:00+07:00",
    "status": "APPROVED",
    "staffNote": "Please arrive 10 minutes early",
    "swapTransactionId": null
  }
}
```

### 8. POST `/api/v1/driver/subscriptions/purchase`

User story mapping: image context - register battery swap package

Request body:

```json
{
  "planId": "plan_monthly_30",
  "paymentMethod": "MOMO",
  "vehicleId": "veh_001"
}
```

Response `201`:

```json
{
  "success": true,
  "code": "SUBSCRIPTION_PURCHASED",
  "message": "Subscription purchased successfully",
  "data": {
    "subscriptionId": "sub_001",
    "planId": "plan_monthly_30",
    "status": "ACTIVE",
    "remainingSwaps": 30,
    "startDate": "2026-03-30",
    "endDate": "2026-04-29",
    "paymentId": "pay_001"
  }
}
```

### 9. GET `/api/v1/driver/subscriptions/current`

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "SUBSCRIPTION_FETCHED",
  "message": "Current subscription fetched successfully",
  "data": {
    "subscriptionId": "sub_001",
    "planName": "Monthly 30 Swaps",
    "status": "ACTIVE",
    "remainingSwaps": 27,
    "usedSwaps": 3,
    "startDate": "2026-03-30",
    "endDate": "2026-04-29"
  }
}
```

### 10. GET `/api/v1/driver/payments`

User story mapping: image context - invoices and payment history

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "PAYMENT_HISTORY_FETCHED",
  "message": "Payment history fetched successfully",
  "data": [
    {
      "paymentId": "pay_001",
      "type": "SUBSCRIPTION_PURCHASE",
      "amount": 1200000,
      "currency": "VND",
      "status": "PAID",
      "paidAt": "2026-03-30T09:00:00+07:00",
      "referenceCode": "INV-20260330-001"
    }
  ]
}
```

### 11. GET `/api/v1/driver/swaps/history`

User story mapping: image context - track number of swaps and cost

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "SWAP_HISTORY_FETCHED",
  "message": "Swap history fetched successfully",
  "data": [
    {
      "swapTransactionId": "sw_001",
      "bookingId": "bk_001",
      "stationName": "District 7 Swap Hub",
      "completedAt": "2026-03-30T10:42:00+07:00",
      "feeAmount": 50000,
      "usedSubscription": true
    }
  ]
}
```

### 12. POST `/api/v1/driver/support-requests`

User story mapping: image context - support and feedback

Request body:

```json
{
  "stationId": "st_001",
  "category": "BATTERY_ISSUE",
  "subject": "Returned battery discharged too fast",
  "description": "After swap, battery level dropped unusually within 5 km."
}
```

Response `201`:

```json
{
  "success": true,
  "code": "SUPPORT_REQUEST_CREATED",
  "message": "Support request created successfully",
  "data": {
    "supportRequestId": "sup_001",
    "status": "OPEN",
    "createdAt": "2026-03-30T11:00:00+07:00"
  }
}
```

### 13. GET `/api/v1/driver/support-requests`

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "SUPPORT_REQUEST_LIST_FETCHED",
  "message": "Support requests fetched successfully",
  "data": [
    {
      "supportRequestId": "sup_001",
      "category": "BATTERY_ISSUE",
      "subject": "Returned battery discharged too fast",
      "status": "IN_PROGRESS",
      "latestResponseAt": "2026-03-30T13:00:00+07:00"
    }
  ]
}
```

### 14. POST `/api/v1/driver/feedback`

User story mapping: image context - station service review

Request body:

```json
{
  "stationId": "st_001",
  "rating": 4,
  "comment": "Fast service and clear instructions"
}
```

Response `201`:

```json
{
  "success": true,
  "code": "FEEDBACK_CREATED",
  "message": "Feedback submitted successfully",
  "data": {
    "feedbackId": "fb_001",
    "stationId": "st_001",
    "rating": 4
  }
}
```

## ADMIN APIs

### 1. GET `/api/v1/admin/stations`

User story mapping: US-ADM-01

Query params:

- `status` optional: `ACTIVE`, `INACTIVE`, `ALL`
- `keyword` optional
- `page`, `size`

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "ADMIN_STATION_LIST_FETCHED",
  "message": "Admin station list fetched successfully",
  "data": {
    "items": [
      {
        "stationId": "st_001",
        "name": "District 7 Swap Hub",
        "status": "ACTIVE",
        "address": "123 Nguyen Van Linh, District 7, HCMC",
        "batterySummary": {
          "available": 18,
          "inUse": 7,
          "maintenance": 2
        },
        "assignedStaffCount": 3
      }
    ]
  }
}
```

### 2. POST `/api/v1/admin/stations`

User story mapping: US-ADM-04

Request body:

```json
{
  "name": "Thu Duc Swap Hub",
  "address": "10 Vo Van Ngan, Thu Duc, HCMC",
  "phone": "02873005678",
  "status": "ACTIVE",
  "latitude": 10.8507,
  "longitude": 106.7718,
  "supportedBatteryTypeIds": [
    "bat_type_lfp_standard"
  ],
  "operatingHours": "06:00-22:00"
}
```

Response `201`:

```json
{
  "success": true,
  "code": "STATION_CREATED",
  "message": "Station created successfully",
  "data": {
    "stationId": "st_010",
    "name": "Thu Duc Swap Hub",
    "status": "ACTIVE"
  }
}
```

### 3. PATCH `/api/v1/admin/stations/{stationId}`

User story mapping: US-ADM-04

Request body:

```json
{
  "phone": "02873009999",
  "status": "INACTIVE",
  "operatingHours": "07:00-21:00"
}
```

Response `200`:

```json
{
  "success": true,
  "code": "STATION_UPDATED",
  "message": "Station updated successfully",
  "data": {
    "stationId": "st_010",
    "phone": "02873009999",
    "status": "INACTIVE",
    "operatingHours": "07:00-21:00"
  }
}
```

### 4. DELETE `/api/v1/admin/stations/{stationId}`

User story mapping: US-ADM-04

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "STATION_DEACTIVATED",
  "message": "Station deactivated successfully",
  "data": {
    "stationId": "st_010",
    "status": "INACTIVE"
  }
}
```

### 5. GET `/api/v1/admin/inventory/summary`

User story mapping: US-ADM-02

Query params:

- `stationId` optional
- `batteryTypeId` optional

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "INVENTORY_SUMMARY_FETCHED",
  "message": "Inventory summary fetched successfully",
  "data": [
    {
      "stationId": "st_001",
      "stationName": "District 7 Swap Hub",
      "available": 18,
      "inUse": 7,
      "maintenance": 2,
      "charging": 5
    }
  ]
}
```

### 6. POST `/api/v1/admin/staff-assignments`

User story mapping: US-ADM-03

Request body:

```json
{
  "staffUserId": "usr_staff_001",
  "stationId": "st_001",
  "effectiveFrom": "2026-03-30"
}
```

Response `201`:

```json
{
  "success": true,
  "code": "STAFF_ASSIGNED",
  "message": "Staff assigned successfully",
  "data": {
    "assignmentId": "asg_001",
    "staffUserId": "usr_staff_001",
    "stationId": "st_001",
    "effectiveFrom": "2026-03-30"
  }
}
```

Business error example `409`:

```json
{
  "success": false,
  "code": "STAFF_ALREADY_ASSIGNED",
  "message": "Staff is already assigned to another station",
  "errors": []
}
```

### 7. GET `/api/v1/admin/users`

User story mapping: image context - manage customers and staff

Query params:

- `role` optional
- `status` optional
- `keyword` optional

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "USER_LIST_FETCHED",
  "message": "Users fetched successfully",
  "data": [
    {
      "userId": "usr_001",
      "fullName": "Nguyen Van A",
      "email": "driver01@example.com",
      "role": "DRIVER",
      "status": "ACTIVE"
    }
  ]
}
```

### 8. POST `/api/v1/admin/subscription-plans`

User story mapping: image context - create battery subscription package

Request body:

```json
{
  "name": "Monthly 30 Swaps",
  "description": "30 swaps in 30 days",
  "price": 1200000,
  "currency": "VND",
  "maxSwaps": 30,
  "durationDays": 30,
  "status": "ACTIVE"
}
```

Response `201`:

```json
{
  "success": true,
  "code": "SUBSCRIPTION_PLAN_CREATED",
  "message": "Subscription plan created successfully",
  "data": {
    "planId": "plan_monthly_30",
    "name": "Monthly 30 Swaps",
    "status": "ACTIVE"
  }
}
```

### 9. PATCH `/api/v1/admin/subscription-plans/{planId}`

Request body:

```json
{
  "price": 1350000,
  "status": "ACTIVE"
}
```

Response `200`:

```json
{
  "success": true,
  "code": "SUBSCRIPTION_PLAN_UPDATED",
  "message": "Subscription plan updated successfully",
  "data": {
    "planId": "plan_monthly_30",
    "price": 1350000,
    "status": "ACTIVE"
  }
}
```

### 10. GET `/api/v1/admin/support-requests`

User story mapping: image context - complaint handling

Query params:

- `status` optional
- `stationId` optional
- `category` optional

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "SUPPORT_REQUEST_ADMIN_LIST_FETCHED",
  "message": "Support requests fetched successfully",
  "data": [
    {
      "supportRequestId": "sup_001",
      "stationId": "st_001",
      "customerId": "usr_001",
      "category": "BATTERY_ISSUE",
      "status": "IN_PROGRESS"
    }
  ]
}
```

### 11. GET `/api/v1/admin/reports/revenue`

User story mapping: image context - revenue report

Query params:

- `fromDate`
- `toDate`
- `groupBy` optional: `day`, `month`, `station`

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "REVENUE_REPORT_FETCHED",
  "message": "Revenue report fetched successfully",
  "data": {
    "totalRevenue": 56000000,
    "currency": "VND",
    "items": [
      {
        "label": "2026-03-30",
        "revenue": 2800000
      }
    ]
  }
}
```

### 12. GET `/api/v1/admin/reports/station-demand`

User story mapping: image context - swap frequency and peak hours

Query params:

- `fromDate`
- `toDate`
- `stationId` optional

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "STATION_DEMAND_REPORT_FETCHED",
  "message": "Station demand report fetched successfully",
  "data": {
    "peakHours": [
      {
        "hour": "08:00",
        "bookingCount": 42
      }
    ],
    "swapCountByStation": [
      {
        "stationId": "st_001",
        "stationName": "District 7 Swap Hub",
        "swapCount": 214
      }
    ]
  }
}
```

### 13. GET `/api/v1/admin/reports/demand-forecast`

User story mapping: image context - AI suggestion for infrastructure upgrade

Query params:

- `stationId` optional
- `forecastDays` optional

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "DEMAND_FORECAST_FETCHED",
  "message": "Demand forecast fetched successfully",
  "data": [
    {
      "stationId": "st_001",
      "predictedDailySwaps": 95,
      "capacityRiskLevel": "HIGH",
      "recommendation": "Increase available battery stock by 15 units"
    }
  ]
}
```

## STAFF APIs

### 1. GET `/api/v1/staff/station-context`

User story mapping: US-STF-06

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "STATION_CONTEXT_FETCHED",
  "message": "Assigned station fetched successfully",
  "data": {
    "staffUserId": "usr_staff_001",
    "stationId": "st_001",
    "stationName": "District 7 Swap Hub"
  }
}
```

Error example `403`:

```json
{
  "success": false,
  "code": "STAFF_STATION_NOT_ASSIGNED",
  "message": "Staff account is not assigned to any station",
  "errors": []
}
```

### 2. GET `/api/v1/staff/bookings`

User story mapping: US-STF-01

Query params:

- `status` optional
- `bookingDate` optional

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "STAFF_BOOKING_LIST_FETCHED",
  "message": "Station bookings fetched successfully",
  "data": [
    {
      "bookingId": "bk_001",
      "customerName": "Nguyen Van A",
      "vehicleId": "veh_001",
      "batteryTypeId": "bat_type_lfp_standard",
      "bookingTime": "2026-03-30T10:30:00+07:00",
      "status": "PENDING"
    }
  ]
}
```

### 3. PATCH `/api/v1/staff/bookings/{bookingId}/decision`

User story mapping: US-STF-03

Request body:

```json
{
  "decision": "APPROVE",
  "staffNote": "Battery reserved. Please come on time."
}
```

Response `200`:

```json
{
  "success": true,
  "code": "BOOKING_STATUS_UPDATED",
  "message": "Booking processed successfully",
  "data": {
    "bookingId": "bk_001",
    "status": "APPROVED",
    "processedBy": "usr_staff_001",
    "processedAt": "2026-03-30T09:45:00+07:00"
  }
}
```

### 4. GET `/api/v1/staff/inventory`

User story mapping: US-STF-02

Query params:

- `status` optional
- `batteryTypeId` optional

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "STAFF_INVENTORY_FETCHED",
  "message": "Station inventory fetched successfully",
  "data": [
    {
      "batteryId": "bat_001",
      "serialNumber": "BAT-LFP-0001",
      "batteryTypeId": "bat_type_lfp_standard",
      "status": "AVAILABLE",
      "sohPercent": 92,
      "lastChargedAt": "2026-03-30T08:10:00+07:00"
    }
  ]
}
```

### 5. PATCH `/api/v1/staff/batteries/{batteryId}/status`

User story mapping: image context - battery classification and condition update

Request body:

```json
{
  "status": "MAINTENANCE",
  "reason": "Physical connector issue"
}
```

Response `200`:

```json
{
  "success": true,
  "code": "BATTERY_STATUS_UPDATED",
  "message": "Battery status updated successfully",
  "data": {
    "batteryId": "bat_001",
    "status": "MAINTENANCE",
    "updatedBy": "usr_staff_001"
  }
}
```

### 6. POST `/api/v1/staff/swaps/complete`

User story mapping: US-STF-04

Request body:

```json
{
  "bookingId": "bk_001",
  "issuedBatteryId": "bat_050",
  "returnedBatteryId": "bat_021",
  "returnedBatteryCondition": "NORMAL",
  "extraFeeAmount": 50000,
  "note": "Swap completed normally"
}
```

Response `201`:

```json
{
  "success": true,
  "code": "SWAP_COMPLETED",
  "message": "Swap completed successfully",
  "data": {
    "swapTransactionId": "sw_001",
    "bookingId": "bk_001",
    "issuedBatteryId": "bat_050",
    "returnedBatteryId": "bat_021",
    "bookingStatus": "COMPLETED",
    "inventoryUpdated": true
  }
}
```

Business error example `409`:

```json
{
  "success": false,
  "code": "BATTERY_NOT_AVAILABLE",
  "message": "Selected battery is no longer available",
  "errors": []
}
```

### 7. POST `/api/v1/staff/battery-return-inspections`

User story mapping: image context - inspect returned battery condition

Request body:

```json
{
  "batteryId": "bat_021",
  "bookingId": "bk_001",
  "sohPercent": 84,
  "physicalCondition": "NORMAL",
  "inspectionNote": "No visible damage"
}
```

Response `201`:

```json
{
  "success": true,
  "code": "BATTERY_RETURN_INSPECTED",
  "message": "Returned battery inspected successfully",
  "data": {
    "inspectionId": "insp_001",
    "batteryId": "bat_021",
    "nextStatus": "CHARGING"
  }
}
```

### 8. POST `/api/v1/staff/payments/record`

User story mapping: image context - record additional swap fee at station

Request body:

```json
{
  "bookingId": "bk_001",
  "swapTransactionId": "sw_001",
  "amount": 50000,
  "paymentMethod": "CASH",
  "note": "Out-of-package swap surcharge"
}
```

Response `201`:

```json
{
  "success": true,
  "code": "PAYMENT_RECORDED",
  "message": "Payment recorded successfully",
  "data": {
    "paymentId": "pay_090",
    "status": "PAID",
    "recordedBy": "usr_staff_001"
  }
}
```

### 9. GET `/api/v1/staff/support-requests`

User story mapping: US-STF-05

Query params:

- `status` optional
- `category` optional

Request body: none

Response `200`:

```json
{
  "success": true,
  "code": "STAFF_SUPPORT_REQUEST_LIST_FETCHED",
  "message": "Support requests fetched successfully",
  "data": [
    {
      "supportRequestId": "sup_001",
      "customerName": "Nguyen Van A",
      "category": "BATTERY_ISSUE",
      "subject": "Returned battery discharged too fast",
      "status": "OPEN"
    }
  ]
}
```

### 10. PATCH `/api/v1/staff/support-requests/{supportRequestId}/response`

User story mapping: US-STF-05

Request body:

```json
{
  "status": "IN_PROGRESS",
  "responseMessage": "We are checking the returned battery log and will update shortly."
}
```

Response `200`:

```json
{
  "success": true,
  "code": "SUPPORT_REQUEST_RESPONDED",
  "message": "Support request updated successfully",
  "data": {
    "supportRequestId": "sup_001",
    "status": "IN_PROGRESS",
    "respondedBy": "usr_staff_001",
    "respondedAt": "2026-03-30T13:00:00+07:00"
  }
}
```

## RBAC Notes

- `DRIVER` can only access own vehicles, subscriptions, bookings, payments, swap history, support requests, and feedback
- `STAFF` can only access records of their assigned station
- `ADMIN` can access all stations and all summaries, but action permission should still be split by module if the project supports fine-grained permission
- `GET /api/v1/stations` and `GET /api/v1/stations/{stationId}` should be public or semi-public depending on business decision

## Suggested Next Step

After this document, the next useful deliverables are:

1. API permission matrix by endpoint and role
2. entity to DTO mapping list
3. validation rules for each request body
4. status enum list for booking, battery, station, payment, and support request
