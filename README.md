# MeetingRooms Booking API

REST API for booking meeting rooms. Employees create and submit booking requests; admins confirm or decline them.

## Requirements

- .NET 10 SDK
- PostgreSQL 14+

## Configuration

Create `src/MeetingRooms.WebApi/appsettings.Local.json` (gitignored):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=MeetingRoomsDB;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

## Running

```bash
# Restore and build
dotnet build

# Apply migrations
dotnet ef database update \
  --project src/MeetingRooms.DataAccess \
  --startup-project src/MeetingRooms.WebApi

# Run
dotnet run --project src/MeetingRooms.WebApi
```

API available at `http://localhost:5290` and `https://localhost:7283`. Swagger UI at `https://localhost:7283/swagger`.

## Migrations

```bash
# Add new migration
dotnet ef migrations add <MigrationName> \
  --project src/MeetingRooms.DataAccess \
  --startup-project src/MeetingRooms.WebApi

# Rollback
dotnet ef database update <PreviousMigrationName> \
  --project src/MeetingRooms.DataAccess \
  --startup-project src/MeetingRooms.WebApi
```

## Architecture

Clean Architecture with 5 layers:

| Layer | Project | Responsibility |
|---|---|---|
| Domain | `MeetingRooms.Domain` | Entities, value objects, domain exceptions, state machine |
| Application | `MeetingRooms.Application` | CQRS commands/queries (MediatR), validators (FluentValidation), repository interfaces |
| DataAccess | `MeetingRooms.DataAccess` | EF Core DbContext, repositories, migrations |
| Infrastructure | `MeetingRooms.Infrastructure` | Middleware, user context, Serilog configuration |
| WebApi | `MeetingRooms.WebApi` | Controllers, DI composition root |

Dependency rule: outer layers depend on inner, Domain has zero external dependencies.

### Booking Workflow

```
Draft → Submitted → Confirmed
                 → Declined
Confirmed → Cancelled
```

**Submit** validates:
- Room is active
- Time slot is valid (`endAt > startAt`, max 4 hours)
- At least one attendee (valid email)
- No conflicting bookings (see Conflict Rules below)

All state transition rules are enforced in the domain entity — not in controllers or handlers.

### Conflict Rules

`HasConflictAsync` checks for time slot overlaps against bookings in **`Submitted` and `Confirmed`** statuses. This is an intentionally strict choice: it prevents two bookings for the same room/slot from both being submitted at the same time, reducing the chance of conflicts reaching the admin confirmation step.

Trade-off: a booking may be blocked by a `Submitted` request that is later declined. In that case the user can re-submit after the conflicting booking is declined.

### Concurrency Protection Against Double Confirmation

Two independent mechanisms defend against simultaneous confirmations:

**1. Partial unique index (database constraint)**

```sql
CREATE UNIQUE INDEX uix_room_confirmed_slot
ON booking_requests (room_id, start_at, end_at)
WHERE status = 'Confirmed';
```

Prevents two *different* bookings for the same room and time slot from both reaching `Confirmed` status. If two admins confirm conflicting bookings simultaneously, the second `INSERT`/`UPDATE` that would create the duplicate violates this constraint — PostgreSQL raises error `23505`, which is caught in `MeetingRoomsDbContext` and returned as HTTP 422.

**2. Optimistic concurrency via PostgreSQL `xmin`**

Each table row has a system column `xmin` (transaction ID of last write), used as a row version. If two admins load the *same* booking and both try to confirm it simultaneously, the second save finds that `xmin` no longer matches the value read at load time — EF Core raises `DbUpdateConcurrencyException`, caught and returned as HTTP 422.

Together these two mechanisms cover both cases:
- Same booking row modified concurrently → `xmin` catches it
- Different conflicting bookings both confirmed → unique index catches it

## Authentication

Simplified header-based auth (no JWT). Pass with every request:

| Header | Values |
|---|---|
| `X-User-Id` | Any valid GUID |
| `X-User-Role` | `Employee` or `Admin` |

## API Examples

### Rooms

**Create room (Admin)**
```bash
curl -X POST https://localhost:7283/api/rooms \
  -H "Content-Type: application/json" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Role: Admin" \
  -d '{"name": "Conference Room A", "capacity": 10, "location": "Floor 2"}'
```

**List rooms**
```bash
curl "https://localhost:7283/api/rooms?location=Floor%202&minCapacity=5&isActive=true" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Role: Employee"
```

### Bookings

**Create booking (Draft)**
```bash
curl -X POST https://localhost:7283/api/bookings \
  -H "Content-Type: application/json" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000002" \
  -H "X-User-Role: Employee" \
  -d '{
    "roomId": "<room-id>",
    "startAt": "2026-05-01T10:00:00Z",
    "endAt": "2026-05-01T11:00:00Z",
    "purpose": "Sprint planning",
    "attendees": ["alice@example.com", "bob@example.com"]
  }'
```

**Submit booking**
```bash
curl -X POST https://localhost:7283/api/bookings/<booking-id>/submit \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000002" \
  -H "X-User-Role: Employee"
```

**Confirm booking (Admin)**
```bash
curl -X POST https://localhost:7283/api/bookings/<booking-id>/confirm \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Role: Admin"
```

**Decline booking (Admin)**
```bash
curl -X POST https://localhost:7283/api/bookings/<booking-id>/decline \
  -H "Content-Type: application/json" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Role: Admin" \
  -d '{"reason": "Room reserved for executive meeting"}'
```

**Cancel booking (Employee)**
```bash
curl -X POST https://localhost:7283/api/bookings/<booking-id>/cancel \
  -H "Content-Type: application/json" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000002" \
  -H "X-User-Role: Employee" \
  -d '{"reason": "Meeting cancelled"}'
```

**Get booking details**
```bash
curl https://localhost:7283/api/bookings/<booking-id> \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000002" \
  -H "X-User-Role: Employee"
```

**Search bookings**
```bash
curl "https://localhost:7283/api/bookings?from=2026-05-01T00:00:00Z&to=2026-05-02T00:00:00Z&status=Submitted" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Role: Admin"
```

## Error Responses

All errors follow [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) ProblemDetails format:

```json
{
  "type": "https://httpstatuses.io/422",
  "title": "Unprocessable Entity",
  "status": 422,
  "detail": "The time slot conflicts with an existing booking.",
  "traceId": "0HN7R2KQVLMNO:00000001"
}
```

| Status | Cause |
|---|---|
| 400 | Validation error (invalid input, missing fields, invalid email) |
| 403 | Missing or invalid `X-User-Id`/`X-User-Role` headers, or insufficient role |
| 404 | Resource not found |
| 422 | Domain rule violation (wrong status, conflict, inactive room, concurrency) |
| 500 | Unexpected server error |
