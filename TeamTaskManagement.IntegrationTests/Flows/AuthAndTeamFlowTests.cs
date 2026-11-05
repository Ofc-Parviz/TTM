using System.Net;
using FluentAssertions;
using TeamTaskManagement.Web.Contracts.Teams;
using TeamTaskManagement.IntegrationTests.Infrastructure;

namespace TeamTaskManagement.IntegrationTests.Flows;

public sealed class AuthAndTeamFlowTests
    : IClassFixture<WebAppFactory>, IDisposable {

    private readonly ApiCallsHelper _api;

    public AuthAndTeamFlowTests(WebAppFactory factory)
        => _api = new ApiCallsHelper(factory.CreateClient());

    [Fact]
    public async Task RegisteredUser_CanCreateAndRetrieveTeams() {

        var user = await _api.RegisterAndLoginAsync();

        _api.UseToken(user.AccessToken);

        var createResult = await _api
            .CreateTeamAsync(new CreateTeamRequest
            ("لیدر", "تیم تست یکپارچه", "توضیحات تیم"));

        createResult.StatusCode.Should().Be(HttpStatusCode.OK);

        var teams = (await _api.GetMyTeamsAsync()).GetRequiredContent();

        teams.Should().ContainSingle(team
            => team.TeamDisplayname == "تیم تست یکپارچه");
    }

    public void Dispose() => _api.Dispose();
}
