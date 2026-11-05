using System.Net;
using FluentAssertions;
using TeamTaskManagement.IntegrationTests.Infrastructure;
using TeamTaskManagement.Web.Contracts.Teams;
using Xunit;

namespace TeamTaskManagement.IntegrationTests.Flows;

public sealed class AuthorizationFlowTests
    : IClassFixture<WebAppFactory>, IDisposable {

    private readonly ApiCallsHelper _api;

    public AuthorizationFlowTests(WebAppFactory factory)
        => _api = new ApiCallsHelper(factory.CreateClient());

    [Theory]
    [InlineData("teams")]
    [InlineData("profile")]
    [InlineData("invites")]
    [InlineData("tasks")]
    public async Task Protected_GetEndpoints_WithoutToken_Returns_Unauthorized(string endpoint) {

        _api.ClearToken();

        var statusCode = endpoint switch {
            "teams" => (await _api.GetMyTeamsAsync()).StatusCode,
            "profile" => (await _api.GetMyProfileAsync()).StatusCode,
            "invites" => (await _api.GetMyInvitesAsync()).StatusCode,
            "tasks" => (await _api.GetMyTasksAsync()).StatusCode,
            _ => throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, "اندپوینت ناشناخته است.")
        };

        statusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTeam_WithoutToken_Returns_Unauthorized() {

        _api.ClearToken();

        var response = await _api.CreateTeamAsync
            (new CreateTeamRequest
            ("لیدر", "تیم بدون مجوز", "این درخواست نباید پذیرفته شود"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public void Dispose() => _api.Dispose();
}
