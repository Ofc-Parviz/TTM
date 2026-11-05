using System.Net;
using FluentAssertions;
using TeamTaskManagement.Web.Contracts.Teams;
using TeamTaskManagement.IntegrationTests.Infrastructure;

namespace TeamTaskManagement.IntegrationTests.Flows;

public sealed class TeamMembershipFlowTests
    : IClassFixture<WebAppFactory> {

    private readonly WebAppFactory _factory;

    public TeamMembershipFlowTests
        (WebAppFactory factory) => _factory = factory;

    [Fact]
    public async Task Member_AcceptsInvitation_IsAddedToTeam() {

        using var leaderApi = new ApiCallsHelper(_factory.CreateClient());
        using var memberApi = new ApiCallsHelper(_factory.CreateClient());

        await leaderApi.RegisterAndLoginAsync(authenticateClient: true);
        var member = await memberApi.RegisterAndLoginAsync(authenticateClient: true);

        var teamId = await TestScenarioHelper.CreateTeamAsync(leaderApi);
        await TestScenarioHelper.AcceptInviteAsync(leaderApi, memberApi, teamId, member.UserId);

        var memberTeams = (await memberApi.GetMyTeamsAsync()).GetRequiredContent();

        memberTeams.Should().Contain(team => team.TeamId == teamId);
    }

    [Fact]
    public async Task NonLeader_Cannot_UpdateTeamDetails() {

        using var leaderApi = new ApiCallsHelper(_factory.CreateClient());
        using var memberApi = new ApiCallsHelper(_factory.CreateClient());

        await leaderApi.RegisterAndLoginAsync(authenticateClient: true);
        var member = await memberApi.RegisterAndLoginAsync(authenticateClient: true);

        var teamId = await TestScenarioHelper.CreateTeamAsync(leaderApi);
        await TestScenarioHelper.AcceptInviteAsync(leaderApi, memberApi, teamId, member.UserId);

        var updateResult = await memberApi.UpdateTeamAsync(new UpdateTeamRequest(
            teamId,
            "عضو",
            "نام غیرمجاز",
            "توضیحات غیرمجاز"));

        updateResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
