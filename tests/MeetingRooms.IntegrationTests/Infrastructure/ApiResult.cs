using System.Net;
using FluentAssertions;

namespace MeetingRooms.IntegrationTests.Infrastructure;

public record ApiResult(HttpStatusCode StatusCode)
{
    public void ShouldBe(HttpStatusCode expected) =>
        StatusCode.Should().Be(expected);
}

public record ApiResult<T>(HttpStatusCode StatusCode, T? Data) : ApiResult(StatusCode);
