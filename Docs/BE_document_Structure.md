# Backend Structure Document

## Purpose

This document refactors the backend structure description so it aligns with:

- `Docs/BE_document_UserStory.md`
- `Docs/BE_document_APIResult.md`

The target is not only to describe folders, but also to show:

- which layer owns which responsibility
- which module serves which role
- where `Request`, `Response`, `Service`, `Repository`, and `Entity` should live
- how the codebase should grow without mixing business logic across modules
- how the backend should be organized for `PostgreSQL` instead of `SQL Server`

## Alignment Rules

When implementing backend code, the structure should follow these rules:

1. `UserStory` defines business scope
2. `APIResult` defines endpoint contract
3. `Structure` defines where code for that contract must live

That means:

- if an API exists in `BE_document_APIResult.md`, it should map to a clear controller and service flow
- if a role has a bounded scope in `BE_document_UserStory.md`, the backend module should keep that scope isolated
- if a station-scoped rule exists for `STAFF`, the validation must be handled in service layer and repository query layer

## Database Platform Direction

The backend data platform should target `PostgreSQL`, not `SQL Server`.

Recommended stack:

- database engine: `PostgreSQL`
- EF Core provider: `Npgsql.EntityFrameworkCore.PostgreSQL`
- migration tool: `EF Core Migrations`

This affects both code structure and schema conventions:

- prefer `uuid` for identifiers instead of SQL Server `uniqueidentifier`
- prefer `timestamp with time zone` for audit and event times
- prefer `text` for long free-text fields instead of `nvarchar(max)`
- prefer `numeric(p,s)` for money and battery metrics
- use PostgreSQL indexes, constraints, and partial indexes where needed
- avoid SQL Server-specific defaults and functions such as `GETDATE()` or `NEWSEQUENTIALID()`

Recommended PostgreSQL mapping examples:

```text
C# Guid                  -> PostgreSQL uuid
C# string (short text)   -> PostgreSQL varchar
C# string (long text)    -> PostgreSQL text
C# decimal               -> PostgreSQL numeric
C# DateTimeOffset        -> PostgreSQL timestamp with time zone
C# bool                  -> PostgreSQL boolean
```

## Current Solution Projects

Backend solution root:

`SWP_EVBatteryChangeStation_BE`

Main projects:

- `EV_BatteryChangeStation`
- `EV_BatteryChangeStation_Common`
- `EV_BatteryChangeStation_Repository`
- `EV_BatteryChangeStation_Service`

These project names should be kept. The refactor should focus on internal organization only.

## Recommended Responsibility By Project

### 1. `EV_BatteryChangeStation`

This is the API host project.

Current important folders:

- `Controllers`
- `Properties`
- `Program.cs`

Recommended responsibility:

- expose REST endpoints
- receive HTTP request
- validate auth header and role policy
- map request DTO to service call
- return standardized response format

Should contain:

- `Controllers/`
- `Extensions/`
- `Middlewares/`
- `Configurations/`
- `Program.cs`

Recommended detail:

- `Controllers` only orchestrate request in and response out
- no business logic in controller
- no EF query in controller
- no cross-module validation logic in controller

Suggested sub-structure:

```text
EV_BatteryChangeStation
|-- Controllers
|   |-- AuthController.cs
|   |-- StationsController.cs
|   |-- DriverController.cs
|   |-- AdminController.cs
|   |-- StaffController.cs
|-- Extensions
|   |-- ServiceCollectionExtensions.cs
|   |-- SwaggerExtensions.cs
|   |-- AuthenticationExtensions.cs
|-- Middlewares
|   |-- GlobalExceptionMiddleware.cs
|   |-- ValidationExceptionMiddleware.cs
|   |-- RequestLoggingMiddleware.cs
|-- Configurations
|   |-- SwaggerConfiguration.cs
|   |-- JwtConfiguration.cs
|-- Program.cs
```

### 2. `EV_BatteryChangeStation_Common`

This is the shared contract and utility project.

Current important folders:

- `DTOs`
- `Enum`
- `Helper`

Recommended responsibility:

- shared DTOs used across modules
- enums and constant values
- response wrapper models
- helper utilities that are safe to reuse

Should contain:

- `DTOs/Common`
- `DTOs/Auth`
- `DTOs/Stations`
- `DTOs/Bookings`
- `DTOs/Inventory`
- `DTOs/Support`
- `DTOs/Payments`
- `DTOs/Reports`
- `Enums`
- `Constants`
- `Helpers`

Recommended detail:

- only truly shared DTOs should stay here
- module-specific request/response DTOs can also stay in service project if the team prefers module ownership
- avoid putting business logic in helpers

### 3. `EV_BatteryChangeStation_Repository`

This is the data access project.

Current important folders:

- `Base`
- `DBContext`
- `Entities`
- `IRepositories`
- `Mapper`
- `Repositories`
- `UnitOfWork`

Recommended responsibility:

- entity definitions
- EF Core mapping and db context
- repository query abstraction
- transaction and unit of work
- low-level data filtering

Should contain:

- `Base/`
- `DBContext/`
- `Entities/`
- `Configurations/`
- `IRepositories/`
- `Repositories/`
- `UnitOfWork/`

Recommended detail:

- `Entities` are database models, not request DTOs
- `Repositories` handle data access only
- business rule like "staff can only view assigned station" should be enforced by service logic and query constraint together
- soft delete, audit fields, and common entity fields should be defined in `Base`
- PostgreSQL-specific mapping should live in `Configurations/`, not be scattered across services or controllers
- avoid embedding raw SQL that depends on SQL Server syntax

Suggested entity base:

```text
BaseEntity
|-- Id
|-- CreatedAt
|-- CreatedBy
|-- UpdatedAt
|-- UpdatedBy
|-- IsDeleted
```

Recommended shared interfaces:

- `IAuditable`
- `ISoftDelete`

Recommended PostgreSQL conventions inside repository project:

- keep entity names in singular PascalCase in C#
- map table names and column names explicitly in EF Core if the team wants snake_case in PostgreSQL
- use `DateTimeOffset` for audit fields in entities where timezone matters
- use partial unique indexes for cases like "one active staff assignment" or "one active subscription per vehicle"
- put advanced PostgreSQL mappings such as enum conversion, `jsonb`, or computed indexes in `Configurations/`

### 4. `EV_BatteryChangeStation_Service`

This is the business logic project.

Current important folders:

- `Base`
- `ExternalService`
- `InternalService`

Recommended responsibility:

- business rules
- role-based access checks
- validation before data write
- coordination across repositories
- transaction flow for booking, swap, payment, and support handling

Should contain:

- `InternalService/`
- `ExternalService/`
- `Requests/`
- `Responses/`
- `Interfaces/`
- `Validators/`
- `Mappings/`

Recommended detail:

- `InternalService` contains domain logic for system modules
- `ExternalService` contains gateway integrations such as payment or map provider
- service layer is the main place to enforce acceptance criteria from `UserStory`
- service layer should not contain PostgreSQL SQL text unless there is a clear performance reason and it is isolated behind repository methods

## Recommended Business Modules

To align with `BE_document_UserStory.md` and `BE_document_APIResult.md`, backend code should be organized around these modules:

### 1. Auth

Related APIs:

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `GET /api/v1/auth/me`

Main responsibility:

- account registration
- login
- current user context
- JWT generation and validation

Suggested service names:

- `IAuthService`
- `AuthService`

### 2. Stations

Related stories:

- `US-DRV-01`
- `US-DRV-02`
- `US-DRV-03`
- `US-DRV-04`
- `US-DRV-05`
- `US-ADM-01`
- `US-ADM-04`

Related APIs:

- `GET /api/v1/stations`
- `GET /api/v1/stations/{stationId}`
- `GET /api/v1/admin/stations`
- `POST /api/v1/admin/stations`
- `PATCH /api/v1/admin/stations/{stationId}`
- `DELETE /api/v1/admin/stations/{stationId}`

Main responsibility:

- station profile
- station search and filter
- map listing
- nearest station calculation
- station management by admin

Suggested service names:

- `IStationService`
- `StationService`

### 3. Vehicles

Related APIs:

- `POST /api/v1/driver/vehicles`
- `GET /api/v1/driver/vehicles`

Main responsibility:

- link driver vehicle
- map vehicle to battery type

Suggested service names:

- `IVehicleService`
- `VehicleService`

### 4. Subscriptions

Related APIs:

- `POST /api/v1/driver/subscriptions/purchase`
- `GET /api/v1/driver/subscriptions/current`
- `POST /api/v1/admin/subscription-plans`
- `PATCH /api/v1/admin/subscription-plans/{planId}`

Main responsibility:

- package management
- active subscription tracking
- remaining swap count

Suggested service names:

- `ISubscriptionService`
- `SubscriptionService`

### 5. Bookings

Related stories:

- `US-DRV-06`
- `US-STF-01`
- `US-STF-03`

Related APIs:

- `POST /api/v1/driver/bookings`
- `GET /api/v1/driver/bookings`
- `GET /api/v1/driver/bookings/{bookingId}`
- `GET /api/v1/staff/bookings`
- `PATCH /api/v1/staff/bookings/{bookingId}/decision`

Main responsibility:

- create booking
- validate subscription and compatible battery
- station queue handling
- approve or reject booking

Suggested service names:

- `IBookingService`
- `BookingService`

### 6. Inventory And Batteries

Related stories:

- `US-ADM-02`
- `US-STF-02`
- `US-STF-04`

Related APIs:

- `GET /api/v1/admin/inventory/summary`
- `GET /api/v1/staff/inventory`
- `PATCH /api/v1/staff/batteries/{batteryId}/status`
- `POST /api/v1/staff/battery-return-inspections`

Main responsibility:

- battery inventory per station
- available or in use or maintenance state
- SoH tracking
- returned battery inspection

Suggested service names:

- `IInventoryService`
- `InventoryService`

### 7. Swaps

Related stories:

- `US-STF-04`

Related APIs:

- `POST /api/v1/staff/swaps/complete`
- `GET /api/v1/driver/swaps/history`

Main responsibility:

- complete battery swap
- create swap transaction
- update booking status
- update issued and returned battery status

Suggested service names:

- `ISwapService`
- `SwapService`

### 8. Payments

Related APIs:

- `GET /api/v1/driver/payments`
- `POST /api/v1/staff/payments/record`

Main responsibility:

- payment history
- invoice tracking
- station fee recording
- external payment integration if needed

Suggested service names:

- `IPaymentService`
- `PaymentService`

### 9. Support

Related stories:

- `US-STF-05`

Related APIs:

- `POST /api/v1/driver/support-requests`
- `GET /api/v1/driver/support-requests`
- `GET /api/v1/admin/support-requests`
- `GET /api/v1/staff/support-requests`
- `PATCH /api/v1/staff/support-requests/{supportRequestId}/response`

Main responsibility:

- customer support request
- station-scoped handling
- admin oversight
- response history and audit

Suggested service names:

- `ISupportService`
- `SupportService`

### 10. Reports

Related APIs:

- `GET /api/v1/admin/reports/revenue`
- `GET /api/v1/admin/reports/station-demand`
- `GET /api/v1/admin/reports/demand-forecast`

Main responsibility:

- dashboard metrics
- demand summary
- revenue summary
- forecast output

Suggested service names:

- `IReportService`
- `ReportService`

## Recommended Module Layout Inside Service Project

To make service code align with business modules, use this structure:

```text
EV_BatteryChangeStation_Service
|-- Base
|-- Interfaces
|   |-- IAuthService.cs
|   |-- IStationService.cs
|   |-- IVehicleService.cs
|   |-- ISubscriptionService.cs
|   |-- IBookingService.cs
|   |-- IInventoryService.cs
|   |-- ISwapService.cs
|   |-- IPaymentService.cs
|   |-- ISupportService.cs
|   |-- IReportService.cs
|-- Requests
|   |-- Auth
|   |-- Stations
|   |-- Vehicles
|   |-- Subscriptions
|   |-- Bookings
|   |-- Inventory
|   |-- Swaps
|   |-- Payments
|   |-- Support
|   |-- Reports
|-- Responses
|   |-- Auth
|   |-- Stations
|   |-- Vehicles
|   |-- Subscriptions
|   |-- Bookings
|   |-- Inventory
|   |-- Swaps
|   |-- Payments
|   |-- Support
|   |-- Reports
|-- InternalService
|   |-- AuthService.cs
|   |-- StationService.cs
|   |-- VehicleService.cs
|   |-- SubscriptionService.cs
|   |-- BookingService.cs
|   |-- InventoryService.cs
|   |-- SwapService.cs
|   |-- PaymentService.cs
|   |-- SupportService.cs
|   |-- ReportService.cs
|-- ExternalService
|   |-- PaymentGatewayService.cs
|   |-- MapDistanceService.cs
|-- Validators
|   |-- Booking
|   |-- Station
|   |-- Support
```

## Recommended Module Layout Inside Repository Project

```text
EV_BatteryChangeStation_Repository
|-- Base
|-- DBContext
|   |-- AppDbContext.cs
|-- Entities
|   |-- User.cs
|   |-- Role.cs
|   |-- Station.cs
|   |-- StationStaffAssignment.cs
|   |-- Vehicle.cs
|   |-- BatteryType.cs
|   |-- Battery.cs
|   |-- SubscriptionPlan.cs
|   |-- UserSubscription.cs
|   |-- Booking.cs
|   |-- SwapTransaction.cs
|   |-- Payment.cs
|   |-- SupportRequest.cs
|   |-- Feedback.cs
|-- IRepositories
|   |-- IUserRepository.cs
|   |-- IStationRepository.cs
|   |-- IVehicleRepository.cs
|   |-- IBatteryRepository.cs
|   |-- ISubscriptionRepository.cs
|   |-- IBookingRepository.cs
|   |-- ISwapRepository.cs
|   |-- IPaymentRepository.cs
|   |-- ISupportRepository.cs
|   |-- IReportRepository.cs
|-- Repositories
|   |-- UserRepository.cs
|   |-- StationRepository.cs
|   |-- VehicleRepository.cs
|   |-- BatteryRepository.cs
|   |-- SubscriptionRepository.cs
|   |-- BookingRepository.cs
|   |-- SwapRepository.cs
|   |-- PaymentRepository.cs
|   |-- SupportRepository.cs
|   |-- ReportRepository.cs
|-- UnitOfWork
|   |-- IUnitOfWork.cs
|   |-- UnitOfWork.cs
```

Recommended PostgreSQL-specific files under repository project:

```text
EV_BatteryChangeStation_Repository
|-- DBContext
|   |-- AppDbContext.cs
|   |-- DesignTimeDbContextFactory.cs
|-- Configurations
|   |-- AccountConfiguration.cs
|   |-- StationConfiguration.cs
|   |-- BookingConfiguration.cs
|   |-- SwappingTransactionConfiguration.cs
|-- Migrations
```

## Suggested Controller Mapping

To keep API routing readable, use this controller grouping:

- `AuthController`
- `StationsController`
- `DriverController`
- `AdminController`
- `StaffController`

Recommended mapping:

- `AuthController`: auth endpoints only
- `StationsController`: public or shared station browsing endpoints
- `DriverController`: vehicle, booking, subscription, payment, support, feedback for driver
- `AdminController`: station management, staff assignment, subscription plan, report, admin support view
- `StaffController`: station-context, booking decision, inventory, battery inspection, swap completion, staff support response

## API To Controller Mapping

This section exists so the structure document still shows the API surface at a high level.

Detailed request and response payloads remain in `BE_document_APIResult.md`, but ownership of each endpoint should be visible here.

### 1. `AuthController`

Endpoints:

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `GET /api/v1/auth/me`

Service dependency:

- `IAuthService`
- `IAccountService`

### 2. `StationsController`

Endpoints:

- `GET /api/v1/stations`
- `GET /api/v1/stations/{stationId}`

Service dependency:

- `IStationService`

### 3. `DriverController`

Endpoints:

- `POST /api/v1/driver/vehicles`
- `GET /api/v1/driver/vehicles`
- `POST /api/v1/driver/bookings`
- `GET /api/v1/driver/bookings`
- `GET /api/v1/driver/bookings/{bookingId}`
- `POST /api/v1/driver/subscriptions/purchase`
- `GET /api/v1/driver/subscriptions/current`
- `GET /api/v1/driver/payments`
- `GET /api/v1/driver/swaps/history`
- `POST /api/v1/driver/support-requests`
- `GET /api/v1/driver/support-requests`
- `POST /api/v1/driver/feedback`

Service dependency:

- `IVehicleService`
- `IBookingService`
- `ISubscriptionService`
- `IPaymentService`
- `ISwapService`
- `ISupportService`
- `IFeedbackService`

### 4. `AdminController`

Endpoints:

- `GET /api/v1/admin/stations`
- `POST /api/v1/admin/stations`
- `PATCH /api/v1/admin/stations/{stationId}`
- `DELETE /api/v1/admin/stations/{stationId}`
- `GET /api/v1/admin/inventory/summary`
- `POST /api/v1/admin/staff-assignments`
- `GET /api/v1/admin/users`
- `POST /api/v1/admin/subscription-plans`
- `PATCH /api/v1/admin/subscription-plans/{planId}`
- `GET /api/v1/admin/support-requests`
- `GET /api/v1/admin/reports/revenue`
- `GET /api/v1/admin/reports/station-demand`
- `GET /api/v1/admin/reports/demand-forecast`

Service dependency:

- `IStationService`
- `IInventoryService`
- `IStaffAssignmentService`
- `IAccountService`
- `ISubscriptionService`
- `ISupportService`
- `IReportService`

### 5. `StaffController`

Endpoints:

- `GET /api/v1/staff/station-context`
- `GET /api/v1/staff/bookings`
- `PATCH /api/v1/staff/bookings/{bookingId}/decision`
- `GET /api/v1/staff/inventory`
- `PATCH /api/v1/staff/batteries/{batteryId}/status`
- `POST /api/v1/staff/swaps/complete`
- `POST /api/v1/staff/battery-return-inspections`
- `POST /api/v1/staff/payments/record`
- `GET /api/v1/staff/support-requests`
- `PATCH /api/v1/staff/support-requests/{supportRequestId}/response`

Service dependency:

- `IAccountService`
- `IBookingService`
- `IInventoryService`
- `ISwapService`
- `IPaymentService`
- `ISupportService`

## API Layer Folder Direction

To make API ownership visible in code, the API host project should not stop at a flat `Controllers/` folder.

Recommended direction:

```text
EV_BatteryChangeStation
|-- Controllers
|   |-- V1
|   |   |-- ApiControllerBase.cs
|   |   |-- AuthController.cs
|   |   |-- StationsController.cs
|   |   |-- DriverController.cs
|   |   |-- AdminController.cs
|   |   |-- StaffController.cs
|-- Contracts
|   |-- Common
|   |-- V1
|   |   |-- Auth
|   |   |-- Stations
|   |   |-- Vehicles
|   |   |-- Bookings
|   |   |-- Inventory
|   |   |-- Swaps
|   |   |-- Payments
|   |   |-- Support
|   |   |-- Reports
```

Meaning:

- `Controllers/` owns endpoint grouping and route versioning
- `Contracts/` owns request and response contracts closest to the API layer
- shared wrappers such as `ApiResponse` can live under `Contracts/Common`
- controller code should depend on service interfaces, never directly on repository classes

## Request And Response Placement Rule

To stay consistent with `BE_document_APIResult.md`:

- one request DTO per create or update action
- one response DTO per main endpoint output
- shared summary DTOs may be reused across modules if the shape is identical

Naming examples:

- `CreateBookingRequest`
- `BookingDetailResponse`
- `ApproveBookingRequest`
- `StationListItemResponse`
- `InventorySummaryResponse`
- `RespondSupportRequestRequest`

Do not:

- use entity directly as API request body
- return EF entity directly from controller
- mix request DTOs into repository project

## Validation Placement Rule

Validation should be split by layer:

### Controller level

- request body exists
- model binding success
- auth required

### Service level

- role and permission check
- station-scoped access check
- business rules from user story
- state transition validation

Examples:

- driver cannot create booking without active subscription
- staff cannot process booking from another station
- booking cannot be approved twice
- swap cannot complete if issued battery is not available

### Repository level

- filtered query execution
- uniqueness lookup
- transaction persistence
- PostgreSQL-friendly query optimization using indexes, projections, and server-side filtering

## PostgreSQL Implementation Rules

When implementing the refactor, the team should follow these PostgreSQL rules:

### 1. Connection and provider

- use `UseNpgsql(...)` in `Program.cs` or service registration
- keep connection string settings under configuration, not hard-coded
- use one `AppDbContext` per request scope

### 2. Naming and schema

- C# entity names may stay PascalCase
- database table and column names should consistently use either EF defaults or explicit snake_case mapping
- do not mix SQL Server naming assumptions into migrations

### 3. Default values and generated ids

- use PostgreSQL uuid generation strategy instead of SQL Server sequential guid functions
- use PostgreSQL current timestamp functions for database-side defaults
- prefer generating ids in application code or via PostgreSQL `gen_random_uuid()`

### 4. Date and time

- prefer `DateTimeOffset` in API, entity, and service boundaries when the time carries timezone meaning
- store operational timestamps in PostgreSQL as `timestamp with time zone`
- avoid ambiguous local server times for booking, payment, and swap events

### 5. Constraints and indexes

- use check constraints for status values where practical
- use partial unique indexes for active-only uniqueness rules
- add indexes based on real query paths such as station inventory, booking by station and status, and support request by account or station

### 6. Transactions

- booking approval, swap completion, battery state update, and payment record should run inside a transaction boundary
- repository and unit of work implementation must support PostgreSQL transaction handling cleanly
- concurrency-sensitive flows should be designed for PostgreSQL row locking behavior when needed

## RBAC And Scope Rule

This part must stay aligned with `BE_document_UserStory.md`.

### Driver

Can access:

- own vehicles
- own bookings
- own subscription
- own payments
- own support requests
- public station search and station detail

Cannot access:

- admin station management
- other user bookings
- station internal inventory

### Staff

Can access:

- assigned station context
- bookings of assigned station
- inventory of assigned station
- support requests of assigned station
- swap completion of assigned station

Cannot access:

- another station data
- admin-wide reports
- global user management

### Admin

Can access:

- all station summaries
- station management
- staff assignment
- inventory summary
- reports
- support monitoring

Can still be limited by fine-grained permissions:

- station-view
- inventory-view
- station-management
- staff-assignment

## Recommended Next Refactor Order

If the backend is refactored gradually, follow this order:

1. standardize response wrapper and exception middleware
2. standardize DTO naming for request and response
3. separate controller responsibility from service responsibility
4. group service code by business module
5. group repository code by entity and query responsibility
6. enforce RBAC and station-scope checks in service layer
7. finalize PostgreSQL provider, migrations, naming, and indexing conventions

## Final Note

This structure document is intentionally tied to the business documents.

- `BE_document_UserStory.md` answers: what the system must do
- `BE_document_APIResult.md` answers: what the API contract looks like
- `BE_document_Structure.md` answers: where the implementation for that contract should live

If later the team adds new APIs, they should also add:

1. story or business justification
2. API contract
3. module placement in this structure
