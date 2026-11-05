using System.Net;
using FluentAssertions;
using TeamTaskManagement.IntegrationTests.Infrastructure;
using TeamTaskManagement.Web.Contracts.Teams;
using Xunit;

namespace TeamTaskManagement.IntegrationTests.Flows;

public sealed class InviteRejectCancelAndLeaveFlowTests
    : IClassFixture<WebAppFactory> {

    private readonly WebAppFactory _factory;

    public InviteRejectCancelAndLeaveFlowTests
        (WebAppFactory factory) => _factory = factory;

    [Fact]
    public async Task RejectInvitation_DoesNot_AddMember_To_Team() {

        using var leaderApi = new ApiCallsHelper(_factory.CreateClient());
        using var memberApi = new ApiCallsHelper(_factory.CreateClient());

        await leaderApi.RegisterAndLoginAsync(authenticateClient: true);
        var member = await memberApi.RegisterAndLoginAsync(authenticateClient: true);

        var teamId = await TestScenarioHelper.CreateTeamAsync(leaderApi);

        var inviteId = await
            TestScenarioHelper.CreatePendingInviteAsync
            (leaderApi, memberApi, teamId, member.UserId);

        var rejectResult = await memberApi
            .RejectInviteAsync(new RejectInviteRequest(teamId, inviteId));

        rejectResult.EnsureSuccessStatusCode();

        var memberTeams = (await memberApi
            .GetMyTeamsAsync()).GetRequiredContent();

        memberTeams.Should().NotContain(team => team.TeamId == teamId);
    }

    [Fact]
    public async Task LeaderCancelsInvitation_ReceiverCannotAcceptIt() {

        using var leaderApi = new ApiCallsHelper(_factory.CreateClient());
        using var memberApi = new ApiCallsHelper(_factory.CreateClient());

        await leaderApi.RegisterAndLoginAsync(authenticateClient: true);

        var member = await memberApi.RegisterAndLoginAsync(authenticateClient: true);

        var teamId = await TestScenarioHelper.CreateTeamAsync(leaderApi);

        var inviteId = await TestScenarioHelper
            .CreatePendingInviteAsync(leaderApi, memberApi, teamId, member.UserId);

        var cancelResult = await leaderApi
            .CancelInviteAsync(new CancelInviteRequest(teamId, inviteId));

        cancelResult.EnsureSuccessStatusCode();

        var acceptResult = await memberApi
            .AcceptInviteAsync(new AcceptInviteRequest(teamId, inviteId));

        acceptResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MemberLeavesTeam_ActiveMembershipIsRemoved() {

        using var leaderApi = new ApiCallsHelper(_factory.CreateClient());
        using var memberApi = new ApiCallsHelper(_factory.CreateClient());

        await leaderApi.RegisterAndLoginAsync(authenticateClient: true);

        var member = await memberApi.RegisterAndLoginAsync(authenticateClient: true);

        var teamId = await TestScenarioHelper.CreateTeamAsync(leaderApi);

        await TestScenarioHelper.AcceptInviteAsync(leaderApi, memberApi, teamId, member.UserId);

        var leaveResult = await memberApi.LeaveTeamAsync(teamId);

        leaveResult.EnsureSuccessStatusCode();

        var memberTeams = (await memberApi.GetMyTeamsAsync()).GetRequiredContent();

        memberTeams.Should().NotContain(team => team.TeamId == teamId);
    }
}
