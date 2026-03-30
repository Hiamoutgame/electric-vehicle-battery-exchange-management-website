# BE User Story Document

## Purpose

This document rewrites user stories in a BA-friendly format so that the team can later map them directly to:

- API permissions
- backend validation rules
- test cases
- role-based access control

## EV Driver Stories

### US-DRV-01

**Role:** EV Driver

**Goal:** View all battery swap stations on the map

**Benefit:** I can know which stations are available around me before choosing where to go.

**Acceptance Criteria:**

- Given I open the station search screen, when the system loads successfully, then I can see all active stations on the map.
- Given a station is inactive, when the map is displayed, then that station must not be shown as available for service.
- Given the map is loaded, when I select a station marker, then I can view basic station information.

### US-DRV-02

**Role:** EV Driver

**Goal:** Search stations by name, address, or area

**Benefit:** I can quickly find a suitable station without manually checking the whole map.

**Acceptance Criteria:**

- Given I enter a valid keyword, when I run the search, then the system returns stations matching the station name, address, or area.
- Given the keyword does not match any station, when the search completes, then the system shows a clear no-result message.
- Given the search result is returned, when I choose a station, then I can open its detail information.

### US-DRV-03

**Role:** EV Driver

**Goal:** Filter stations by battery type

**Benefit:** I can only view stations that are compatible with my vehicle battery type.

**Acceptance Criteria:**

- Given I select a battery type filter, when the filter is applied, then only stations supporting that battery type are shown.
- Given a station does not support the selected battery type, when the filter is applied, then that station must not appear in the result list.
- Given I clear the filter, when the system refreshes the result, then all active stations are shown again.

### US-DRV-04

**Role:** EV Driver

**Goal:** View station details

**Benefit:** I can decide whether the station is suitable for my trip and my battery swap needs.

**Acceptance Criteria:**

- Given I open a station detail screen, when the station exists, then I can see address, phone number, supported battery categories, and operating status.
- Given the station has battery inventory data, when I open station details, then I can see the available battery quantity.
- Given the station is closed or unavailable, when I open station details, then the system must show that status clearly.

### US-DRV-05

**Role:** EV Driver

**Goal:** View the nearest stations from my current location

**Benefit:** I can reduce travel time when my battery level is low.

**Acceptance Criteria:**

- Given I allow location access, when the system gets my location, then the system can calculate distance from me to each station.
- Given distance data is available, when I sort by nearest, then stations are ordered from shortest distance to farthest distance.
- Given my location cannot be determined, when the system fails to get GPS data, then the system must show a clear fallback message.

### US-DRV-06

**Role:** EV Driver

**Goal:** Book a battery swap slot after finding a suitable station

**Benefit:** I can reserve service in advance and reduce waiting time at the station.

**Acceptance Criteria:**

- Given I am authenticated, when I choose a station and submit a valid booking request, then the booking is created successfully.
- Given I do not have a valid subscription or enough remaining swap usage, when I attempt to book, then the system must reject the booking with a clear reason.
- Given the selected station has no compatible available battery, when I submit the booking, then the system must reject the booking.

## Admin Stories

### US-ADM-01

**Role:** Admin

**Goal:** View all stations on the map and in summary list

**Benefit:** I can monitor the overall station network and operational distribution.

**Acceptance Criteria:**

- Given I have station-view permission, when I open the admin station dashboard, then I can see all active and inactive stations.
- Given the system has battery inventory data, when I open station summary, then I can see battery quantity grouped by station.
- Given I do not have station-view permission, when I attempt to access the station dashboard, then the system must deny access.

### US-ADM-02

**Role:** Admin

**Goal:** View battery inventory summary by station

**Benefit:** I can identify stations that need inventory balancing or operational attention.

**Acceptance Criteria:**

- Given I have inventory-view permission, when I open the inventory summary screen, then I can see battery totals by station.
- Given a station has batteries in different states, when the summary is shown, then I can see available, in-use, and maintenance counts.
- Given I do not have inventory-view permission, when I access the inventory summary, then the system must deny access.

### US-ADM-03

**Role:** Admin

**Goal:** Assign staff to a specific station

**Benefit:** I can ensure each station has the correct responsible operators.

**Acceptance Criteria:**

- Given I have staff-assignment permission, when I assign a staff account to a station, then the assignment is saved successfully.
- Given a staff member is already assigned to another station and the business rule allows only one station, when I try to assign again, then the system must reject the request.
- Given I do not have staff-assignment permission, when I attempt to assign staff, then the system must deny access.

### US-ADM-04

**Role:** Admin

**Goal:** Manage station profile information

**Benefit:** Users and staff receive accurate station information for operation and navigation.

**Acceptance Criteria:**

- Given I have station-management permission, when I create or update station information, then the system saves address, phone number, and operating status correctly.
- Given required station data is missing, when I submit the form, then the system must reject the request with validation errors.
- Given I do not have station-management permission, when I attempt to create, update, or delete station data, then the system must deny access.

### US-ADM-05

**Role:** Admin

**Goal:** Access only the modules allowed by my permission scope

**Benefit:** Administrative operations remain controlled and auditable instead of giving full unrestricted power.

**Acceptance Criteria:**

- Given my admin account has limited permissions, when I access an unauthorized module, then the system must deny access.
- Given my admin account has view-only permission, when I try to perform create, update, or delete actions, then the system must reject the action.
- Given the system logs access control events, when an unauthorized admin action is blocked, then the event should be auditable.

## Station Staff Stories

### US-STF-01

**Role:** Station Staff

**Goal:** View bookings of my assigned station only

**Benefit:** I can prepare local service operations without seeing data from other stations.

**Acceptance Criteria:**

- Given I am assigned to a station, when I open the booking screen, then I can only see bookings belonging to my assigned station.
- Given a booking belongs to another station, when I attempt to access it, then the system must deny access.
- Given my account has no assigned station, when I open the booking screen, then the system must reject access with a clear message.

### US-STF-02

**Role:** Station Staff

**Goal:** View battery inventory of my assigned station only

**Benefit:** I can manage the battery readiness of the station where I work.

**Acceptance Criteria:**

- Given I am assigned to a station, when I open battery inventory, then I can only see batteries belonging to my assigned station.
- Given batteries exist in multiple statuses, when the inventory is shown, then I can identify available, in-use, and maintenance batteries in my station.
- Given I attempt to query inventory from another station directly, when the backend validates the request, then access must be denied.

### US-STF-03

**Role:** Station Staff

**Goal:** Confirm or reject bookings for my assigned station only

**Benefit:** I can control the service queue of the station where I am currently working.

**Acceptance Criteria:**

- Given a booking belongs to my assigned station and is pending, when I approve or reject it, then the booking status is updated successfully.
- Given a booking belongs to another station, when I try to approve or reject it, then the system must deny access.
- Given the booking is no longer in pending status, when I try to process it again, then the system must reject the action.

### US-STF-04

**Role:** Station Staff

**Goal:** Confirm battery swap completion for my assigned station only

**Benefit:** I can record completed service accurately and keep station inventory updated.

**Acceptance Criteria:**

- Given the booking belongs to my station and is approved, when I confirm swap completion, then the system creates the swap transaction successfully.
- Given the selected battery is no longer available, when I try to complete the swap, then the system must reject the action.
- Given the swap is completed successfully, when the transaction is saved, then battery status, station inventory, and booking status must be updated consistently.
- Given the booking belongs to another station, when I try to complete the swap, then the system must deny access.

### US-STF-05

**Role:** Station Staff

**Goal:** View support requests related to my assigned station only

**Benefit:** I can respond to local issues without accessing data from unrelated stations.

**Acceptance Criteria:**

- Given support requests are linked to my assigned station, when I open the support screen, then I can only see requests relevant to my station.
- Given a support request belongs to another station, when I try to access it, then the system must deny access.
- Given I update a support request response, when the save succeeds, then the response is stored with my staff identity.

### US-STF-06

**Role:** Station Staff

**Goal:** Access only station-scoped data and actions

**Benefit:** Cross-station operational mistakes and unauthorized data exposure are prevented.

**Acceptance Criteria:**

- Given I am authenticated as station staff, when I call a station-scoped API, then the backend must validate my assigned station before returning data.
- Given I try to update another station profile, inventory, booking, or transaction, when the backend validates the request, then the request must be denied.
- Given the frontend shows station-scoped screens, when my station assignment changes, then the returned data must follow my new assignment only.
