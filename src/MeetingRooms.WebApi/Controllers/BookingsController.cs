using MediatR;
using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Commands.Bookings.CancelBooking;
using MeetingRooms.Application.Commands.Bookings.ConfirmBooking;
using MeetingRooms.Application.Commands.Bookings.CreateBooking;
using MeetingRooms.Application.Commands.Bookings.DeclineBooking;
using MeetingRooms.Application.Commands.Bookings.SubmitBooking;
using MeetingRooms.Application.Extensions;
using MeetingRooms.Application.Queries.Bookings.GetBooking;
using MeetingRooms.Application.Queries.Bookings.SearchBookings;
using MeetingRooms.Contracts.Enums;
using MeetingRooms.Contracts.Requests.Booking;
using MeetingRooms.Contracts.Responses.Booking;
using MeetingRooms.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRooms.WebApi.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController(IMediator mediator, IUserContext userContext) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(BookingDetailsResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        userContext.RequireRole(UserRole.Employee);

        var command = new CreateBookingCommand
        {
            RoomId = request.RoomId,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            Purpose = request.Purpose,
            Attendees = request.Attendees,
            UserId = userContext.UserId
        };

        var booking = await mediator.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpPost("{id:guid}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
    {
        userContext.RequireRole(UserRole.Employee);

        var command = new SubmitBookingCommand 
        {
            BookingId = id,
            UserId = userContext.UserId
        };

        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken ct)
    {
        userContext.RequireRole(UserRole.Admin);

        var command = new ConfirmBookingCommand 
        { 
            BookingId = id,
            UserId = userContext.UserId
        };

        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/decline")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Decline(Guid id, [FromBody] DeclineBookingRequest request, CancellationToken ct)
    {
        userContext.RequireRole(UserRole.Admin);

        var command = new DeclineBookingCommand 
        { 
            BookingId = id, 
            Reason = request.Reason,
            UserId = userContext.UserId
        };

        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelBookingRequest request, CancellationToken ct)
    {
        userContext.RequireRole(UserRole.Employee);

        var command = new CancelBookingCommand 
        { 
            BookingId = id, 
            Reason = request.Reason,
            UserId = userContext.UserId
        };

        await mediator.Send(command, ct);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingDetailsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetBookingQuery { BookingId = id };

        var booking = await mediator.Send(query, ct);

        return Ok(booking);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<BookingSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] BookingSearchRequest request, CancellationToken ct)
    {
        var query = new SearchBookingsQuery
        {
            From = request.From,
            To = request.To,
            RoomId = request.RoomId,
            Status = request.Status.HasValue ? (BookingStatus)(int)request.Status.Value : null
        };

        var bookings = await mediator.Send(query, ct);
        return Ok(bookings);
    }
}
